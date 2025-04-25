using HRProDataModels.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRProContracts.BindingModels
{
    public class MeetingBindingModel : IMeetingModel
    {
        public int? ResumeId { get; set; }

        public int? CompanyId { get; set; }

        public string Topic { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public DateTime TimeFrom { get; set; }

        public DateTime TimeTo { get; set; }

        public int? VacancyId { get; set; }

        public string? Place { get; set; }

        public string? Comment { get; set; }

        public int Id { get; set; }

        public string? GoogleEventId { get; set; }
        [HiddenInput]
        public string SelectedParticipantIds { get; set; } = string.Empty;
    }
}
