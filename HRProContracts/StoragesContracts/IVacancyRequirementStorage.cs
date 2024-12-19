using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;

namespace HRProContracts.StoragesContracts
{
    public interface IVacancyRequirementStorage
    {
        List<VacancyRequirementViewModel> GetFullList();
        List<VacancyRequirementViewModel> GetFilteredList(VacancyRequirementSearchModel model);
        VacancyRequirementViewModel? GetElement(VacancyRequirementSearchModel model);
        VacancyRequirementViewModel? Insert(VacancyRequirementBindingModel model);
        VacancyRequirementViewModel? Update(VacancyRequirementBindingModel model);
        VacancyRequirementViewModel? Delete(VacancyRequirementBindingModel model);
    }
}
