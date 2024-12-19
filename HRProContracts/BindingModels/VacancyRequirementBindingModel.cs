using HRProDataModels.Models;

namespace HRProContracts.BindingModels
{
    public class VacancyRequirementBindingModel : IVacancyRequirement
    {
        public int VacancyId { get; set; }

        public int RequirementId { get; set; }

        public int Id { get; set; }
    }
}
