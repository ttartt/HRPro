﻿namespace HRProContracts.ViewModels
{
    public class VacancyStatusStatisticsViewModel
    {
        public Dictionary<string, int> StatusCounts { get; set; } = [];
        public Dictionary<string, double> StatusPercentages { get; set; } = [];
        public int TotalVacancies { get; set; }
    }
}
