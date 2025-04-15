using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;
using HRproDatabaseImplement;
using HRProDatabaseImplement.Models;

namespace HRProDatabaseImplement.Implements
{
    public class MessageInfoStorage : IMessageInfoStorage
    {
        public MessageInfoViewModel? GetElement(MessageInfoSearchModel model)
        {
            if (model.MessageId == null)
                return null;
            using var context = new HRproDatabase();
            return context.Messages.FirstOrDefault(x => x.MessageId == model.MessageId)?.GetViewModel;
        }

        public List<MessageInfoViewModel> GetFilteredList(MessageInfoSearchModel model)
        {
            if (!model.UserId.HasValue)
                return new();
            using var context = new HRproDatabase();
            return context.Messages
                .Where(x => x.UserId == model.UserId)
                .Select(x => x.GetViewModel)
                .ToList();
        }

        public List<MessageInfoViewModel> GetFullList()
        {
            using var context = new HRproDatabase();
            return context.Messages
                .Select(x => x.GetViewModel)
                .ToList();
        }

        public MessageInfoViewModel? Insert(MessageInfoBindingModel model)
        {
            var newMessage = Message.Create(model);
            if (newMessage == null)
            {
                return null;
            }
            using var context = new HRproDatabase();
            context.Messages.Add(newMessage);
            context.SaveChanges();
            return newMessage.GetViewModel;
        }
    }
}
