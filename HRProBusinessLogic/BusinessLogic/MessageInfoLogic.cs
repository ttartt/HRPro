using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;
using Microsoft.Extensions.Logging;

namespace HRProBusinessLogic.BusinessLogic
{
    public class MessageInfoLogic : IMessageInfoLogic
    {
        private readonly ILogger _logger;
        private readonly IMessageInfoStorage _messageStorage;

        public MessageInfoLogic(ILogger<MessageInfoLogic> logger, IMessageInfoStorage messageStorage)
        {
            _logger = logger;
            _messageStorage = messageStorage;
        }

        public bool Create(MessageInfoBindingModel model)
        {
            if (_messageStorage.Insert(model) == null)
            {
                _logger.LogWarning("Insert operation failed");
                return false;
            }
            return true;
        }

        public List<MessageInfoViewModel>? ReadList(MessageInfoSearchModel? model)
        {
            _logger.LogInformation("ReadList. MessageId:{MessageId}.UserId:{UserId} ", model?.MessageId, model?.UserId);
            var list = model == null ? _messageStorage.GetFullList() : _messageStorage.GetFilteredList(model);

            if (list == null)
            {
                _logger.LogWarning("ReadList return null list");
                return null;
            }

            _logger.LogInformation("ReadList. Count: {Count}", list.Count);
            return list;
        }
    }
}
