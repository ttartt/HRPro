using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;

namespace HRProContracts.BusinessLogicsContracts
{
    public interface IDocumentTagLogic
    {
        List<DocumentTagViewModel>? ReadList(DocumentTagSearchModel? model);
        DocumentTagViewModel? ReadElement(DocumentTagSearchModel model);
        bool Create(DocumentTagBindingModel model);
        bool Update(DocumentTagBindingModel model);
        bool Delete(DocumentTagBindingModel model);
    }
}
