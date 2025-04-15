using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;

namespace HRProContracts.StoragesContracts
{
    public interface IMessageInfoStorage
    {
        List<MessageInfoViewModel> GetFullList();
        List<MessageInfoViewModel> GetFilteredList(MessageInfoSearchModel model);
        MessageInfoViewModel? GetElement(MessageInfoSearchModel model);
        MessageInfoViewModel? Insert(MessageInfoBindingModel model);
    }
}
