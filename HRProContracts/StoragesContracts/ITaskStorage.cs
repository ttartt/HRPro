using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;

namespace HRProContracts.StoragesContracts
{
    public interface ITaskStorage
    {
        List<TaskViewModel> GetFullList();
        List<TaskViewModel> GetFilteredList(TaskSearchModel model);
        TaskViewModel? GetElement(TaskSearchModel model);
        TaskViewModel? Insert(TaskBindingModel model);
        TaskViewModel? Update(TaskBindingModel model);
        TaskViewModel? Delete(TaskBindingModel model);
    }
}
