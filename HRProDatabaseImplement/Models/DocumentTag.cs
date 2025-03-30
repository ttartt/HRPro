using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using HRProDataModels.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace HRProDatabaseImplement.Models
{
    [DataContract]
    public class DocumentTag : IDocumentTagModel
    {
        [Required]
        [DataMember]
        public int DocumentId { get; set; }

        [Required]
        [DataMember]
        public int TagId { get; set; }

        [Required]
        [DataMember]
        public string Value { get; set; } = string.Empty;

        [DataMember]
        public int Id { get; set; }

        public static DocumentTag? Create(DocumentTagBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new DocumentTag
            {
                Id = model.Id,
                DocumentId = model.DocumentId,
                TagId = model.TagId,
                Value = model.Value
            };
        }

        public static DocumentTag Create(DocumentTagViewModel model)
        {
            return new DocumentTag
            {
                Id = model.Id,
                DocumentId = model.DocumentId,
                TagId = model.TagId,
                Value = model.Value
            };
        }

        public void Update(DocumentTagBindingModel model)
        {
            if (model == null)
            {
                return;
            }
            DocumentId = model.DocumentId;
            TagId = model.TagId;
            Value = model.Value;
        }

        public DocumentTagViewModel GetViewModel => new()
        {
            Id = Id,
            DocumentId = DocumentId,
            TagId = TagId,
            Value = Value
        };
    }
}
