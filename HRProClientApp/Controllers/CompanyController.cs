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
            string redirectUrl = $"/Company/CompanyProfile/{APIClient.Company?.Id}";
            try
            {
                if (APIClient.User == null)
                {
                    throw new Exception("Доступно только авторизованным пользователям");
                }
                if (string.IsNullOrEmpty(model.Name))
                {
                    throw new ArgumentException("Нет названия компании");
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
                return Json(new { success = true, redirectUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
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
    }
}
