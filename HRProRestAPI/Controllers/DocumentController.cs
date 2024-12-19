using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HRProRestAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DocumentController : Controller
    {
        private readonly ILogger _logger;
        private readonly IDocumentLogic _logic;
        public DocumentController(IDocumentLogic logic, ILogger<DocumentController> logger)
        {
            _logger = logger;
            _logic = logic;
        }

        [HttpGet]
        public DocumentViewModel? Details(int id)
        {
            try
            {
                return _logic.ReadElement(new DocumentSearchModel
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
        public List<DocumentViewModel>? Search(string? tags)
        {
            try
            {
                if (!string.IsNullOrEmpty(tags))
                {
                    return _logic.ReadList(new DocumentSearchModel
                    {
                        Name = tags
                    });
                }
                return new List<DocumentViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения вакансий");
                throw;
            }
        }

        [HttpGet]
        public List<DocumentViewModel>? List(int? userId)
        {
            try
            {
                if (userId.HasValue)
                {
                    return _logic.ReadList(new DocumentSearchModel
                    {
                        CreatorId = userId
                    });
                }
                else return new List<DocumentViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения вакансий");
                throw;
            }
        }

        [HttpPost]
        public void Create(DocumentBindingModel model)
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
        public void Update(DocumentBindingModel model)
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
        public void Delete(DocumentBindingModel model)
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