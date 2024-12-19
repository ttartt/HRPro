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
    public interface ITaskLogic
    {
        List<TaskViewModel>? ReadList(TaskSearchModel? model);
        TaskViewModel? ReadElement(TaskSearchModel model);
        bool Create(TaskBindingModel model);
        bool Update(TaskBindingModel model);
        bool Delete(TaskBindingModel model);
    }
}
