using HRProClientApp.Models;
using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Xml.Linq;

namespace HRProClientApp.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ILogger<CompanyController> _logger;
        public CompanyController(ILogger<CompanyController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult CompanyProfile(int? id)
        {
            if (APIClient.User == null)
            {
                return Redirect("~/Home/Enter");
            }
            if (id.HasValue)
            {
                APIClient.Company = APIClient.GetRequest<CompanyViewModel?>($"api/company/profile?id={id}");
            }
            var model = APIClient.Company;
            return View(model);
        }

        [HttpGet]
        public IActionResult EditCompanyProfile(int? id)
        {
            if (APIClient.User == null)
            {
                return Redirect("~/Home/Enter");
            }
            if (!id.HasValue)
            {
                return View(new CompanyViewModel());
            }
            var model = APIClient.GetRequest<CompanyViewModel?>($"api/company/profile?id={id}");            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditCompanyProfile(CompanyBindingModel model)
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
                    APIClient.PostRequest("api/company/update", model);                    
                }
                else
                {
                    var companyId = await APIClient.PostRequestAsync("api/company/create", model);
                    APIClient.PostRequest("api/user/update", new UserBindingModel
                    {
                        Id = APIClient.User.Id,
                        Surname = APIClient.User.Surname,
                        Name = APIClient.User.Name,
                        LastName = APIClient.User.LastName,
                        CompanyId = companyId,
                        Email = APIClient.User.Email,
                        Password = APIClient.User.Password,
                        Role = APIClient.User.Role,
                        PhoneNumber = APIClient.User.PhoneNumber,
                        DateOfBirth = APIClient.User.DateOfBirth
                    });
                    APIClient.Company = APIClient.GetRequest<CompanyViewModel?>($"api/company/profile?id={companyId}");
                }
                if (APIClient.Company == null)
                {
                    throw new Exception("Компания не определена");
                }
                return Redirect($"~/Company/CompanyProfile/{APIClient.Company.Id}");            
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { errorMessage = $"{ex.Message}", returnUrl });
            }  
        }

        [HttpPost]
        public void Delete(int id)
        {
            if (APIClient.User == null)
            {
                throw new Exception("Доступно только авторизованным пользователям");
            }

            APIClient.PostRequest($"api/company/delete", new CompanyBindingModel { Id = id });
            Response.Redirect("/Home/Index");
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
