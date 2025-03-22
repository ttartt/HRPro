using HRProClientApp.Models;
using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
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
            if (!id.HasValue)
            {
                return View(new DocumentViewModel());
            }
            ViewBag.Templates = APIClient.GetRequest<List<TemplateViewModel>>("api/template/list");
            var model = APIClient.GetRequest<DocumentViewModel?>($"api/document/details?id={id}");
            return View(model);
        }

        [HttpPost]
        public IActionResult DocumentEdit(DocumentBindingModel model)
        {
/*
            File.Delete("OutputDocument.docx");
            File.Copy("InputTemplate.docx", "OutputDocument.docx");

            var valuesToFill = new Content(
                new FieldContent("Report date", DateTime.Now.ToString()));

            using (var outputDocument = new TemplateProcessor("OutputDocument.docx")
                .SetRemoveContentControls(true))
            {
                outputDocument.FillContent(valuesToFill);
                outputDocument.SaveChanges();
            }*/


            string returnUrl = HttpContext.Request.Headers["Referer"].ToString();
            try
            {
                if (APIClient.User == null)
                {
                    throw new Exception("Доступно только авторизованным пользователям");
                }
                
                if (model.Id != 0)
                {
                    APIClient.PostRequest("api/document/update", model);
                }
                else
                {
                    model.CompanyId = APIClient.Company.Id;
                    APIClient.PostRequest("api/document/create", model);
                    if (APIClient.Company != null)
                    {
                        APIClient.User?.Documents.Add(new DocumentViewModel
                        {
                            Id = model.Id,
                            CompanyId = APIClient.Company.Id,
                            CreatedAt = DateTime.Now.ToUniversalTime().AddHours(4),
                            CompanyName = APIClient.Company.Name,
                            CreatorId = APIClient.User.Id,
                            CreatorName = APIClient.User.Name,
                            Name = model.Name,
                            Status = model.Status,
                            TemplateId = model.TemplateId,
                            TemplateName = APIClient.GetRequest<TemplateViewModel>($"api/template/details?id={model.TemplateId}").Name
                        });
                    }
                }
                return Redirect($"~/User/UserProfile/{APIClient.User?.Id}");
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

                return Redirect("~/User/UserProfile");
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
