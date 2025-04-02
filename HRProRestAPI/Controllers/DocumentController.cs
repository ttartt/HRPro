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
        private readonly IDocumentTagLogic _documentTagLogic;
        public DocumentController(IDocumentLogic logic, IDocumentTagLogic documentTagLogic, ILogger<DocumentController> logger)
        {
            _logger = logger;
            _logic = logic;
            _documentTagLogic = documentTagLogic;
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
                _logger.LogError(ex, "Ошибка получения документа");
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
                _logger.LogError(ex, "Ошибка получения документов");
                throw;
            }
        }

        [HttpGet]
        public List<DocumentViewModel>? List(int? userId, int? companyId)
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
                else if (companyId.HasValue)
                {
                    return _logic.ReadList(new DocumentSearchModel
                    {
                        CompanyId = companyId
                    });
                }
                else if (userId.HasValue && companyId.HasValue)
                {
                    return _logic.ReadList(new DocumentSearchModel
                    {
                        CompanyId = companyId,
                        CreatorId = userId
                    });
                }
                else return _logic.ReadList(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения документов");
                throw;
            }
        }

        [HttpPost]
        public IActionResult Create(DocumentBindingModel model)
        {
            try
            {
                int? id = _logic.Create(model);
                return Ok(new DocumentBindingModel { Id = (int)id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка создания документа");
                throw;
            }
        }

        [HttpPost]
        public void CreateTag(DocumentTagBindingModel model)
        {
            try
            {
                _documentTagLogic.Create(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка создания тега документа");
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
                _logger.LogError(ex, "Ошибка обновления документа");
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
                _logger.LogError(ex, "Ошибка удаления документа");
                throw;
            }
        }
    }
}