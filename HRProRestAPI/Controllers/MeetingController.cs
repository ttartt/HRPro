using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HRProRestAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]        
    public class MeetingController : Controller
    {
        private readonly ILogger _logger;
        private readonly IMeetingLogic _logic;
        private readonly IMeetingParticipantLogic _logicMP;
        public MeetingController(IMeetingLogic logic, ILogger<MeetingController> logger, IMeetingParticipantLogic logicMP)
        {
            _logger = logger;
            _logic = logic;
            _logicMP = logicMP;
        }

        [HttpGet]
        public MeetingViewModel? Details(int id)
        {
            try
            {
                return _logic.ReadElement(new MeetingSearchModel
                {
                    Id = id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения собеседования");
                throw;
            }
        }

        [HttpGet]
        public List<MeetingViewModel>? ListByUserId(int? userId)
        {
            try
            {
                List<MeetingViewModel> result = new(); 
                if (userId.HasValue)
                {
                    var list = _logicMP.ReadList(new MeetingParticipantSearchModel
                    {
                        UserId = userId
                    });
                    foreach (var item in list)
                    {
                        var found = _logic.ReadElement(new MeetingSearchModel { Id = item.MeetingId });
                        if (found != null)
                        {
                            result.Add(found);
                        }                        
                    }
                    return result;
                }
                else return new List<MeetingViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения собеседований");
                throw;
            }
        }

        [HttpPost]
        public void Create(MeetingBindingModel model)
        {
            try
            {
                _logic.Create(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка создания собеседования");
                throw;
            }
        }

        [HttpPost]
        public void CreateParticipant(MeetingParticipantBindingModel model)
        {
            try
            {
                _logicMP.Create(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка создания собеседования");
                throw;
            }
        }

        [HttpPost]
        public void Update(MeetingBindingModel model)
        {
            try
            {
                _logic.Update(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обновления собеседования");
                throw;
            }
        }

        [HttpPost]
        public void Delete(MeetingBindingModel model)
        {
            try
            {
                _logic.Delete(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка удаления собеседования");
                throw;
            }
        }

        [HttpPost]
        public void DeleteParticipant(MeetingParticipantBindingModel model)
        {
            try
            {
                _logicMP.Delete(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка удаления собеседования");
                throw;
            }
        }
    }
}
