using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using HRProDataModels.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRproDatabaseImplement.Models
{
    public class Company : ICompanyModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? LogoFilePath { get; set; }

        public string? Description { get; set; }

        public string? Website { get; set; }

        public string? Address { get; set; }

        public string? Contacts { get; set; }

        public int Id { get; set; }

        public static Company? Create(CompanyBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new Company()
            {
                Id = model.Id,
                Name = model.Name,
                LogoFilePath = model.LogoFilePath,
                Description = model.Description,
                Website = model.Website,
                Address = model.Address,
                Contacts = model.Contacts
            };
        }

        public static Company Create(CompanyViewModel model)
        {
            return new Company
            {
                Id = model.Id,
                Name = model.Name,
                LogoFilePath = model.LogoFilePath,
                Description = model.Description,
                Website = model.Website,
                Address = model.Address,
                Contacts = model.Contacts
            };
        }

        public void Update(CompanyBindingModel model)
        {
            if (model == null)
            {
                return;
            }
            Name = model.Name;
            LogoFilePath = model.LogoFilePath;
            Description = model.Description;
            Website = model.Website;
            Address = model.Address;
            Contacts = model.Contacts;
        }
        public CompanyViewModel GetViewModel => new()
        {
            Id = Id,
            Name = Name,
            LogoFilePath = LogoFilePath,
            Description = Description,
            Website = Website,
            Address = Address,
            Contacts = Contacts
        };
    }
}
