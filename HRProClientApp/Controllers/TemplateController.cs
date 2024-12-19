using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HRProClientApp.Controllers
{
    public class TemplateController : Controller
    {
        private readonly ILogger<TemplateController> _logger;

        public TemplateController(ILogger<TemplateController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult TemplateDetails(int? id)
        {
            if (APIClient.User == null)
            {
                return Redirect("~/Home/Enter");
            }
            TemplateViewModel template;
            if (id.HasValue)
            {
                template = APIClient.GetRequest<TemplateViewModel?>($"api/template/details?id={id}");
                return View(template);
            }
            return View();
        }

        [HttpGet]
        public IActionResult Templates()
        {
            if (APIClient.User == null)
            {
                return Redirect("~/Home/Enter");
            }
            var templates = APIClient.GetRequest<List<TemplateViewModel>?>($"api/template/list?userId={APIClient.User?.Id}");
            return View(templates);
        }

        [HttpGet]
        public IActionResult EditTemplate(int? id)
        {
            if (APIClient.User == null)
            {
                return Redirect("~/Home/Enter");
            }
            if (!id.HasValue)
            {
                return View(new TemplateViewModel());
            }
            var model = APIClient.GetRequest<TemplateViewModel?>($"api/template/details?id={id}");
            return View(model);
        }

        [HttpPost]
        public IActionResult EditTemplate(TemplateBindingModel model)
        {
            string returnUrl = HttpContext.Request.Headers["Referer"].ToString();
            try
            {
                if (APIClient.User == null)
                {
                    throw new Exception("Доступно только авторизованным пользователям");
                }

                if (model.Id != 0)
                {
                    APIClient.PostRequest("api/template/update", model);
                }
                else
                {
                    APIClient.PostRequest("api/template/create", model);                    
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

                APIClient.PostRequest($"api/template/delete", new TemplateBindingModel { Id = id });
                APIClient.Company = APIClient.GetRequest<CompanyViewModel?>($"api/company/profile?id={APIClient.User?.CompanyId}");

                return Redirect("~/User/UserProfile");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { errorMessage = $"{ex.Message}", returnUrl });
            }
        }

        public IActionResult SearchTemplates(string? tags)
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
                    return View(new List<TemplateViewModel?>());
                }

                var results = APIClient.GetRequest<List<TemplateViewModel?>>($"api/template/search?tags={tags}");
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
