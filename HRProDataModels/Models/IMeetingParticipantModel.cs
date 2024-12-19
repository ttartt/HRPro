namespace HRProDataModels.Models
{
    public interface IMeetingParticipantModel : IId
    {
        int UserId { get; }
        int MeetingId { get; }
    }
}
