using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using HRProDataModels.Enums;
using HRProDataModels.Models;
using System.ComponentModel.DataAnnotations;

namespace HRProDatabaseImplement.Models
{
    public class Template : ITemplateModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public TemplateTypeEnum Type { get; set; }

        [Required]
        public string FilePath { get; set; } = string.Empty;

        public int Id { get; set; }

        public static Template? Create(TemplateBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new Template()
            {
                Id = model.Id,
                Name = model.Name,
                Type = model.Type,
                FilePath = model.FilePath
            };
        }

        public static Template Create(TemplateViewModel model)
        {
            return new Template
            {
                Id = model.Id,
                Name = model.Name,
                Type = model.Type,
                FilePath = model.FilePath
            };
        }

        public void Update(TemplateBindingModel model)
        {
            if (model == null)
            {
                return;
            }
            Name = model.Name;
            Type = model.Type;
            FilePath = model.FilePath;
        }

        public TemplateViewModel GetViewModel => new()
        {
            Id = Id,
            Name = Name,
            Type = Type,
            FilePath = FilePath
        };
    }
}
