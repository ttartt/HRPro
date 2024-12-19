using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;

namespace HRProContracts.StoragesContracts
{
    public interface IUserStorage
    {
        List<UserViewModel> GetFullList();
        List<UserViewModel> GetFilteredList(UserSearchModel model);
        UserViewModel? GetElement(UserSearchModel model);
        UserViewModel? Insert(UserBindingModel model);
        UserViewModel? Update(UserBindingModel model);
        UserViewModel? Delete(UserBindingModel model);
    }
}
