using HRProDataModels.Enums;

namespace HRProDataModels.Models
{
    public interface IResumeModel : IId
    {
        int? VacancyId { get; }
        string? Title { get; }
        string? City { get; }
        string? Url { get; }
        string? Experience { get; }
        string? Education { get; }
        string? Description { get; }
        DateTime CreatedAt { get; }
        ResumeStatusEnum Status { get; }
        string? Salary { get; }
        string? CandidateInfo { get; }
    }
}
