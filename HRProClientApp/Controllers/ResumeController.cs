using HRProClientApp.Models;
using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace HRProClientApp.Controllers
{
    public class ResumeController : Controller
    {
        private readonly ILogger<ResumeController> _logger;

        public ResumeController(ILogger<ResumeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Resumes()
        {
            if (APIClient.User == null)
            {
                return Redirect("~/Home/Enter");
            }
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "cities.json");
            var json = await System.IO.File.ReadAllTextAsync(filePath);
            var cities = JsonConvert.DeserializeObject<List<CityViewModel>>(json);
            ViewBag.Cities = cities;

            var list = APIClient.GetRequest<List<ResumeViewModel>?>($"api/resume/list?companyId={APIClient.Company?.Id}");
            if (list == null)
            {
                return View();
            }

            return View(list);
        }

        [HttpGet]
        public IActionResult ResumeDetails(int? id)
        {
            if (APIClient.User == null)
            {
                return Redirect("~/Home/Enter");
            }

            var resume = APIClient.GetRequest<ResumeViewModel?>($"api/resume/details?id={id}");
            if (resume == null || !id.HasValue)
            {
                return View();
            }

            return View(resume);
        }

        [HttpGet]
        public IActionResult CollectResume(string? cityName)
        {
            string redirectUrl = "/Resume/Resumes";
            try
            {
                if (APIClient.User == null)
                    throw new Exception("Доступно только авторизованным пользователям");

                if (APIClient.Company == null)
                    throw new Exception("Компания не найдена");
                var apiResponse = APIClient.GetRequest<ApiResponse<List<ResumeViewModel>>>(
                $"api/parser/parse?cityName={cityName}");

                if (apiResponse == null)
                    return Json(new { success = false, message = "Не получен ответ от API" });

                if (!apiResponse.Success)
                    return Json(new { success = false, message = apiResponse.Message ?? "Ошибка API" });

                foreach (var resume in apiResponse.Data)
                {
                    resume.CompanyId = APIClient.Company.Id;
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
        public async Task<IActionResult> EditResume(int? id, int? vacancyId)
        {
            if (APIClient.User == null)
            {
                return RedirectToAction("Enter", "Home");
            }

            ResumeViewModel model;
            if (id.HasValue)
            {
                model = await APIClient.GetRequestAsync<ResumeViewModel>($"api/resume/details?id={id}");
                if (model == null)
                {
                    return NotFound();
                }
            }
            else
            {
                model = new ResumeViewModel { VacancyId = vacancyId ?? 0 };
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult EditResume(ResumeBindingModel model, bool isDraft)
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
                    APIClient.PostRequest("api/resume/update", model);
                }
                else
                {                    
                    var vacancy = APIClient.GetRequest<VacancyViewModel>($"api/vacancy/details?id={model.VacancyId}");
                    var resume = APIClient.GetRequest<ResumeViewModel>($"api/resume/check?vacancyId={model.VacancyId}");
                    if (resume == null)
                    {
                        APIClient.PostRequest("api/resume/create", model);
                        if (vacancy != null)
                        {
                            vacancy.Resumes.Add(new ResumeViewModel
                            {
                                Id = model.Id,
                                Description = model.Description,
                                Education = model.Education,
                                Experience = model.Experience,
                                Skills = model.Skills,
                                Title = model.Title,
                                VacancyId = model.VacancyId,
                                CompanyId = model.CompanyId,
                                CandidateInfo = model.CandidateInfo,
                                City = model.City,
                                CreatedAt = model.CreatedAt,
                                Salary = model.Salary,
                                Url = model.Url
                            });
                        }
                        else
                        {
                            throw new Exception("Вакансия не найдена");
                        }
                    }
                    else
                    {
                        throw new Exception("Вы уже создавали резюме на эту вакансию!");
                    }
                }
                return Redirect($"~/User/UserProfile/{APIClient.User.Id}");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { errorMessage = $"{ex.Message}", returnUrl });
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

                APIClient.PostRequest($"api/resume/delete", new ResumeBindingModel { Id = id });

                // Для AJAX-запросов возвращаем JSON, для обычных - редирект
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Redirect("~/Resume/Resumes");
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}