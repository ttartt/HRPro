using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;

namespace HRProContracts.StoragesContracts
{
    public interface IResumeStorage
    {
        List<ResumeViewModel> GetFullList();
        List<ResumeViewModel> GetFilteredList(ResumeSearchModel model);
        ResumeViewModel? GetElement(ResumeSearchModel model);
        ResumeViewModel? Insert(ResumeBindingModel model);
        ResumeViewModel? Update(ResumeBindingModel model);
        ResumeViewModel? Delete(ResumeBindingModel model);
    }
}
