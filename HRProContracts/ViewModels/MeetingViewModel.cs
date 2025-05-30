﻿using HRProDataModels.Models;

namespace HRProContracts.ViewModels
{
    public class MeetingViewModel : IMeetingModel
    {
        public int? ResumeId { get; set; }

        public int? CompanyId { get; set; }

        public string Topic { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public DateTime TimeFrom { get; set; }

        public DateTime TimeTo { get; set; }

        public int? VacancyId { get; set; }

        public string? Place { get; set; } = string.Empty;

        public string? Comment { get; set; }

        public int Id { get; set; }
        public List<UserViewModel> Participants { get; set; } = new();

        public string? GoogleEventId { get; set; }
    }
}
