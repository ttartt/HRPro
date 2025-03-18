using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HRProRestApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CandidateController : Controller
    {
        private readonly ILogger _logger;
        private readonly ICandidateLogic _logic;
        public CandidateController(ICandidateLogic logic, ILogger<CandidateController> logger)
        {
            _logger = logger;
            _logic = logic;
        }

        [HttpGet]
        public CandidateViewModel? Details(int id)
        {
            try
            {
                return _logic.ReadElement(new CandidateSearchModel
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
        public List<CandidateViewModel>? List()
        {
            try
            {
                return _logic.ReadList(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения вакансий");
                throw;
            }
        }

        [HttpPost]
        public void Create(CandidateBindingModel model)
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
        public void Update(CandidateBindingModel model)
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
        public void Delete(CandidateBindingModel model)
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
