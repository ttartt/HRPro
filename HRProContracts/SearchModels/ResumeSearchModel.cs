using HRProDataModels.Enums;

namespace HRProContracts.SearchModels
{
    public class ResumeSearchModel
    {
        public int? VacancyId { get; set; }

        public int? UserId { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? City { get; set; }

        public ResumeStatusEnum? Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? Id { get; set; }
    }
}
