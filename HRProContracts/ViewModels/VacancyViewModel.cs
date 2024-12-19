using HRProDataModels.Enums;
using HRProDataModels.Models;

namespace HRProContracts.ViewModels
{
    public class VacancyViewModel : IVacancyModel
    {
        public int CompanyId { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        public string JobTitle { get; set; } = string.Empty;

        public List<RequirementViewModel> Requirements { get; set; } = new();

        public List<ResponsibilityViewModel> Responsibilities { get; set; } = new();

        public JobTypeEnum JobType { get; set; }

        public string? Salary { get; set; }

        public string? Description { get; set; }

        public VacancyStatusEnum Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? Tags { get; set; }

        public int Id { get; set; }

        public List<ResumeViewModel> Resumes { get; set; } = new();
    }
}
