using HRProDataModels.Enums;
using HRProDataModels.Models;

namespace HRProContracts.BindingModels
{
    public class VacancyBindingModel : IVacancyModel
    {
        public int CompanyId { get; set; }

        public string JobTitle { get; set; } = string.Empty;

        public string Requirements { get; set; } = string.Empty;

        public string Responsibilities { get; set; } = string.Empty;

        public JobTypeEnum JobType { get; set; }

        public string? Salary { get; set; }

        public string? Description { get; set; }

        public VacancyStatusEnum Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? Tags { get; set; }

        public int Id { get; set; }
    }
}
