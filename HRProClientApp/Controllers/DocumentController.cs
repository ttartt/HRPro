using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using HRProDataModels.Enums;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using TemplateEngine.Docx;

namespace HRProClientApp.Controllers
{
    public class DocumentController : Controller
    {
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(ILogger<DocumentController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult DocumentDetails(int? id)
        {
            if (APIClient.User == null)
            {
                return Redirect("~/Home/Enter");
            }
            DocumentViewModel document;
            if (id.HasValue)
            {
                document = APIClient.GetRequest<DocumentViewModel?>($"api/document/details?id={id}");
                return View(document);
            }
            return View();
        }

        [HttpGet]
        public IActionResult Documents()
        {
            if (APIClient.User == null)
            {
                return Redirect("~/Home/Enter");
            }
            var documents = APIClient.GetRequest<List<DocumentViewModel>?>($"api/document/list?userId={APIClient.User?.Id}&companyId={APIClient.Company?.Id}");
            return View(documents);
        }

        [HttpGet]
        public IActionResult DocumentEdit(int? id)
        {
            if (APIClient.User == null)
            {
                return Redirect("~/Home/Enter");
            }

            var templates = APIClient.GetRequest<List<TemplateViewModel>>($"api/template/list?companyId={APIClient.Company?.Id}") ?? new List<TemplateViewModel>();
            ViewBag.Templates = templates;

            DocumentViewModel model = id.HasValue
                ? APIClient.GetRequest<DocumentViewModel?>($"api/document/details?id={id}") ?? new DocumentViewModel()
                : new DocumentViewModel();

            return View(model);
        }


        [HttpGet]
        public IActionResult LoadTags(int? templateId)
        {
            if (APIClient.User == null)
            {
                return Unauthorized("Доступ запрещен. Необходимо авторизоваться.");
            }

            if (!templateId.HasValue)
            {
                return BadRequest("TemplateId не указан.");
            }

            var tags = APIClient.GetRequest<List<TagViewModel>>($"api/template/tags?templateId={templateId}");

            return Json(tags ?? new List<TagViewModel>());
        }

        public async Task<IActionResult> Download(int id)
        {
            try
            {
                var document = APIClient.GetRequest<DocumentViewModel>($"api/document/details?id={id}");
                if (document == null)
                {
                    throw new Exception("Документ не найден");
                }

                var response = await APIClient.GetRequestWithFullResponseAsync($"api/document/download?id={id}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }

                var fileStream = await response.Content.ReadAsStreamAsync();

                var contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

                return File(fileStream, contentType, document.Name + ".docx");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> DocumentEdit(DocumentBindingModel model)
        {
            string redirectUrl = $"/Document/Documents?userId={APIClient.User?.Id}";
            try
            {
                if (APIClient.User == null) throw new Exception("Доступно только авторизованным пользователям");
                if (APIClient.Company == null) throw new Exception("Компания не найдена");
                if (string.IsNullOrEmpty(model.Name)) throw new ArgumentException("Название документа не может быть пустым");
                if (!Enum.IsDefined(typeof(DocumentStatusEnum), model.Status)) throw new ArgumentException("Некорректный статус документа");
                if (model.TemplateId <= 0) throw new ArgumentException("Не выбран шаблон документа");

                if (model.Id == 0)
                {
                    var existingDocuments = APIClient.GetRequest<List<DocumentViewModel>>($"api/document/list?companyId={model.CompanyId}&userId={APIClient.User.Id}");
                    if (existingDocuments?.Any(v => v.Name.Equals(model.Name, StringComparison.OrdinalIgnoreCase)) == true)
                        throw new InvalidOperationException("Такой документ уже существует");
                }

                var template = APIClient.GetRequest<TemplateViewModel>($"api/template/details?id={model.TemplateId}");
                var tags = APIClient.GetRequest<List<TagViewModel>?>($"api/template/tags?templateId={template.Id}");

                using (var templateResponse = await APIClient.GetRequestWithFullResponseAsync($"api/template/File?id={model.TemplateId}"))
                using (var templateStream = await templateResponse.Content.ReadAsStreamAsync())
                using (var outputStream = new MemoryStream())
                {
                    templateStream.CopyTo(outputStream);
                    outputStream.Position = 0;

                    using (var docProcessor = new TemplateProcessor(outputStream).SetRemoveContentControls(true))
                    {
                        var content = new Content();
                        foreach (var tag in tags)
                        {
                            var value = Request.Form[$"Tags[{tag.Id}]"];
                            if (!string.IsNullOrEmpty(value))
                                content.Fields.Add(new FieldContent(tag.TagName, value));
                        }
                        docProcessor.FillContent(content);
                        docProcessor.SaveChanges();
                    }

                    outputStream.Position = 0;
                    var formFields = new Dictionary<string, string>
                    {
                        { "documentId", model.Id.ToString() },
                        { "templateId", model.TemplateId.ToString() }
                    };

                    var uploadResponse = await APIClient.PostFileWithFullResponseAsync(
                        "api/document/SaveDocument",
                        outputStream,
                        $"{model.Name}.docx",
                        formFields);

                    if (!uploadResponse.IsSuccessStatusCode)
                    {
                        throw new Exception(await uploadResponse.Content.ReadAsStringAsync());
                    }

                    var uploadResult = JsonConvert.DeserializeObject<dynamic>(await uploadResponse.Content.ReadAsStringAsync());
                    model.FilePath = "Uploads\\Documents\\" + uploadResult.fileName;
                }                

                int createdDocumentId;
                if (model.Id != 0)
                {
                    APIClient.PostRequest("api/document/update", model);
                    createdDocumentId = model.Id;
                }
                else
                {
                    model.CompanyId = APIClient.Company.Id;
                    model.CreatorId = APIClient.User.Id;
                    createdDocumentId = await APIClient.PostRequestAsync("api/document/create", model);
                }

                if (createdDocumentId <= 0)
                {
                    throw new Exception("Сервер вернул недопустимый ID документа");
                }

                foreach (var tag in tags)
                {
                    var tagValue = Request.Form[$"Tags[{tag.Id}]"];
                    if (!string.IsNullOrEmpty(tagValue))
                    {
                        var tagModel = new DocumentTagBindingModel
                        {
                            DocumentId = createdDocumentId,
                            TagId = tag.Id,
                            Value = tagValue
                        };
                        APIClient.PostRequest("api/document/createTag", tagModel);
                    }
                }

                return Json(new { success = true, redirectUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private async Task<Stream> GetTemplateStream(int templateId)
        {
            var response = await APIClient.GetRequestWithFullResponseAsync($"api/template/file?id={templateId}");
            if (!response.IsSuccessStatusCode)
                throw new Exception("Ошибка получения шаблона");

            return await response.Content.ReadAsStreamAsync();
        }

        public IActionResult Delete(int id)
        {
            if (APIClient.Company == null)
            {
                throw new Exception("Компания не определена");
            }

            APIClient.PostRequest($"api/document/delete", new DocumentBindingModel { Id = id });
            APIClient.Company = APIClient.GetRequest<CompanyViewModel?>($"api/company/profile?id={APIClient.User?.CompanyId}");
            return Redirect($"~/Document/Documents?userId={APIClient.User?.Id}");
        }
    }
}
