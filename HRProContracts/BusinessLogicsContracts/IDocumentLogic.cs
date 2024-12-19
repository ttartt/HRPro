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
    public interface IDocumentLogic
    {
        List<DocumentViewModel>? ReadList(DocumentSearchModel? model);
        DocumentViewModel? ReadElement(DocumentSearchModel model);
        bool Create(DocumentBindingModel model);
        bool Update(DocumentBindingModel model);
        bool Delete(DocumentBindingModel model);
    }
}
