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
    public interface ITestTaskLogic
    {
        List<TestTaskViewModel>? ReadList(TestTaskSearchModel? model);
        TestTaskViewModel? ReadElement(TestTaskSearchModel model);
        bool Create(TestTaskBindingModel model);
        bool Update(TestTaskBindingModel model);
        bool Delete(TestTaskBindingModel model);
    }
}
