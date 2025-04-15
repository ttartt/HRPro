using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRProRestApi.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VacancyController : Controller
    {
        private readonly ILogger _logger;
        private readonly IVacancyLogic _logic;
        private readonly IResumeLogic _resumeLogic;
        public VacancyController(IVacancyLogic logic, ILogger<VacancyController> logger, IResumeLogic resumeLogic)
        {
            _logger = logger;
            _logic = logic;
            _resumeLogic = resumeLogic;
        }

        [HttpGet]
        public VacancyViewModel? Details(int id)
        {
            try
            {
                return _logic.ReadElement(new VacancySearchModel
                {
                    Id = id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения вакансии");
                throw;
            }
        }

        [HttpGet]
        public List<VacancyViewModel>? List(int? companyId)
        {
            try
            {
                if (companyId.HasValue)
                {
                    return _logic.ReadList(new VacancySearchModel
                    {
                        CompanyId = companyId
                    });
                }
                else return new List<VacancyViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения вакансий");
                throw;
            }
        }

        [HttpGet]
        public List<ResumeViewModel>? Resumes(int? vacancyId)
        {
            try
            {
                if (vacancyId.HasValue)
                {
                    return _resumeLogic.ReadList(new ResumeSearchModel
                    {
                        VacancyId = vacancyId
                    });
                }
                else return new List<ResumeViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения резюме на вакансию");
                throw;
            }
        }


        [HttpPost]
        public void Create(VacancyBindingModel model)
        {
            try
            {
                _logic.Create(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка создания вакансии");
                throw;
            }
        }

        [HttpPost]
        public void Update(VacancyBindingModel model)
        {
            try
            {
                _logic.Update(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обновления вакансии");
                throw;
            }
        }

        [HttpPost]
        public void Delete(VacancyBindingModel model)
        {
            try
            {
                _logic.Delete(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка удаления вакансии");
                throw;
            }
        }
    }
}
