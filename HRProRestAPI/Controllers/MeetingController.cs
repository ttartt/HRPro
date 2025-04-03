using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

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
        public List<MeetingViewModel>? List(int? userId, int? companyId)
        {
            try
            {
                List<MeetingViewModel> result = new();
                if (companyId.HasValue && userId.HasValue)
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
                    result.AddRange(_logic.ReadList(new MeetingSearchModel
                    {
                        CompanyId = companyId
                    }));
                    return result;
                }
                else if (companyId.HasValue)
                {
                    return _logic.ReadList(new MeetingSearchModel
                    {
                        CompanyId = companyId
                    });
                }
                else if (userId.HasValue)
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
                else return _logic.ReadList(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения собеседований");
                throw;
            }
        }


        [HttpGet]
        public List<MeetingParticipantViewModel>? Participants(int? meetingId)
        {
            try
            {
                var list = _logicMP.ReadList(new MeetingParticipantSearchModel { MeetingId = meetingId });
                return list;
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
