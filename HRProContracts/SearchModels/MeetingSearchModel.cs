namespace HRProContracts.SearchModels
{
    public class MeetingSearchModel
    {
        public int? Id { get; set; }
        public string? Topic { get; set; }
        public int? VacancyId { get; set; }
        public int? CandidateId { get; set; }
        public DateTime? Date { get; set; }
    }
}
