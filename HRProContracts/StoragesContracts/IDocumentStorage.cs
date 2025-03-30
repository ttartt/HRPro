using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;

namespace HRProContracts.StoragesContracts
{
    public interface IDocumentStorage
    {
        List<DocumentViewModel> GetFullList();
        List<DocumentViewModel> GetFilteredList(DocumentSearchModel model);
        DocumentViewModel? GetElement(DocumentSearchModel model);
        int? Insert(DocumentBindingModel model);
        DocumentViewModel? Update(DocumentBindingModel model);
        DocumentViewModel? Delete(DocumentBindingModel model);
    }
}
