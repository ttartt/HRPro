using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using HRProDataModels.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace HRProDatabaseImplement.Models
{
    [DataContract]
    public class Requirement : IRequirementModel
    {
        [Required]
        [DataMember]
        public string Name { get; set; } = string.Empty;
        [DataMember]
        public int Id { get; set; }

        public static Requirement? Create(RequirementBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new Requirement
            {
                Id = model.Id,
                Name = model.Name
            };
        }

        public static Requirement Create(RequirementViewModel model)
        {
            return new Requirement
            {
                Id = model.Id,
                Name = model.Name
            };
        }

        public void Update(RequirementBindingModel model)
        {
            if (model == null)
            {
                return;
            }
            Name = model.Name;
        }

        public RequirementViewModel GetViewModel => new()
        {
            Id = Id,
            Name = Name
        };
    }
}
