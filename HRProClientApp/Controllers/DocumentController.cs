using DocumentFormat.OpenXml.Office2010.Excel;
using HRProClientApp.Models;
using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;
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

        /*[HttpGet]
        public IActionResult DocumentEdit(int? id)
        {
            if (APIClient.User == null)
            {
                return Redirect("~/Home/Enter");
            }
            ViewBag.Templates = APIClient.GetRequest<List<TemplateViewModel>?>($"api/template/list");
            if (!id.HasValue)
            {
                return View(new DocumentViewModel());
            }            
            var model = APIClient.GetRequest<DocumentViewModel?>($"api/document/details?id={id}");
            if (model != null)
                return View(model);
            else
                return View();
        }*/

        [HttpGet]
        public IActionResult EditTags(int? templateId)
        {
            if (APIClient.User == null)
            {
                return Redirect("~/Home/Enter");
            }
            ViewBag.Tags = APIClient.GetRequest<List<TagViewModel>?>($"api/tag/list?templateId={templateId}");
            return View();
        }

        [HttpPost]
        public IActionResult EditTags(List<string> tagsValues)
        {
            if (APIClient.User == null)
            {
                return Redirect("~/Home/Enter");
            }
            foreach (var tag in tagsValues)
            {
                APIClient.PostRequest("api/tag/update", new TagBindingModel { TagName = tag });
            }
            return View();
        }


        [HttpGet]
        public IActionResult DocumentEdit(int? id)
        {
            if (APIClient.User == null)
            {
                return Redirect("~/Home/Enter");
            }
            ViewBag.Templates = APIClient.GetRequest<List<TemplateViewModel>?>($"api/template/list");
            if (!id.HasValue)
            {
                return View(new DocumentViewModel());
            }
            
            var model = APIClient.GetRequest<DocumentViewModel?>($"api/document/details?id={id}");
            if (model != null)
                return View(model);
            else
                return View();
/*
            try
            {
                var template = APIClient.GetRequest<TemplateViewModel>($"api/document/details?id={templateId}");

                if (template == null)
                {
                    return RedirectToAction("Error", new { errorMessage = "Шаблон не найден." });
                }

                var templatePath = $"{template.FilePath}";
                var tags = ExtractTags(templatePath);

                var model = new DocumentViewModel
                {
                    TemplateId = templateId.Value,
                    Tags = tags 
                };

                ViewBag.Templates = APIClient.GetRequest<List<TemplateViewModel>>("api/template/list");

                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { errorMessage = ex.Message });
            }*/
        }

        [HttpPost]
        public IActionResult DocumentEdit(DocumentBindingModel model)
        {
            string returnUrl = HttpContext.Request.Headers["Referer"].ToString();
            try
            {
                if (APIClient.User == null)
                {
                    throw new Exception("Доступно только авторизованным пользователям");
                }

                var template = APIClient.GetRequest<TemplateViewModel>($"api/template/details?id={model.TemplateId}");
                var templatePath = $"{template.FilePath}";
                var outputPath = $"GeneratedDocuments/Document_{model.Id}_{DateTime.Now.Ticks}.docx";

                var contentToFill = new Content();

                foreach (var tag in model.Tags)
                {
                    contentToFill.Append(new FieldContent(tag, Request.Form[tag]));
                }

                System.IO.File.Copy(templatePath, outputPath, true);
                using (var outputDoc = new TemplateProcessor(outputPath)
                    .SetRemoveContentControls(true))
                    {
                        outputDoc.FillContent(contentToFill);
                        outputDoc.SaveChanges();
                    }

                if (model.Id != 0)
                {
                    APIClient.PostRequest("api/document/update", model);
                }
                else
                {
                    model.CompanyId = APIClient.Company.Id;
                    model.CreatorId = APIClient.User.Id;
                    APIClient.PostRequest("api/document/create", model);
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
