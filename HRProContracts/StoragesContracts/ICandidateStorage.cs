using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;

namespace HRProContracts.StoragesContracts
{
    public interface ICandidateStorage
    {
        List<CandidateViewModel> GetFullList();
        List<CandidateViewModel> GetFilteredList(CandidateSearchModel model);
        CandidateViewModel? GetElement(CandidateSearchModel model);
        CandidateViewModel? Insert(CandidateBindingModel model);
        CandidateViewModel? Update(CandidateBindingModel model);
        CandidateViewModel? Delete(CandidateBindingModel model);
    }
}
