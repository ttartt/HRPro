using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using HRProDataModels.Enums;
using Microsoft.AspNetCore.Mvc;
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


        [HttpPost]
        public async Task<IActionResult> DocumentEdit(DocumentBindingModel model)
        {
            string redirectUrl = $"/Document/Documents?userId={APIClient.User?.Id}";
            try
            {
                if (APIClient.User == null)
                {
                    throw new Exception("Доступно только авторизованным пользователям");
                }

                if (APIClient.Company == null)
                {
                    throw new Exception("Компания не найдена");
                }

                if (string.IsNullOrEmpty(model.Name))
                {
                    throw new ArgumentException("Название документа не может быть пустым");
                }

                if (!Enum.IsDefined(typeof(DocumentStatusEnum), model.Status))
                {
                    throw new ArgumentException("Некорректный статус документа");
                }

                if (model.TemplateId <= 0 || model.TemplateId == null)
                {
                    throw new ArgumentException("Не выбран шаблон документа.");
                }

                if (model.Id == 0)
                {
                    var existingDocuments = APIClient.GetRequest<List<DocumentViewModel>>($"api/document/list?companyId={model.CompanyId}&userId={APIClient.User.Id}");
                    var duplicate = existingDocuments?.FirstOrDefault(v =>
                        v.Name.Equals(model.Name, StringComparison.OrdinalIgnoreCase));

                    if (duplicate != null)
                    {
                        throw new InvalidOperationException("Такой документ уже существует");
                    }
                }

                var template = APIClient.GetRequest<TemplateViewModel>($"api/template/details?id={model.TemplateId}");

                var templatePath = $"{template.FilePath}";
                var outputFolder = "GeneratedDocuments";
                var outputPath = $"{outputFolder}/{model.Name}_{DateTime.Now:yyyy-MM-dd}.docx";

                var contentToFill = new Content();
                var tags = APIClient.GetRequest<List<TagViewModel>?>($"api/template/tags?templateId={template.Id}");

                foreach (var tag in tags)
                {
                    var tagValue = Request.Form[$"Tags[{tag.Id}]"];
                    if (!string.IsNullOrEmpty(tagValue))
                    {
                        contentToFill.Append(new FieldContent(tag.TagName, tagValue));
                    }
                }

                System.IO.Directory.CreateDirectory(outputFolder);
                System.IO.File.Copy(templatePath, outputPath, true);

                using (var outputDoc = new TemplateProcessor(outputPath).SetRemoveContentControls(true))
                {
                    var content = new Content();

                    foreach (var tag in tags)
                    {
                        var value = Request.Form[$"Tags[{tag.Id}]"];
                        if (!string.IsNullOrEmpty(value))
                        {
                            content.Fields.Add(new FieldContent(tag.TagName, value));
                        }
                    }

                    outputDoc.FillContent(content);
                    outputDoc.SaveChanges();
                }

                var createdDocumentId = 0;
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

                Process.Start(new ProcessStartInfo
                {
                    FileName = Path.GetFullPath(outputFolder),
                    UseShellExecute = true
                });
                
                return Json(new { success = true, redirectUrl });
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

            APIClient.PostRequest($"api/document/delete", new DocumentBindingModel { Id = id });
            APIClient.Company = APIClient.GetRequest<CompanyViewModel?>($"api/company/profile?id={APIClient.User?.CompanyId}");
            return Redirect($"~/Document/Documents?userId={APIClient.User?.Id}");
        }
    }
}
