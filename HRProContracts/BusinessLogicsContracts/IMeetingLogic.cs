using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;

namespace HRProContracts.BusinessLogicsContracts
{
    public interface IMeetingLogic
    {
        List<MeetingViewModel>? ReadList(MeetingSearchModel? model);
        MeetingViewModel? ReadElement(MeetingSearchModel model);
        int? Create(MeetingBindingModel model);
        bool Update(MeetingBindingModel model);
        bool Delete(MeetingBindingModel model);
    }
}
