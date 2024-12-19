using HRProDataModels.Models;

namespace HRProContracts.BindingModels
{
    public class VacancyResponsibilityBindingModel : IVacancyResponsibility
    {
        public int VacancyId { get; set; }

        public int ResponsibilityId { get; set; }

        public int Id { get; set; }
    }
}
