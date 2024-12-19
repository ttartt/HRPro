using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;

namespace HRProContracts.StoragesContracts
{
    public interface IVacancyStorage
    {
        List<VacancyViewModel> GetFullList();
        List<VacancyViewModel> GetFilteredList(VacancySearchModel model);
        VacancyViewModel? GetElement(VacancySearchModel model);
        VacancyViewModel? Insert(VacancyBindingModel model);
        VacancyViewModel? Update(VacancyBindingModel model);
        VacancyViewModel? Delete(VacancyBindingModel model);
    }
}
