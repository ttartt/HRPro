using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRProContracts.BindingModels
{
    public class DeleteEventRequest
    {
        public string EventId { get; set; }
        public string GoogleAccessToken { get; set; }
        public int MeetingId { get; set; }
    }
}
