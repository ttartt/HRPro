using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;

namespace HRProContracts.BusinessLogicsContracts
{
    public interface ICompanyLogic
    {
        List<CompanyViewModel>? ReadList(CompanySearchModel? model);
        CompanyViewModel? ReadElement(CompanySearchModel model);
        int? Create(CompanyBindingModel model);
        bool Update(CompanyBindingModel model);
        bool Delete(CompanyBindingModel model);
    }
}
