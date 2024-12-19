using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HRProRestAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ResponsibilityController : Controller
    {
        private readonly ILogger _logger;
        private readonly IResponsibilityLogic _logic;
        private readonly IVacancyResponsibilityLogic _logicVR;
        public ResponsibilityController(IResponsibilityLogic logic, ILogger<ResponsibilityController> logger, IVacancyResponsibilityLogic logicVR)
        {
            _logger = logger;
            _logic = logic;
            _logicVR = logicVR;
        }

        [HttpGet]
        public List<ResponsibilityViewModel>? List()
        {
            try
            {
                var list = _logic.ReadList(null);
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения оценок");
                throw;
            }
        }

        [HttpGet]
        public ResponsibilityViewModel? Details(int id)
        {
            try
            {
                return _logic.ReadElement(new ResponsibilitySearchModel
                {
                    Id = id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения оценки");
                throw;
            }
        }

        [HttpGet]
        public List<VacancyResponsibilityViewModel>? ListByVacancyId(int id)
        {
            try
            {
                return _logicVR.ReadList(new VacancyResponsibilitySearchModel
                {
                    VacancyId = id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения оценок");
                throw;
            }
        }

        [HttpGet]
        public List<VacancyResponsibilityViewModel>? VacancyResponsibilities(int assessmentId)
        {
            try
            {
                return _logicVR.ReadList(new VacancyResponsibilitySearchModel
                {
                    ResponsibilityId = assessmentId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения оценок");
                throw;
            }
        }

        [HttpPost]
        public IActionResult Create(ResponsibilityBindingModel model)
        {
            try
            {
                int? id = _logic.Create(model);
                return Ok(new ResponsibilityBindingModel { Id = (int)id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка создания оценки");
                throw;
            }
        }

        [HttpPost]
        public void CreateVR(VacancyResponsibilityBindingModel model)
        {
            try
            {
                _logicVR.Create(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка создания оценки");
                throw;
            }
        }

        [HttpPost]
        public void Update(ResponsibilityBindingModel model)
        {
            try
            {
                _logic.Update(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обновления оценки");
                throw;
            }
        }

        [HttpPost]
        public void Delete(ResponsibilityBindingModel model)
        {
            try
            {
                _logic.Delete(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка удаления оценки");
                throw;
            }
        }
    }
}
