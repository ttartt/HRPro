namespace HRProDataModels.Models
{
    public interface IVacancyRequirement : IId
    {
        int VacancyId { get; }
        int RequirementId { get; }
    }
}
