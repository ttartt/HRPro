namespace HRProContracts.BindingModels
{
    public class ReportBindingModel
    {
        public string FileName { get; set; } = string.Empty;
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int ResumeId { get; set; }
        public int VacancyId { get; set; }
    }
}
