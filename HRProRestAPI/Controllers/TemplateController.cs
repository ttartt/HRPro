using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HRProRestAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TemplateController : Controller
    {
        private readonly ILogger _logger;
        private readonly ITemplateLogic _logic;
        public TemplateController(ITemplateLogic logic, ILogger<TemplateController> logger)
        {
            _logger = logger;
            _logic = logic;
        }

        [HttpGet]
        public TemplateViewModel? Details(int id)
        {
            try
            {
                return _logic.ReadElement(new TemplateSearchModel
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
        public List<TemplateViewModel>? Search(string? tags)
        {
            try
            {
                if (!string.IsNullOrEmpty(tags))
                {
                    return _logic.ReadList(new TemplateSearchModel
                    {
                        Name = tags
                    });
                }
                return new List<TemplateViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения вакансий");
                throw;
            }
        }

        [HttpGet]
        public List<TemplateViewModel>? List()
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
        public void Create(TemplateBindingModel model)
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
        public void Update(TemplateBindingModel model)
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
        public void Delete(TemplateBindingModel model)
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
