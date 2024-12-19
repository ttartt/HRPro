using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;

namespace HRProContracts.StoragesContracts
{
    public interface ITemplateStorage
    {
        List<TemplateViewModel> GetFullList();
        List<TemplateViewModel> GetFilteredList(TemplateSearchModel model);
        TemplateViewModel? GetElement(TemplateSearchModel model);
        TemplateViewModel? Insert(TemplateBindingModel model);
        TemplateViewModel? Update(TemplateBindingModel model);
        TemplateViewModel? Delete(TemplateBindingModel model);
    }
}
