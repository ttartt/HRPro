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
    public interface IVacancyRequirementLogic
    {
        List<VacancyRequirementViewModel>? ReadList(VacancyRequirementSearchModel? model);
        VacancyRequirementViewModel? ReadElement(VacancyRequirementSearchModel model);
        bool Create(VacancyRequirementBindingModel model);
        bool Update(VacancyRequirementBindingModel model);
        bool Delete(VacancyRequirementBindingModel model);
    }
}
