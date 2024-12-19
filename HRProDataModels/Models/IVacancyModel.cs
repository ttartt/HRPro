using HRProDataModels.Enums;

namespace HRProDataModels.Models
{
    public interface IVacancyModel : IId
    {
        int CompanyId { get; }
        string JobTitle { get; }        
        JobTypeEnum JobType { get; }
        string? Salary { get; }
        string? Description { get; }
        VacancyStatusEnum Status { get; }
        DateTime CreatedAt {  get; }
        string? Tags { get; }
    }
}
