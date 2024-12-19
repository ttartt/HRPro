using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;

namespace HRProContracts.StoragesContracts
{
    public interface IMeetingStorage
    {
        List<MeetingViewModel> GetFullList();
        List<MeetingViewModel> GetFilteredList(MeetingSearchModel model);
        MeetingViewModel? GetElement(MeetingSearchModel model);
        MeetingViewModel? Insert(MeetingBindingModel model);
        MeetingViewModel? Update(MeetingBindingModel model);
        MeetingViewModel? Delete(MeetingBindingModel model);
    }
}
