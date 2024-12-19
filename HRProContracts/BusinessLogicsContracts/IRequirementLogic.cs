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
    public interface IRequirementLogic
    {
        List<RequirementViewModel>? ReadList(RequirementSearchModel? model);
        RequirementViewModel? ReadElement(RequirementSearchModel model);
        int? Create(RequirementBindingModel model);
        bool Update(RequirementBindingModel model);
        bool Delete(RequirementBindingModel model);
    }
}
