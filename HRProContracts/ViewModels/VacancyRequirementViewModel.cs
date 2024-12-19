using HRProDataModels.Models;

namespace HRProContracts.ViewModels
{
    public class VacancyRequirementViewModel : IVacancyRequirement
    {
        public int VacancyId { get; set; }

        public int RequirementId { get; set; }

        public int Id { get; set; }
    }
}
