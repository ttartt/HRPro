using DocumentFormat.OpenXml.Office2010.Excel;
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
        public IActionResult CollectResume(string? cityName, int? vacancyId, string? tags)
        {
            string redirectUrl = $"/Vacancy/VacancyDetails/{vacancyId}";
            try
            {
                if (APIClient.User == null)
                    throw new Exception("Доступно только авторизованным пользователям");

                if (APIClient.Company == null)
                    throw new Exception("Компания не найдена");
                
                var apiResponse = APIClient.GetRequest<ApiResponse<List<ResumeViewModel>>>(
                $"api/parser/parseForVacancy?cityName={cityName}&tags={tags}");

                if (apiResponse == null)
                    return Json(new { success = false, message = "Не получен ответ от API" });

                if (!apiResponse.Success)
                    return Json(new { success = false, message = apiResponse.Message ?? "Ошибка API" });

                foreach (var resume in apiResponse.Data)
                {
                    resume.CompanyId = APIClient.Company.Id;
                    resume.VacancyId = vacancyId;
                    resume.Source = HRProDataModels.Enums.ResumeSourceEnum.Avito;
                    APIClient.PostRequest("api/resume/create", resume);
                }
                return Json(new
                {
                    success = true,
                    message = apiResponse.Data?.Count > 0
                        ? $"Успешно собрано {apiResponse.Data.Count} резюме"
                        : "Новые резюме не найдены",
                    redirectUrl,
                    resumes = apiResponse.Data
                });   
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
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
        public IActionResult EditVacancy(VacancyBindingModel model, string redirectUrl)
        {
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
            try
            {
                if (APIClient.User == null)
                {
                    return Json(new { success = false, message = "Доступно только авторизованным пользователям" });
                }

                if (APIClient.Company == null)
                {
                    return Json(new { success = false, message = "Компания не определена" });
                }

                APIClient.PostRequest($"api/vacancy/delete", new VacancyBindingModel { Id = id });
                APIClient.Company = APIClient.GetRequest<CompanyViewModel?>($"api/company/profile?id={APIClient.User?.CompanyId}");

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
