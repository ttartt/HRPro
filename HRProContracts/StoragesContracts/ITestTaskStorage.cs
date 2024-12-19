using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;

namespace HRProContracts.StoragesContracts
{
    public interface ITestTaskStorage
    {
        List<TestTaskViewModel> GetFullList();
        List<TestTaskViewModel> GetFilteredList(TestTaskSearchModel model);
        TestTaskViewModel? GetElement(TestTaskSearchModel model);
        TestTaskViewModel? Insert(TestTaskBindingModel model);
        TestTaskViewModel? Update(TestTaskBindingModel model);
        TestTaskViewModel? Delete(TestTaskBindingModel model);
    }
}
