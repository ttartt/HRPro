using HRProDataModels.Models;

namespace HRProContracts.BindingModels
{
    public class CompanyBindingModel : ICompanyModel
    {
        public string Name { get; set; } = string.Empty;

        public string? LogoFilePath { get; set; }

        public string? Description { get; set; }

        public string? Website { get; set; }

        public string? Address { get; set; }

        public string? Contacts { get; set; }

        public int Id { get; set; }
    }
}
