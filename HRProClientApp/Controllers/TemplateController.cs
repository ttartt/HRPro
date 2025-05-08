using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO.Compression;
using System.Xml;

namespace HRProClientApp.Controllers
{
    public class TemplateController : Controller
    {
        private readonly ILogger<TemplateController> _logger;
        private readonly IWebHostEnvironment _env;

        public TemplateController(ILogger<TemplateController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        [HttpGet]
        public IActionResult Templates()
        {
            if (APIClient.User == null)
            {
                return Redirect("~/Home/Enter");
            }
            var templates = APIClient.GetRequest<List<TemplateViewModel>?>($"api/template/list?companyId={APIClient.Company?.Id}");
            return View(templates);
        }

        [HttpPost]
        public async Task<IActionResult> UploadTemplate(IFormFile file, HRProDataModels.Enums.TemplateTypeEnum type, string name)
        {
            string redirectUrl = $"/Template/Templates";
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException("Нет названия шаблона");
                }
                if (file == null || file.Length == 0)
                    throw new ArgumentException("Файл не выбран");

                var extension = Path.GetExtension(file.FileName);
                if (string.IsNullOrEmpty(extension) || !extension.Equals(".docx"))
                {
                    throw new ArgumentException("Файл должен иметь расширение .docx");
                }

                var uploadResult = await APIClient.PostFileAsync("api/template/upload", file, name);
                var filePath = uploadResult.RelativePath;

                var model = new TemplateBindingModel()
                {
                    Name = name,
                    FilePath = filePath,
                    Type = type,
                    CompanyId = APIClient.Company?.Id
                };


                if (APIClient.User == null)
                {
                    throw new Exception("Доступно только авторизованным пользователям");
                }

                if (APIClient.Company == null)
                {
                    throw new Exception("Компания не определена");
                }
                if (model.Id == 0)
                {
                    var existingTemplates = APIClient.GetRequest<List<TemplateViewModel>>($"api/template/list?companyId={APIClient.Company.Id}");
                    var duplicate = existingTemplates?.FirstOrDefault(v =>
                        v.Name.Equals(model.Name, StringComparison.OrdinalIgnoreCase) &&
                        v.FilePath == model.FilePath);

                    if (duplicate != null)
                    {
                        throw new InvalidOperationException("Такой шаблон документа уже существует");
                    }
                }
                var templateId = await APIClient.PostRequestAsync("api/template/create", model);              

                var response = await APIClient.PostRequestWithFullResponseAsync(
                "api/template/AnalyzeDocx",
                new { FileName = uploadResult.FileName });

                var responseJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Ошибка анализа файла: {responseJson}");
                }

                var tags = JsonConvert.DeserializeObject<List<string>>(responseJson);

                var templateViewModel = APIClient.GetRequest<TemplateViewModel?>($"api/template/details?id={templateId}");
                foreach (var tag in tags)
                {
                    var tagModel = new TagBindingModel
                    {
                        TemplateId = templateId,
                        TagName = tag
                    };
                    var tagId = await APIClient.PostRequestAsync("api/tag/create", tagModel);
                    templateViewModel.Tags.Add(new TagViewModel
                    {
                        Id = tagId,
                        TagName = tagModel.TagName,
                        TemplateId = tagModel.TemplateId
                    });
                }

                return Json(new { success = true, redirectUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> Download(int id)
        {
            try
            {
                var template = APIClient.GetRequest<TemplateViewModel>($"api/template/details?id={id}");
                if (template == null)
                {
                    throw new Exception("Шаблон не найден");
                }

                var response = await APIClient.GetRequestWithFullResponseAsync($"api/template/download?id={id}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }

                var fileStream = await response.Content.ReadAsStreamAsync();

                var contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

                return File(fileStream, contentType, template.Name + ".docx");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult Delete(int id)
        {
            if (APIClient.Company == null)
            {
                throw new Exception("Компания не определена");
            }

            APIClient.PostRequest($"api/template/delete", new TemplateBindingModel { Id = id });

            APIClient.Company = APIClient.GetRequest<CompanyViewModel?>($"api/company/profile?id={APIClient.User?.CompanyId}");
            return Redirect($"~/Template/Templates");
        }

    }
}
