using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRProBusinessLogic.BusinessLogic
{
    public class MeetingLogic : IMeetingLogic
    {
        private readonly ILogger _logger;
        private readonly IMeetingStorage _meetingStorage;
        public MeetingLogic(ILogger<MeetingLogic> logger, IMeetingStorage meetingStorage)
        {
            _logger = logger;
            _meetingStorage = meetingStorage;
        }
        public bool Create(MeetingBindingModel model)
        {
            CheckModel(model);
            if (_meetingStorage.Insert(model) == null)
            {
                _logger.LogWarning("Insert operation failed");
                return false;
            }
            return true;
        }

        public bool Delete(MeetingBindingModel model)
        {
            CheckModel(model, false);
            _logger.LogInformation("Delete. Id: {Id}", model.Id);
            if (_meetingStorage.Delete(model) == null)
            {
                _logger.LogWarning("Delete operation failed");
                return false;
            }
            return true;
        }

        public MeetingViewModel? ReadElement(MeetingSearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var element = _meetingStorage.GetElement(model);
            if (element == null)
            {
                _logger.LogWarning("ReadElement element not found");
                return null;
            }
            _logger.LogInformation("ReadElement find. Id: {Id}", element.Id);
            return element;
        }

        public List<MeetingViewModel>? ReadList(MeetingSearchModel? model)
        {
            var list = model == null ? _meetingStorage.GetFullList() : _meetingStorage.GetFilteredList(model);
            if (list == null)
            {
                _logger.LogWarning("ReadList return null list");
                return null;
            }
            _logger.LogInformation("ReadList. Count: {Count}", list.Count);
            return list;
        }

        public bool Update(MeetingBindingModel model)
        {
            CheckModel(model);
            if (_meetingStorage.Update(model) == null)
            {
                _logger.LogWarning("Update operation failed");
                return false;
            }
            return true;
        }

        private void CheckModel(MeetingBindingModel model, bool withParams = true)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!withParams)
            {
                return;
            }            

            if (string.IsNullOrEmpty(model.Topic))
            {
                throw new ArgumentNullException("Тема встречи не может быть пустой", nameof(model.Topic));
            }

            if (model.TimeFrom >= model.TimeTo)
            {
                throw new ArgumentException("Время начала должно быть меньше времени окончания", nameof(model.TimeFrom));
            }


            var existingMeeting = _meetingStorage.GetElement(new MeetingSearchModel
            {
                Date = model.Date.Date,
                TimeFrom = model.TimeFrom,
                TimeTo = model.TimeTo
            });

            if (existingMeeting != null && existingMeeting.Id != model.Id)
            {
                throw new InvalidOperationException("Встреча уже существует");
            }
        }
    }
}
