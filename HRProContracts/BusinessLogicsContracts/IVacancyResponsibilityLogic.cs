using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRProContracts.BusinessLogicsContracts
{
    public interface IVacancyResponsibilityLogic
    {
        List<VacancyResponsibilityViewModel>? ReadList(VacancyResponsibilitySearchModel? model);
        VacancyResponsibilityViewModel? ReadElement(VacancyResponsibilitySearchModel model);
        bool Create(VacancyResponsibilityBindingModel model);
        bool Update(VacancyResponsibilityBindingModel model);
        bool Delete(VacancyResponsibilityBindingModel model);
    }
}
