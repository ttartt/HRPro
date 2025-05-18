
using HRProDataModels.Enums;

namespace HRProDataModels.Models
{
    public interface IResumeModel : IId
    {
        int? VacancyId { get; }
        string? Title { get; }
        string? City { get; }
        string? Url { get; }
        string? LastWorkPlace { get; }
        string? LastJobTitle { get; }
        string? Age { get; }
        DateTime CreatedAt { get; }
        string? Salary { get; }
        string? CandidateInfo { get; }
        ResumeSourceEnum? Source { get; }
        int? CompanyId { get; }
    }
}
