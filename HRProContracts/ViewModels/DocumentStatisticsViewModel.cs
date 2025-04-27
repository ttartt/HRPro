namespace HRProContracts.ViewModels
{
    public class DocumentStatisticsViewModel
    {
        public int TotalDocuments { get; set; }
        public Dictionary<string, int> DocumentsByType { get; set; } = [];
        public Dictionary<DateTime, int> DailyCounts { get; set; } = [];
    }
}
