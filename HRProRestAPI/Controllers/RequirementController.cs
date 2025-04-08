using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRProRestAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RequirementController : Controller
    {
        private readonly ILogger _logger;
        private readonly IRequirementLogic _logic;
        private readonly IVacancyRequirementLogic _logicVR;
        public RequirementController(IRequirementLogic logic, ILogger<RequirementController> logger, IVacancyRequirementLogic logicVR)
        {
            _logger = logger;
            _logic = logic;
            _logicVR = logicVR;
        }

        [HttpGet]
        public List<VacancyRequirementViewModel>? ListByVacancyId(int id)
        {
            try
            {
                return _logicVR.ReadList(new VacancyRequirementSearchModel
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
        public List<RequirementViewModel>? List()
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
        public List<VacancyRequirementViewModel>? VacancyRequirements(int assessmentId)
        {
            try
            {
                return _logicVR.ReadList(new VacancyRequirementSearchModel
                {
                    RequirementId = assessmentId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения оценок");
                throw;
            }
        }

        [HttpPost]
        public IActionResult Create(RequirementBindingModel model)
        {
            try
            {
                int? id = _logic.Create(model);
                return Ok(new RequirementBindingModel { Id = (int)id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка создания оценки");
                throw;
            }
        }

        [HttpPost]
        public void CreateVR(VacancyRequirementBindingModel model)
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
        public void Update(RequirementBindingModel model)
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
        public void Delete(RequirementBindingModel model)
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
