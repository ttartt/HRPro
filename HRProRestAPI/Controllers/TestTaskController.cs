using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HRProRestAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestTaskController : Controller
    {
        private readonly ILogger _logger;
        private readonly ITestTaskLogic _logic;
        public TestTaskController(ITestTaskLogic logic, ILogger<TestTaskController> logger)
        {
            _logger = logger;
            _logic = logic;
        }

        [HttpGet]
        public TestTaskViewModel? Details(int id)
        {
            try
            {
                return _logic.ReadElement(new TestTaskSearchModel
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
        public List<TestTaskViewModel>? List()
        {
            try
            {
                var list = _logic.ReadList(null);
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения вакансий");
                throw;
            }
        }

        [HttpPost]
        public void Create(TestTaskBindingModel model)
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
        public void Update(TestTaskBindingModel model)
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
        public void Delete(TestTaskBindingModel model)
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
