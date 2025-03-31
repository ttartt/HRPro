using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Office2010.Excel;
using HRProClientApp.Models;
using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Template;
using System.Diagnostics;
using System.IO.Compression;
using System.Xml;
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
            var documents = APIClient.GetRequest<List<DocumentViewModel>?>($"api/document/list?userId={APIClient.User?.Id}");
            return View(documents);
        }

        [HttpGet]
        public IActionResult DocumentEdit(int? id)
        {
            if (APIClient.User == null)
            {
                return Redirect("~/Home/Enter");
            }

            var templates = APIClient.GetRequest<List<TemplateViewModel>>("api/template/list") ?? new List<TemplateViewModel>();
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
            string returnUrl = HttpContext.Request.Headers["Referer"].ToString();
            try
            {
                if (APIClient.User == null)
                {
                    throw new Exception("Доступно только авторизованным пользователям");
                }

                var template = APIClient.GetRequest<TemplateViewModel>($"api/template/details?id={model.TemplateId}");
                if (template == null)
                {
                    throw new Exception("Шаблон не найден.");
                }

                var templatePath = $"{template.FilePath}";
                var outputPath = $"GeneratedDocuments/{model.Name}_{DateTime.Now.Date.ToShortDateString()}.docx";

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

                return Redirect($"~/Document/Documents?userId={APIClient.User?.Id}");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { errorMessage = $"{ex.Message}", returnUrl });
            }
        }

        public IActionResult Delete(int id)
        {
            string returnUrl = HttpContext.Request.Headers["Referer"].ToString();
            try
            {
                if (APIClient.Company == null)
                {
                    throw new Exception("Компания не определена");
                }

                APIClient.PostRequest($"api/document/delete", new DocumentBindingModel { Id = id });
                APIClient.Company = APIClient.GetRequest<CompanyViewModel?>($"api/company/profile?id={APIClient.User?.CompanyId}");

                return Redirect($"~/Document/Documents?userId={APIClient.User?.Id}");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { errorMessage = $"{ex.Message}", returnUrl });
            }
        }

        public IActionResult SearchDocuments(string? tags)
        {
            string returnUrl = HttpContext.Request.Headers["Referer"].ToString();
            try
            {
                if (APIClient.User == null)
                {
                    throw new Exception("Доступно только авторизованным пользователям");
                }

                if (string.IsNullOrEmpty(tags))
                {
                    ViewBag.Message = "Пожалуйста, введите поисковый запрос.";
                    return View(new List<DocumentViewModel?>());
                }

                var results = APIClient.GetRequest<List<DocumentViewModel?>>($"api/document/search?tags={tags}");
                return View(results);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { errorMessage = $"{ex.Message}", returnUrl });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string errorMessage, string returnUrl)
        {
            ViewBag.ErrorMessage = errorMessage ?? "Произошла непредвиденная ошибка.";
            ViewBag.ReturnUrl = returnUrl;
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
