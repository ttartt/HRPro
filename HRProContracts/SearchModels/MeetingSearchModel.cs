namespace HRProContracts.SearchModels
{
    public class MeetingSearchModel
    {
        public int? Id { get; set; }
        public string? Topic { get; set; }
        public int? VacancyId { get; set; }
        public int? ResumeId { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? TimeFrom { get; set; }
        public DateTime? TimeTo { get; set; }
        public int? CompanyId { get; set; }
        public string? GoogleEventId { get; set; }
    }
}
