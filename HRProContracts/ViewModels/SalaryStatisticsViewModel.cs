namespace HRProContracts.ViewModels
{
    public class SalaryStatisticsViewModel
    {
        public Dictionary<string, decimal> AverageSalaryByPosition { get; set; } = [];
        public Dictionary<string, SalaryRangeViewModel> SalaryRangesByPosition { get; set; } = [];
        // public Dictionary<string, double> AverageSalaryByCity { get; set; }
    }
}
