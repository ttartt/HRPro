using HRProDataModels.Models;

namespace HRProContracts.BindingModels
{
    public class MeetingParticipantBindingModel : IMeetingParticipantModel
    {
        public int UserId { get; set; }

        public int MeetingId { get; set; }

        public int Id { get; set; }
    }
}
