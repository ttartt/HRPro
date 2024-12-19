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
    public class MeetingParticipantLogic : IMeetingParticipantLogic
    {
        private readonly ILogger _logger;
        private readonly IMeetingParticipantStorage _meetingParticipantStorage;
        public MeetingParticipantLogic(ILogger<MeetingParticipantLogic> logger, IMeetingParticipantStorage meetingParticipantStorage)
        {
            _logger = logger;
            _meetingParticipantStorage = meetingParticipantStorage;
        }
        public bool Create(MeetingParticipantBindingModel model)
        {
            CheckModel(model);
            if (_meetingParticipantStorage.Insert(model) == null)
            {
                _logger.LogWarning("Insert operation failed");
                return false;
            }
            return true;
        }

        public bool Delete(MeetingParticipantBindingModel model)
        {
            CheckModel(model, false);
            _logger.LogInformation("Delete. Id: {Id}", model.Id);
            if (_meetingParticipantStorage.Delete(model) == null)
            {
                _logger.LogWarning("Delete operation failed");
                return false;
            }
            return true;
        }

        public MeetingParticipantViewModel? ReadElement(MeetingParticipantSearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var element = _meetingParticipantStorage.GetElement(model);
            if (element == null)
            {
                _logger.LogWarning("ReadElement element not found");
                return null;
            }
            _logger.LogInformation("ReadElement find. Id: {Id}", element.Id);
            return element;
        }

        public List<MeetingParticipantViewModel>? ReadList(MeetingParticipantSearchModel? model)
        {
            var list = model == null ? _meetingParticipantStorage.GetFullList() : _meetingParticipantStorage.GetFilteredList(model);
            if (list == null)
            {
                _logger.LogWarning("ReadList return null list");
                return null;
            }
            _logger.LogInformation("ReadList. Count: {Count}", list.Count);
            return list;
        }

        public bool Update(MeetingParticipantBindingModel model)
        {
            CheckModel(model);
            if (_meetingParticipantStorage.Update(model) == null)
            {
                _logger.LogWarning("Update operation failed");
                return false;
            }
            return true;
        }

        private void CheckModel(MeetingParticipantBindingModel model, bool withParams = true)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!withParams)
            {
                return;
            }

            if (model.MeetingId <= 0)
            {
                throw new ArgumentException("Нет идентификатора встречи", nameof(model.MeetingId));
            }

            if (model.UserId <= 0)
            {
                throw new ArgumentException("Нет идентификатора пользователя", nameof(model.UserId));
            }

            var element = _meetingParticipantStorage.GetElement(new MeetingParticipantSearchModel
            {
                MeetingId = model.MeetingId,
                UserId = model.UserId
            });

            if (element != null && element.Id != model.Id)
            {
                throw new InvalidOperationException("Этот пользователь уже является участником");
            }
        }
    }
}
