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
    public interface ICandidateLogic
    {
        List<CandidateViewModel>? ReadList(CandidateSearchModel? model);
        CandidateViewModel? ReadElement(CandidateSearchModel model);
        bool Create(CandidateBindingModel model);
        bool Update(CandidateBindingModel model);
        bool Delete(CandidateBindingModel model);
    }
}
