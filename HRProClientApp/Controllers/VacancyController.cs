using HRProClientApp.Models;
using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace HRProClientApp.Controllers
{
    public class VacancyController : Controller
    {
        private readonly ILogger<VacancyController> _logger;

        public VacancyController(ILogger<VacancyController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult VacancyStatistics(int id)
        {
            if (APIClient.User == null)
            {
                return Redirect("/Home/Enter");
            }

            var vacancy = APIClient.GetRequest<VacancyViewModel>($"/api/vacancy/details?id={id}");
            if (vacancy == null)
            {
                return NotFound("Вакансия не найдена.");
            }

            var viewModel = new VacancyStatisticsViewModel
            {
                DateFrom = DateTime.UtcNow,
                DateTo = DateTime.UtcNow,
                VacancyId = vacancy.Id
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult GetStatistics(int id, DateTime dateFrom, DateTime dateTo)
        {
            if (APIClient.User == null)
            {
                return Redirect("/Home/Enter");
            }

            var vacancy = APIClient.GetRequest<VacancyViewModel>($"/api/vacancy/details?id={id}");
            if (vacancy == null)
            {
                return NotFound("Вакансия не найдена.");
            }

            var reportFilePath = $"C:\\Users\\User\\source\\repos\\HRPro\\Статистика_по_вакансии_{vacancy.JobTitle}.pdf";

            APIClient.PostRequest("api/report/statistics", new ReportBindingModel
            {
                VacancyId = vacancy.Id,
                FileName = reportFilePath,
                DateFrom = dateFrom,
                DateTo = dateTo
            });

            if (!System.IO.File.Exists(reportFilePath))
            {
                return NotFound("Файл отчета не найден.");
            }

            return PhysicalFile(reportFilePath, "application/pdf", $"Статистика_по_вакансии_{vacancy.JobTitle}.pdf");
        }        

        [HttpGet]
        public async Task<ActionResult> VacancyDetails(int? id)
        {
            if (APIClient.User == null)
            {
                return Redirect("/Home/Enter");
            }
            VacancyViewModel vacancy;
            if (id.HasValue)
            {
                vacancy = APIClient.GetRequest<VacancyViewModel?>($"api/vacancy/details?id={id}");
                ViewBag.Resumes = APIClient.GetRequest<List<ResumeViewModel>?>($"api/vacancy/resumes?vacancyId={id}");
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "cities.json");
                var json = await System.IO.File.ReadAllTextAsync(filePath);
                var cities = JsonConvert.DeserializeObject<List<CityViewModel>>(json);
                ViewBag.Cities = cities;

                return View(vacancy);
            }
            return View();
        }

        [HttpGet]
        public IActionResult Vacancies(int? companyId)
        {
            if (APIClient.User == null)
            {
                return Redirect("/Home/Enter");
            }
            var vacancies = APIClient.GetRequest<List<VacancyViewModel>?>($"api/vacancy/list?companyId={companyId}");
            return View(vacancies);
        }

        [HttpGet]
        public IActionResult EditVacancy(int? id)
        {
            if (APIClient.User == null)
            {
                return Redirect("/Home/Enter");
            }
            if (!id.HasValue)
            {
                return View(new VacancyViewModel());
            }
            var model = APIClient.GetRequest<VacancyViewModel?>($"api/vacancy/details?id={id}");
            

            return View(model);
        }

        [HttpPost]
        public IActionResult EditVacancy(VacancyBindingModel model)
        {
            string redirectUrl = $"/Company/CompanyProfile/{APIClient.Company.Id}";
            try
            {
                if (APIClient.User == null)
                {
                    throw new Exception("Доступно только авторизованным пользователям");
                }

                if (APIClient.Company == null)
                {
                    throw new Exception("Компания не найдена");
                }

                if (string.IsNullOrEmpty(model.JobTitle))
                {
                    throw new ArgumentException("Нет названия вакансии");
                }

                if (string.IsNullOrEmpty(model.JobType.ToString()))
                {
                    throw new ArgumentException("Нет типа занятости");
                }

                if (string.IsNullOrEmpty(model.Status.ToString()))
                {
                    throw new ArgumentException("Нет статуса вакансии");
                }

                if (model.Id == 0)
                {
                    var existingVacancies = APIClient.GetRequest<List<VacancyViewModel>>($"api/vacancy/list?companyId={APIClient.Company.Id}");
                    var duplicate = existingVacancies?.FirstOrDefault(v =>
                        v.JobTitle.Equals(model.JobTitle, StringComparison.OrdinalIgnoreCase) &&
                        v.Salary == model.Salary);

                    if (duplicate != null)
                    {
                        throw new InvalidOperationException("Такая вакансия уже существует");
                    }
                }

                if (!string.IsNullOrEmpty(model.Tags))
                {
                    model.Tags = model.Tags.ToLowerInvariant();
                }
                if (model.Id != 0)
                {
                    APIClient.PostRequest("api/vacancy/update", model);
                }
                else
                {
                    model.CompanyId = APIClient.Company.Id;
                    APIClient.PostRequest("api/vacancy/create", model);
                    if (APIClient.Company != null)
                    {
                        if (!string.IsNullOrEmpty(model.Tags))
                        {
                            model.Tags = model.Tags.ToLowerInvariant();
                        }

                        APIClient.Company?.Vacancies.Add(new VacancyViewModel
                        {
                            Id = model.Id,
                            CompanyId = model.CompanyId,
                            CreatedAt = DateTime.Now.ToUniversalTime(),
                            Description = model.Description,
                            JobTitle = model.JobTitle,
                            JobType = model.JobType,
                            Salary = model.Salary,
                            Status = model.Status,
                            Tags = model.Tags
                        });
                    }
                }
                return Json(new { success = true, redirectUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult Delete(int id)
        {
            string returnUrl = HttpContext.Request.Headers["Referer"].ToString();
            try
            {
                if (APIClient.User == null)
                {
                    return Redirect("/Home/Enter");
                }
                if (APIClient.Company == null)
                {
                    throw new Exception("Компания не определена");
                }

                APIClient.PostRequest($"api/vacancy/delete", new VacancyBindingModel { Id = id });
                APIClient.Company = APIClient.GetRequest<CompanyViewModel?>($"api/company/profile?id={APIClient.User?.CompanyId}");

                return Redirect("~/Company/CompanyProfile");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { errorMessage = $"{ex.Message}", returnUrl });
            }
        }

        public IActionResult SearchVacancies(string? tags)
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
                    return View(new List<VacancyViewModel?>());
                }

                var results = APIClient.GetRequest<List<VacancyViewModel?>>($"api/vacancy/search?tags={tags}");
                return View(results);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { errorMessage = $"{ex.Message}", returnUrl });
            }
        }
    }
}
