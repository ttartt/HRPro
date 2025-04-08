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
    public class CompanyController : Controller
    {
        private readonly ILogger _logger;
        private readonly ICompanyLogic _logic;
        private readonly IVacancyLogic _vacancyLogic;
        public CompanyController(ICompanyLogic logic, IVacancyLogic vacancyLogic, ILogger<CompanyController> logger)
        {
            _logger = logger;
            _logic = logic;
            _vacancyLogic = vacancyLogic;
        }

        [HttpGet]
        public CompanyViewModel? Profile(int id)
        {
            try
            {
                return _logic.ReadElement(new CompanySearchModel
                {
                    Id = id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения профиля компании");
                throw;
            }
        }

        [HttpGet]
        public CompanyViewModel? DefineByName(string name)
        {
            try
            {
                return _logic.ReadElement(new CompanySearchModel
                {
                    Name = name
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка определения компании");
                throw;
            }
        }

        [HttpPost]
        public IActionResult Create(CompanyBindingModel model)
        {
            try
            {
                int? id = _logic.Create(model);
                return Ok(new CompanyBindingModel { Id = (int)id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка создания компании");
                throw;
            }
        }

        [HttpPost]
        public void Update(CompanyBindingModel model)
        {
            try
            {
                _logic.Update(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обновления профиля компании");
                throw;
            }
        }

        [HttpDelete]
        public void Delete(CompanyBindingModel model)
        {
            try
            {
                _logic.Delete(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка удаления профиля компании");
                throw;
            }
        }
    }
}
