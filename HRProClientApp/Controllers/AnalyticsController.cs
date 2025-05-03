using HRProClientApp.Models;
using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HRProClientApp.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly ILogger<AnalyticsController> _logger;
        public AnalyticsController(ILogger<AnalyticsController> logger)
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

            var reportFilePath = $"C:\\Users\\User\\source\\repos\\HRPro\\Резюме_{resume.CandidateInfo}.pdf";

            APIClient.PostRequest("api/report/resume", new ReportBindingModel
            {
                ResumeId = resume.Id,
                FileName = reportFilePath
            });

            if (!System.IO.File.Exists(reportFilePath))
            {
                return NotFound("Файл отчета не найден.");
            }

            return PhysicalFile(reportFilePath, "application/pdf", $"Резюме_{resume.CandidateInfo}.pdf");
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

        // новые методы
        [HttpGet]
        public IActionResult SalaryStatistics()
        {
            var model = APIClient.GetRequest<SalaryStatisticsViewModel>("api/report/GetSalaryStats");
            return View(model);
        }

        [HttpGet]
        public IActionResult VacancyStatusStatistics()
        {
            var model = APIClient.GetRequest<VacancyStatusStatisticsViewModel>("api/report/GetVacancyStatusStats");
            return View(model);
        }

        [HttpGet]
        public IActionResult ResumeStatistics()
        {
            var model = APIClient.GetRequest<ResumeStatisticsViewModel>("api/report/GetResumeStats");
            return View(model);
        }
    }
}
