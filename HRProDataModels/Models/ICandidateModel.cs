namespace HRProDataModels.Models
{
    public interface ICandidateModel : IId
    {
        int? TestTaskId { get; }
        string FIO { get; }
        string? Email { get; }
        string? PhoneNumber { get; }
    }
}
