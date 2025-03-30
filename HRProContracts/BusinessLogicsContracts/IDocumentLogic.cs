using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;

namespace HRProContracts.BusinessLogicsContracts
{
    public interface IDocumentLogic
    {
        List<DocumentViewModel>? ReadList(DocumentSearchModel? model);
        DocumentViewModel? ReadElement(DocumentSearchModel model);
        int? Create(DocumentBindingModel model);
        bool Update(DocumentBindingModel model);
        bool Delete(DocumentBindingModel model);
    }
}
