using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;

namespace HRProContracts.StoragesContracts
{
    public interface IMeetingParticipantStorage
    {
        List<MeetingParticipantViewModel> GetFullList();
        List<MeetingParticipantViewModel> GetFilteredList(MeetingParticipantSearchModel model);
        MeetingParticipantViewModel? GetElement(MeetingParticipantSearchModel model);
        MeetingParticipantViewModel? Insert(MeetingParticipantBindingModel model);
        MeetingParticipantViewModel? Update(MeetingParticipantBindingModel model);
        MeetingParticipantViewModel? Delete(MeetingParticipantBindingModel model);
    }
}
