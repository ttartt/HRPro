using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;

namespace HRProContracts.StoragesContracts
{
    public interface ITagStorage
    {
        List<TagViewModel> GetFullList();
        List<TagViewModel> GetFilteredList(TagSearchModel model);
        TagViewModel? GetElement(TagSearchModel model);
        int? Insert(TagBindingModel model);
        TagViewModel? Update(TagBindingModel model);
        TagViewModel? Delete(TagBindingModel model);
    }
}
