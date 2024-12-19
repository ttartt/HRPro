using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;

namespace HRProContracts.BusinessLogicsContracts
{
    public interface IResumeLogic
    {
        List<ResumeViewModel>? ReadList(ResumeSearchModel? model);
        ResumeViewModel? ReadElement(ResumeSearchModel model);
        bool Create(ResumeBindingModel model);
        bool Update(ResumeBindingModel model);
        bool Delete(ResumeBindingModel model);
    }
}
