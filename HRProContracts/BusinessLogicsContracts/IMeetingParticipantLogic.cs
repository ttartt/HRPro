using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;

namespace HRProContracts.BusinessLogicsContracts
{
    public interface IMeetingParticipantLogic
    {
        List<MeetingParticipantViewModel>? ReadList(MeetingParticipantSearchModel? model);
        MeetingParticipantViewModel? ReadElement(MeetingParticipantSearchModel model);
        bool Create(MeetingParticipantBindingModel model);
        bool Update(MeetingParticipantBindingModel model);
        bool Delete(MeetingParticipantBindingModel model);
    }
}
