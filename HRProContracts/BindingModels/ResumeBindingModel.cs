using HRProDataModels.Enums;
using HRProDataModels.Models;

namespace HRProContracts.BindingModels
{
    public class ResumeBindingModel : IResumeModel
    {
        public int? VacancyId { get; set; }

        public string? Title { get; set; } = string.Empty;

        public string? City { get; set; } = string.Empty;

        public string? LastWorkPlace { get; set; } = string.Empty;

        public string? LastJobTitle { get; set; } = string.Empty;

        public string? Age { get; set; }

        public DateTime CreatedAt { get; set; }

        public int Id { get; set; }

        public int? CompanyId { get; set; }

        public string? Salary { get; set; } = string.Empty;

        public string? CandidateInfo { get; set; } = string.Empty;

        public string? Url { get; set; } = string.Empty;

        public ResumeSourceEnum? Source { get; set; }
    }
}
