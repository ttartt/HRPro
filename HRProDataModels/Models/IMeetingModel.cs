namespace HRProDataModels.Models
{
    public interface IMeetingModel : IId
    {
        int CandidateId { get; }
        string Topic { get; }
        DateTime Date { get; }
        DateTime TimeFrom { get; }
        DateTime TimeTo { get; }
        int VacancyId { get; }
        string Place {  get; }
        string? Comment { get; }
    }
}
