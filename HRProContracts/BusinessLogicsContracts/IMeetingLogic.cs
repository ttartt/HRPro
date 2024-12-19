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
    public interface IMeetingLogic
    {
        List<MeetingViewModel>? ReadList(MeetingSearchModel? model);
        MeetingViewModel? ReadElement(MeetingSearchModel model);
        bool Create(MeetingBindingModel model);
        bool Update(MeetingBindingModel model);
        bool Delete(MeetingBindingModel model);
    }
}
