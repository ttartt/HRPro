using HRProDataModels.Models;

namespace HRProContracts.ViewModels
{
    public class MeetingViewModel : IMeetingModel
    {
        public int CandidateId { get; set; }

        public string Topic { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public DateTime TimeFrom { get; set; }

        public DateTime TimeTo { get; set; }

        public int VacancyId { get; set; }

        public string Place { get; set; } = string.Empty;

        public string? Comment { get; set; }

        public int Id { get; set; }
        public List<MeetingParticipantViewModel> Participants { get; set; } = new();
    }
}
