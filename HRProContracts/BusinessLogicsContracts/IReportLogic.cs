using HRProContracts.BindingModels;

namespace HRProContracts.BusinessLogicsContracts
{
    public interface IReportLogic
    {       
        void SaveResumeToPdf(ReportBindingModel model);
        void SaveResumesStatisticsToPdf(ReportBindingModel model);
    }
}
