using HRProContracts.BindingModels;
using HRProContracts.ViewModels;

namespace HRProContracts.BusinessLogicsContracts
{
    public interface IReportLogic
    { 
        public SalaryStatisticsViewModel GetSalaryStatistics();
        public VacancyStatusStatisticsViewModel GetVacancyStatusStatistics();
        public ResumeStatisticsViewModel GetResumeStatistics();
    }
}
