using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using HRProDataModels.Models;
using System.ComponentModel.DataAnnotations;

namespace HRProDatabaseImplement.Models
{
    public class Responsibility : IResponsibilityModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public int Id { get; set; }
        public static Responsibility? Create(ResponsibilityBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new Responsibility
            {
                Id = model.Id,
                Name = model.Name
            };
        }

        public static Responsibility Create(ResponsibilityViewModel model)
        {
            return new Responsibility
            {
                Id = model.Id,
                Name = model.Name
            };
        }

        public void Update(ResponsibilityBindingModel model)
        {
            if (model == null)
            {
                return;
            }
            Name = model.Name;
        }

        public ResponsibilityViewModel GetViewModel => new()
        {
            Id = Id,
            Name = Name
        };
    }
}
