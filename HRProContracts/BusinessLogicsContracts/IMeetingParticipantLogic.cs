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
    public interface IMeetingParticipantLogic
    {
        List<MeetingParticipantViewModel>? ReadList(MeetingParticipantSearchModel? model);
        MeetingParticipantViewModel? ReadElement(MeetingParticipantSearchModel model);
        bool Create(MeetingParticipantBindingModel model);
        bool Update(MeetingParticipantBindingModel model);
        bool Delete(MeetingParticipantBindingModel model);
    }
}
