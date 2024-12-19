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
    public interface ITagLogic
    {
        List<TagViewModel>? ReadList(TagSearchModel? model);
        TagViewModel? ReadElement(TagSearchModel model);
        bool Create(TagBindingModel model);
        bool Update(TagBindingModel model);
        bool Delete(TagBindingModel model);
    }
}
