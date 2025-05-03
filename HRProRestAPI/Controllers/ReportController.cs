using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRProRestApi.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReportController : Controller
    {
        private readonly IReportLogic _reportLogic;
        private readonly ILogger _logger;
        public ReportController(IReportLogic reportLogic, ILogger<ReportController> logger)
        {
            _reportLogic = reportLogic;
            _logger = logger;
        }

        [HttpPost]
        public void Resume(ReportBindingModel model)
        {
            try
            {
                _reportLogic.SaveResumeToPdf(model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public void Statistics(ReportBindingModel model)
        {
            try
            {
                _reportLogic.SaveResumesStatisticsToPdf(model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // новые методы
        [HttpGet]
        public SalaryStatisticsViewModel? GetSalaryStats()
        {
            try
            {
                var stats = _reportLogic.GetSalaryStatistics();
                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения статистики по зарплатам вакансий");
                throw;
            }
        }

        [HttpGet]
        public VacancyStatusStatisticsViewModel GetVacancyStatusStats()
        {
            try
            {
                var result = _reportLogic.GetVacancyStatusStatistics();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения статистики по статусам вакансий");
                throw;
            }
        }

        [HttpGet]
        public ResumeStatisticsViewModel GetResumeStats()
        {
            try
            {
                var result = _reportLogic.GetResumeStatistics();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения статистики по резюме");
                throw;
            }
        }
    }
}
