using HRProClientApp.Models;
using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HRProClientApp.Controllers
{
    public class TagController : Controller
    {
        private readonly ILogger<TagController> _logger;

        public TagController(ILogger<TagController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult TagDetails(int? id)
        {
            if (APIClient.User == null)
            {
                return Redirect("~/Home/Enter");
            }
            TagViewModel tag;
            if (id.HasValue)
            {
                tag = APIClient.GetRequest<TagViewModel?>($"api/tag/details?id={id}");
                return View(tag);
            }
            return View();
        }

        [HttpGet]
        public IActionResult Tags()
        {
            if (APIClient.User == null)
            {
                return Redirect("~/Home/Enter");
            }
            var tags = APIClient.GetRequest<List<TagViewModel>?>($"api/tag/list?userId={APIClient.User?.Id}");
            return View(tags);
        }

        [HttpGet]
        public IActionResult TagEdit(int? id)
        {
            if (APIClient.User == null)
            {
                return Redirect("~/Home/Enter");
            }
            if (!id.HasValue)
            {
                return View(new TagViewModel());
            }
            var model = APIClient.GetRequest<TagViewModel?>($"api/tag/details?id={id}");
            return View(model);
        }

        [HttpPost]
        public IActionResult TagEdit(TagBindingModel model, int templateId)
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
                    APIClient.PostRequest("api/tag/update", model);
                }
                else
                {
                    model.TemplateId = templateId;
                    APIClient.PostRequest("api/tag/create", model);
                    var template = APIClient.GetRequest<TemplateViewModel>($"api/template/details?id={templateId}");
                    template.Tags.Add(new TagViewModel
                    {
                        Id = model.Id,
                        Name = model.Name,
                        TagName = model.TagName,
                        TemplateId = model.TemplateId,
                        TemplateName = template.Name,
                        Type = model.Type
                    });
                }
                return Redirect($"~/Template/TemplateEdit/{templateId}");
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

                APIClient.PostRequest($"api/tag/delete", new TagBindingModel { Id = id });
                APIClient.Company = APIClient.GetRequest<CompanyViewModel?>($"api/company/profile?id={APIClient.User?.CompanyId}");

                return Redirect("~/User/UserProfile");
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
