﻿using HRProContracts.BindingModels;
using HRProContracts.ViewModels;

namespace HRProContracts.BusinessLogicsContracts
{
    public interface IReportLogic
    {       
        void SaveResumeToPdf(ReportBindingModel model);
        void SaveResumesStatisticsToPdf(ReportBindingModel model);
        public SalaryStatisticsViewModel GetSalaryStatistics();
        public VacancyStatusStatisticsViewModel GetVacancyStatusStatistics();
        public ResumeStatisticsViewModel GetResumeStatistics();
    }
}
