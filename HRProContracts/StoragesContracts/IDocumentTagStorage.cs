using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;

namespace HRProContracts.StoragesContracts
{
    public interface IDocumentTagStorage
    {
        List<DocumentTagViewModel> GetFullList();
        List<DocumentTagViewModel> GetFilteredList(DocumentTagSearchModel model);
        DocumentTagViewModel? GetElement(DocumentTagSearchModel model);
        DocumentTagViewModel? Insert(DocumentTagBindingModel model);
        DocumentTagViewModel? Update(DocumentTagBindingModel model);
        DocumentTagViewModel? Delete(DocumentTagBindingModel model);
    }
}
