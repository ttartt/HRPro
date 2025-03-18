using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;

namespace HRProContracts.BusinessLogicsContracts
{
    public interface IVacancyLogic
    {
        List<VacancyViewModel>? ReadList(VacancySearchModel? model);
        VacancyViewModel? ReadElement(VacancySearchModel model);
        int? Create(VacancyBindingModel model);
        bool Update(VacancyBindingModel model);
        bool Delete(VacancyBindingModel model);
    }
}
