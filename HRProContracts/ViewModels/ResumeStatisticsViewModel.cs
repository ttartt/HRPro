namespace HRProContracts.ViewModels
{
    public class ResumeStatisticsViewModel
    {
        public Dictionary<string, decimal> AverageSalaryByCity { get; set; } = [];
        public Dictionary<string, SalaryStatsViewModel> SalaryByTitle { get; set; } = [];
        public int TotalResumes { get; set; }
        public int ResumesWithSalary { get; set; }
        public int ResumesWithoutSalary { get; set; }
    }
}
