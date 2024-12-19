using HRProClientApp;
using HRProClientApp.Models;
using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using HRProDataModels.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
            string returnUrl = HttpContext.Request.Headers["Referer"].ToString();
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

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { errorMessage = $"{ex.Message}", returnUrl });
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string login, string password, string surname, string name, string lastname, DateTime dateOfBirth)
        {
            string returnUrl = HttpContext.Request.Headers["Referer"].ToString();
            try
            {
                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(surname) || string.IsNullOrEmpty(name))
                {
                    throw new Exception("Введите логин, пароль и ФИО");
                }

                RoleEnum role = RoleEnum.Неизвестен;

                if (login.Equals("tania.art03@gmail.com", StringComparison.OrdinalIgnoreCase))
                {
                    role = RoleEnum.Администратор;
                }
                else
                {
                    role = RoleEnum.Пользователь;
                }
                APIClient.PostRequest("api/user/register", new UserBindingModel
                {
                    Surname = surname,
                    Name = name,
                    LastName = lastname ?? null,
                    Email = login,
                    Password = password,
                    Role = role,
                    DateOfBirth = dateOfBirth
                });

                return RedirectToAction("Enter");
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
