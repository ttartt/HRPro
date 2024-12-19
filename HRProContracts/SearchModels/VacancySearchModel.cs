using HRProDataModels.Enums;

namespace HRProContracts.SearchModels
{
    public class VacancySearchModel
    {
        public int? CompanyId { get; set; }

        public string? JobTitle { get; set; }

        public string? Salary { get; set; }

        public string? Tags { get; set; }

        public int? Id { get; set; }

        public VacancyStatusEnum? Status { get; set; }
    }
}
