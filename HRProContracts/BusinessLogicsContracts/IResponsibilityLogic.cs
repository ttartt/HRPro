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
    public interface IResponsibilityLogic
    {
        List<ResponsibilityViewModel>? ReadList(ResponsibilitySearchModel? model);
        ResponsibilityViewModel? ReadElement(ResponsibilitySearchModel model);
        bool Create(ResponsibilityBindingModel model);
        bool Update(ResponsibilityBindingModel model);
        bool Delete(ResponsibilityBindingModel model);
    }
}
