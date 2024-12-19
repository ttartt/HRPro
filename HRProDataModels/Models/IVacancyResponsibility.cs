namespace HRProDataModels.Models
{
    public interface IVacancyResponsibility : IId
    {
        int VacancyId { get; }
        int ResponsibilityId { get; }
    }
}
