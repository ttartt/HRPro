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
    public class Document : IDocumentModel
    {
        [Required]
        [DataMember]
        public int CreatorId { get; set; }
        [Required]
        [DataMember]
        public int CompanyId { get; set; }
        [Required]
        [DataMember]
        public string Name { get; set; } = string.Empty;
        [Required]
        [DataMember]
        public DocumentStatusEnum Status { get; set; }
        [Required]
        [DataMember]
        public DateTime CreatedAt { get; set; }
        [Required]
        [DataMember]
        public int TemplateId { get; set; }
        [DataMember]
        public int Id { get; set; }
        public virtual Company Company { get; set; }
        public virtual User Creator { get; set; }
        public virtual Template Template { get; set; }

        public static Document? Create(DocumentBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new Document()
            {
                Id = model.Id,
                CreatorId = model.CreatorId,
                CompanyId = model.CompanyId,
                Name = model.Name,
                Status = model.Status,
                CreatedAt = model.CreatedAt,
                TemplateId = model.TemplateId
            };
        }

        public static Document Create(DocumentViewModel model)
        {
            return new Document
            {
                Id = model.Id,
                CreatorId = model.CreatorId,
                CompanyId = model.CompanyId,
                Name = model.Name,
                Status = model.Status,
                CreatedAt = model.CreatedAt,
                TemplateId = model.TemplateId
            };
        }

        public void Update(DocumentBindingModel model)
        {
            if (model == null)
            {
                return;
            }
            Name = model.Name;
            Status = model.Status;
            CreatedAt = model.CreatedAt;
            TemplateId = model.TemplateId;
        }

        public DocumentViewModel GetViewModel => new()
        {
            Id = Id,
            CreatorId = CreatorId,
            CompanyId = CompanyId,
            Name = Name,
            Status = Status,
            CreatedAt = CreatedAt,
            TemplateId = TemplateId
        };
    }
}
