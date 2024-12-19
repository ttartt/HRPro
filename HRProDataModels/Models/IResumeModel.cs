using HRProDataModels.Enums;

namespace HRProDataModels.Models
{
    public interface IResumeModel : IId
    {
        int VacancyId { get; }
        int UserId { get; }
        string Title { get; }
        string Experience { get; }
        string Education { get; }
        string? Description { get; }
        string Skills { get; }
        DateTime CreatedAt { get; }
        ResumeStatusEnum Status { get; }
    }
}
