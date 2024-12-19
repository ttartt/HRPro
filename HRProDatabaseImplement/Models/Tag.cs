using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using HRProDataModels.Enums;
using HRProDataModels.Models;
using System.ComponentModel.DataAnnotations;

namespace HRProDatabaseImplement.Models
{
    public class Tag : ITagModel
    {
        [Required]
        public string TagName { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DataTypeEnum Type { get; set; }

        [Required]
        public int TemplateId { get; set; }

        public int Id { get; set; }

        public virtual Template Template { get; set; }

        public static Tag? Create(TagBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new Tag
            {
                Id = model.Id,
                TagName = model.TagName,
                Name = model.Name,
                Type = model.Type,
                TemplateId = model.TemplateId
            };
        }

        public static Tag Create(TagViewModel model)
        {
            return new Tag
            {
                Id = model.Id,
                TagName = model.TagName,
                Name = model.Name,
                Type = model.Type,
                TemplateId = model.TemplateId
            };
        }

        public void Update(TagBindingModel model)
        {
            if (model == null)
            {
                return;
            }
            TagName = model.TagName;
            Name = model.Name;
            Type = model.Type;
            TemplateId = model.TemplateId;
        }

        public TagViewModel GetViewModel => new()
        {
            Id = Id,
            TagName = TagName,
            Name = Name,
            Type = Type,
            TemplateId = TemplateId
        };
    }
}
