using HRProDataModels.Models;

namespace HRProContracts.ViewModels
{
    public class VacancyResponsibilityViewModel : IVacancyResponsibility
    {
        public int VacancyId { get; set; }

        public int ResponsibilityId { get; set; }

        public int Id { get; set; }
    }
}
