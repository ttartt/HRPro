using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;

namespace HRProContracts.StoragesContracts
{
    public interface IRequirementStorage
    {
        List<RequirementViewModel> GetFullList();
        List<RequirementViewModel> GetFilteredList(RequirementSearchModel model);
        RequirementViewModel? GetElement(RequirementSearchModel model);
        RequirementViewModel? Insert(RequirementBindingModel model);
        RequirementViewModel? Update(RequirementBindingModel model);
        RequirementViewModel? Delete(RequirementBindingModel model);
    }
}
