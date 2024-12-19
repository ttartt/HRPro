using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;

namespace HRProContracts.StoragesContracts
{
    public interface IVacancyResponsibilityStorage
    {
        List<VacancyResponsibilityViewModel> GetFullList();
        List<VacancyResponsibilityViewModel> GetFilteredList(VacancyResponsibilitySearchModel model);
        VacancyResponsibilityViewModel? GetElement(VacancyResponsibilitySearchModel model);
        VacancyResponsibilityViewModel? Insert(VacancyResponsibilityBindingModel model);
        VacancyResponsibilityViewModel? Update(VacancyResponsibilityBindingModel model);
        VacancyResponsibilityViewModel? Delete(VacancyResponsibilityBindingModel model);
    }
}
