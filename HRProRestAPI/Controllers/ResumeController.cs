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
    public class ResumeController : Controller
    {
        private readonly ILogger _logger;
        private readonly IResumeLogic _logic;
        public ResumeController(IResumeLogic logic, ILogger<ResumeController> logger)
        {
            _logger = logger;
            _logic = logic;
        }

        [HttpGet]
        public ResumeViewModel? Details(int id)
        {
            try
            {
                return _logic.ReadElement(new ResumeSearchModel
                {
                    Id = id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения резюме");
                throw;
            }
        }

        [HttpGet]
        public List<ResumeViewModel>? List(int? companyId)
        {
            try
            {
                if (companyId.HasValue)
                {
                    return _logic.ReadList(new ResumeSearchModel
                    {
                        CompanyId = companyId
                    });
                }
                else return _logic.ReadList(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения резюме");
                throw;
            }
        }

        [HttpGet]
        public ResumeViewModel? Check(int userId, int vacancyId)
        {
            try
            {
                return _logic.ReadElement(new ResumeSearchModel
                {
                    UserId = userId,
                    VacancyId = vacancyId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения резюме");
                throw;
            }
        }

        [HttpPost]
        public void Create(ResumeBindingModel model)
        {
            try
            {
                _logic.Create(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка создания резюме");
                throw;
            }
        }

        [HttpPost]
        public void Update(ResumeBindingModel model)
        {
            try
            {
                _logic.Update(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обновления резюме");
                throw;
            }
        }

        [HttpPost]
        public void Delete(ResumeBindingModel model)
        {
            try
            {
                _logic.Delete(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка удаления резюме");
                throw;
            }
        }
    }    
}
