using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;

namespace HRProContracts.StoragesContracts
{
    public interface ICompanyStorage
    {
        List<CompanyViewModel> GetFullList();
        List<CompanyViewModel> GetFilteredList(CompanySearchModel model);
        CompanyViewModel? GetElement(CompanySearchModel model);
        int? Insert(CompanyBindingModel model);
        CompanyViewModel? Update(CompanyBindingModel model);
        CompanyViewModel? Delete(CompanyBindingModel model);
    }
}
