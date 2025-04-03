using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using HRproDatabaseImplement.Models;
using HRProDataModels.Enums;
using HRProDataModels.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace HRProDatabaseImplement.Models
{
    [DataContract]
    public class Template : ITemplateModel
    {
        [DataMember]
        [Required]
        public string Name { get; set; } = string.Empty;
        [DataMember]
        [Required]
        public TemplateTypeEnum Type { get; set; }
        [DataMember]
        [Required]
        public string FilePath { get; set; } = string.Empty;
        [DataMember]
        public int? CompanyId { get; set; }
        [DataMember]
        public int Id { get; set; }

        public virtual Company Company { get; set; }

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
                FilePath = model.FilePath,
                CompanyId = model.CompanyId
            };
        }

        public static Template Create(TemplateViewModel model)
        {
            return new Template
            {
                Id = model.Id,
                Name = model.Name,
                Type = model.Type,
                FilePath = model.FilePath,
                CompanyId = model.CompanyId
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
            CompanyId = model.CompanyId;
        }

        public TemplateViewModel GetViewModel => new()
        {
            Id = Id,
            Name = Name,
            Type = Type,
            FilePath = FilePath,
            CompanyId = CompanyId
        };
    }
}
