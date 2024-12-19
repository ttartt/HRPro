using HRProClientApp.Models;
using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HRProClientApp.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult UserProfile(int? id)
        {
            var model = APIClient.GetRequest<UserViewModel>($"api/user/profile?id={id}"); 

            if (model == null)
            {
                return Redirect("/Home/Index");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult UserProfileEdit(int? id)
        {
            if (APIClient.User == null)
            {
                return Redirect("/Home/Enter");
            }

            if (APIClient.Company == null)
            {
                return Redirect("/Home/Index");
            }

            if (!id.HasValue)
            {
                var model = new UserViewModel
                {
                    CompanyId = APIClient.Company.Id
                };
                return View(model);
            }
            else if (id.HasValue)
            {
                var employee = APIClient.GetRequest<UserViewModel?>($"api/user/profile?id={id}");
                return View(employee);
            }
            else
            {
                var model = APIClient.GetRequest<UserViewModel?>($"api/user/profile?id={APIClient.User.Id}");
                return View(model);
            }            
        }

        [HttpPost]
        public IActionResult UserProfileEdit(UserBindingModel model)
        {
            string returnUrl = HttpContext.Request.Headers["Referer"].ToString();
            try
            {
                if (model.Id != 0)
                {
                    APIClient.PostRequest("api/user/update", model);
                    if (model.Role == HRProDataModels.Enums.RoleEnum.Сотрудник)
                    {
                        return Redirect($"~/Company/CompanyProfile/{model.CompanyId}");
                    }
                }
                else
                {
                    var existingUser = APIClient.GetRequest<UserViewModel?>($"api/user/login?login={model.Email}&password={model.Password}");
                    if (existingUser != null)
                    {
                        throw new Exception("Такой пользователь уже существует");
                    }
                    APIClient.PostRequest("api/user/register", model);
                    if (APIClient.Company != null)
                    {
                        APIClient.Company?.Employees.Add(new UserViewModel
                        {
                            Id = model.Id,
                            Surname = model.Surname,
                            Name = model.Name,
                            LastName = model.LastName,
                            CompanyId = APIClient.Company.Id,
                            Email = model.Email,
                            Password = model.Password,
                            Role = HRProDataModels.Enums.RoleEnum.Сотрудник,
                            PhoneNumber = model.PhoneNumber,
                            DateOfBirth = model.DateOfBirth
                        });
                    }
                    return Redirect($"~/Company/CompanyProfile/{model.CompanyId}");
                }
                return Redirect($"/User/UserProfile/{model.Id}");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { errorMessage = $"{ex.Message}", returnUrl });
            }
        }

        public IActionResult DeleteEmployee(int id)
        {
            string returnUrl = HttpContext.Request.Headers["Referer"].ToString();
            try
            {
                APIClient.PostRequest("api/user/delete", new UserBindingModel
                {
                    Id = id
                });
                APIClient.Company = APIClient.GetRequest<CompanyViewModel?>($"api/company/profile?id={APIClient.User?.CompanyId}");

                return Redirect($"~/Company/CompanyProfile");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { errorMessage = $"{ex.Message}", returnUrl });
            }            
        }

        [HttpGet]
        public void Logout()
        {
            APIClient.User = null;
            APIClient.Company = null;
            Response.Redirect("/Home/Enter");
        }

        [HttpPost]
        public void Delete(UserBindingModel model)
        {
            if (APIClient.User == null)
            {
                throw new Exception("Доступно только авторизованным пользователям");
            }

            APIClient.PostRequest($"api/user/delete", model);
            Response.Redirect("/Home/Enter");
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
