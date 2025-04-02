using HRProClientApp;
using HRProClientApp.Models;
using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HRProUserApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Enter()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Enter(string login, string password)
        {
            string redirectUrl = "/Home/Index";
            try
            {
                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                {
                    throw new Exception("Введите логин и пароль");
                }
                APIClient.User = await APIClient.GetRequestAsync<UserViewModel>($"api/user/login?login={login}&password={password}");
                if (APIClient.User == null)
                {
                    throw new Exception("Неверный логин/пароль");
                }
                if (APIClient.User?.CompanyId != null)
                {
                    APIClient.Company = await APIClient.GetRequestAsync<CompanyViewModel>($"api/company/profile?id={APIClient.User?.CompanyId}");
                }

                return Json(new { success = true, redirectUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
