using HRProDataModels.Enums;
using HRProDataModels.Models;

namespace HRProContracts.ViewModels
{
    public class ResumeViewModel : IResumeModel
    {
        public int? VacancyId { get; set; }
        public int? CompanyId { get; set; }

        public string? VacancyName { get; set; } = string.Empty;

        public string? Title { get; set; } = string.Empty;

        public string? City { get; set; } = string.Empty;

        public string? Url { get; set; } = string.Empty;

        public string? Experience { get; set; } = string.Empty;

        public string? Education { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Skills { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public int Id { get; set; }

        public string? Salary { get; set; } = string.Empty;

        public string? CandidateInfo { get; set; } = string.Empty;
    }
}