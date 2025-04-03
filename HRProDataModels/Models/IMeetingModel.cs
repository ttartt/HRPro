namespace HRProDataModels.Models
{
    public interface IMeetingModel : IId
    {
        string Topic { get; }
        DateTime Date { get; }
        DateTime TimeFrom { get; }
        DateTime TimeTo { get; }
        int? VacancyId { get; }
        int? ResumeId { get; }

        int? CompanyId { get; }
        string? Place {  get; }
        string? Comment { get; }
    }
}
