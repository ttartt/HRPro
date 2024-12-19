using HRProDataModels.Models;

namespace HRProContracts.ViewModels
{
    public class MeetingParticipantViewModel : IMeetingParticipantModel
    {
        public int UserId { get; set; }

        public int MeetingId { get; set; }

        public int Id { get; set; }
    }
}
