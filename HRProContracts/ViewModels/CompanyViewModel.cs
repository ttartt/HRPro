using HRProDataModels.Models;

namespace HRProContracts.ViewModels
{
    public class CompanyViewModel : ICompanyModel
    {
        public string Name { get; set; } = string.Empty;
        public string? LogoFilePath { get; set; }

        public string? Description { get; set; }

        public string? Website { get; set; }

        public string? Address { get; set; }

        public string? Contacts { get; set; }

        public int Id { get; set; }

        public List<VacancyViewModel> Vacancies { get; set; } = new();

        public List<UserViewModel> Employees { get; set; } = new();
    }
}
