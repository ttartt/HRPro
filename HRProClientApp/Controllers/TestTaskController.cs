using HRProClientApp.Models;
using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HRProClientApp.Controllers
{
    public class TestTaskController : Controller
    {
        private readonly ILogger<TestTaskController> _logger;

        public TestTaskController(ILogger<TestTaskController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult TestTaskDetails(int? id)
        {
            if (APIClient.User == null)
            {
                return Redirect("~/Home/Enter");
            }
            TestTaskViewModel testTask;
            if (id.HasValue)
            {
                testTask = APIClient.GetRequest<TestTaskViewModel?>($"api/testTask/details?id={id}");
                return View(testTask);
            }
            return View();
        }

        [HttpGet]
        public IActionResult TestTasks()
        {
            if (APIClient.User == null)
            {
                return Redirect("~/Home/Enter");
            }
            var testTasks = APIClient.GetRequest<List<TestTaskViewModel>?>($"api/testTask/list");
            return View(testTasks);
        }

        [HttpGet]
        public IActionResult TestTaskEdit(int? id)
        {
            if (APIClient.User == null)
            {
                return Redirect("~/Home/Enter");
            }
            if (!id.HasValue)
            {
                return View(new TestTaskViewModel());
            }
            var model = APIClient.GetRequest<TestTaskViewModel?>($"api/testTask/details?id={id}");
            return View(model);
        }

        [HttpPost]
        public IActionResult TestTaskEdit(TestTaskBindingModel model)
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
                    APIClient.PostRequest("api/testTask/update", model);
                }
                else
                {
                    APIClient.PostRequest("api/testTask/create", model);
                   
                }
                return Redirect($"~/Vacancy/Vacancies/{APIClient.Company.Id}");
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

                APIClient.PostRequest($"api/testTask/delete", new TestTaskBindingModel { Id = id });
                APIClient.Company = APIClient.GetRequest<CompanyViewModel?>($"api/company/profile?id={APIClient.User?.CompanyId}");

                return Redirect("~/User/UserProfile");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { errorMessage = $"{ex.Message}", returnUrl });
            }
        }

        public IActionResult SearchTestTasks(string? tags)
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
                    return View(new List<TestTaskViewModel?>());
                }

                var results = APIClient.GetRequest<List<TestTaskViewModel?>>($"api/testTask/search?tags={tags}");
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
