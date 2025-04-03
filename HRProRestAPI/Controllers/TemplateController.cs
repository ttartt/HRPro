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
        private readonly ITagLogic _tagLogic;
        public TemplateController(ITemplateLogic logic, ITagLogic tagLogic, ILogger<TemplateController> logger)
        {
            _logger = logger;
            _logic = logic;
            _tagLogic = tagLogic;
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
                _logger.LogError(ex, "Ошибка получения шаблона");
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
                _logger.LogError(ex, "Ошибка получения шаблонов");
                throw;
            }
        }

        [HttpGet]
        public List<TagViewModel>? Tags(int? templateId)
        {
            try
            {
                var list = _tagLogic.ReadList(new TagSearchModel { TemplateId = templateId });
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения тэгов шаблона");
                throw;
            }
        }

        [HttpGet]
        public List<TemplateViewModel>? List(int? companyId)
        {
            try
            {
                if (companyId.HasValue)
                {
                    return _logic.ReadList(new TemplateSearchModel
                    {
                        CompanyId = companyId
                    });
                }
                else return _logic.ReadList(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения шаблонов");
                throw;
            }
        }

        [HttpPost]
        public IActionResult Create(TemplateBindingModel model)
        {
            try
            {
                int? id = _logic.Create(model);
                return Ok(new TemplateBindingModel { Id = (int)id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка создания шаблона");
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
                _logger.LogError(ex, "Ошибка обновления шаблона");
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
                _logger.LogError(ex, "Ошибка удаления шаблона");
                throw;
            }
        }
    }
}
