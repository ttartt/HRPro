using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;

namespace HRProContracts.StoragesContracts
{
    public interface IResponsibilityStorage
    {
        List<ResponsibilityViewModel> GetFullList();
        List<ResponsibilityViewModel> GetFilteredList(ResponsibilitySearchModel model);
        ResponsibilityViewModel? GetElement(ResponsibilitySearchModel model);
        int? Insert(ResponsibilityBindingModel model);
        ResponsibilityViewModel? Update(ResponsibilityBindingModel model);
        ResponsibilityViewModel? Delete(ResponsibilityBindingModel model);
    }
}
