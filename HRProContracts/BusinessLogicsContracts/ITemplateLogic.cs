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
    public interface ITemplateLogic
    {
        List<TemplateViewModel>? ReadList(TemplateSearchModel? model);
        TemplateViewModel? ReadElement(TemplateSearchModel model);
        int? Create(TemplateBindingModel model);
        bool Update(TemplateBindingModel model);
        bool Delete(TemplateBindingModel model);
    }
}
