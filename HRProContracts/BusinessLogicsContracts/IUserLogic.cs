using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;

namespace HRProContracts.BusinessLogicsContracts
{
    public interface IUserLogic
    {
        List<UserViewModel>? ReadList(UserSearchModel? model);
        UserViewModel? ReadElement(UserSearchModel model);
        bool Create(UserBindingModel model);
        bool Update(UserBindingModel model);
        bool Delete(UserBindingModel model);
    }
}
