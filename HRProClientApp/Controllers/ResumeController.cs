using HRProClientApp.Models;
using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult GetReport(int id)
        {
            if (APIClient.User == null)
            {
                return Redirect("~Home/Enter");
            }

            var resume = APIClient.GetRequest<ResumeViewModel>($"/api/resume/details?id={id}");
            if (resume == null)
            {
                return NotFound("Резюме не найдено.");
            }

            var reportFilePath = $"C:\\Users\\User\\source\\repos\\HRPro\\Резюме_{resume.UserName}.pdf";

            APIClient.PostRequest("api/report/resume", new ReportBindingModel
            {
                ResumeId = resume.Id,
                FileName = reportFilePath
            });

            if (!System.IO.File.Exists(reportFilePath))
            {
                return NotFound("Файл отчета не найден.");
            }

            return PhysicalFile(reportFilePath, "application/pdf", $"Резюме_{resume.UserName}.pdf");
        }

        [HttpGet]
        public async Task<IActionResult> ResumeDetails(int? id)
        {
            if (APIClient.User == null)
            {
                return Redirect("~/Home/Enter");
            }

            var resume = await APIClient.GetRequestAsync<ResumeViewModel?>($"api/resume/details?id={id}");
            if (resume == null || !id.HasValue)
            {
                return View();
            }           

            return View(resume);
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
                model = new ResumeViewModel { UserId = APIClient.User.Id, VacancyId = vacancyId ?? 0 };
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
                    if (isDraft)
                    {
                        model.Status = HRProDataModels.Enums.ResumeStatusEnum.Черновик;
                    }
                    else
                    {
                        model.Status = HRProDataModels.Enums.ResumeStatusEnum.Обрабатывается;
                    }
                    APIClient.PostRequest("api/resume/update", model);
                }
                else
                {
                    model.UserId = APIClient.User.Id;
                    if (isDraft)
                    {
                        model.Status = HRProDataModels.Enums.ResumeStatusEnum.Черновик;
                    } 
                    else
                    {
                        model.Status = HRProDataModels.Enums.ResumeStatusEnum.Обрабатывается;
                    }
                    var vacancy = APIClient.GetRequest<VacancyViewModel>($"api/vacancy/details?id={model.VacancyId}");
                    var resume = APIClient.GetRequest<ResumeViewModel>($"api/resume/check?userId={model.UserId}&vacancyId={model.VacancyId}");
                    if (resume == null)
                    {
                        APIClient.PostRequest("api/resume/create", model);
                        if (APIClient.User != null)
                        {
                            APIClient.User?.Resumes.Add(new ResumeViewModel
                            {
                                Id = model.Id,
                                Description = model.Description,
                                Education = model.Education,
                                Experience = model.Experience,
                                Skills = model.Skills,
                                Status = model.Status,
                                Title = model.Title,
                                UserId = model.UserId,
                                VacancyId = model.VacancyId
                            });
                        }
                        else
                        {
                            throw new Exception("Пользователь не найден");
                        }
                        if (vacancy != null && model.Status != HRProDataModels.Enums.ResumeStatusEnum.Черновик)
                        {
                            vacancy.Resumes.Add(new ResumeViewModel
                            {
                                Id = model.Id,
                                Description = model.Description,
                                Education = model.Education,
                                Experience = model.Experience,
                                Skills = model.Skills,
                                Status = model.Status,
                                Title = model.Title,
                                UserId = model.UserId,
                                VacancyId = model.VacancyId
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
                return Redirect($"~/User/UserProfile/{model.UserId}");
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
                APIClient.PostRequest($"api/resume/delete", new ResumeBindingModel { Id = id });
                return Redirect($"~/User/UserProfile/{APIClient.User?.Id}");
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
