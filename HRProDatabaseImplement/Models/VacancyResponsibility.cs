using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using HRProDataModels.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace HRProDatabaseImplement.Models
{
    [DataContract]
    public class VacancyResponsibility : IVacancyResponsibility
    {
        [DataMember]
        [Required]
        public int VacancyId { get; set; }
        [DataMember]
        [Required]
        public int ResponsibilityId { get; set; }
        [DataMember]
        public int Id { get; set; }

        public static VacancyResponsibility? Create(VacancyResponsibilityBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new VacancyResponsibility
            {
                Id = model.Id,
                VacancyId = model.VacancyId,
                ResponsibilityId = model.ResponsibilityId
            };
        }

        public static VacancyResponsibility Create(VacancyResponsibilityViewModel model)
        {
            return new VacancyResponsibility
            {
                Id = model.Id,
                VacancyId = model.VacancyId,
                ResponsibilityId = model.ResponsibilityId
            };
        }

        public void Update(VacancyResponsibilityBindingModel model)
        {
            if (model == null)
            {
                return;
            }
            VacancyId = model.VacancyId;
            ResponsibilityId = model.ResponsibilityId;
        }

        public VacancyResponsibilityViewModel GetViewModel => new()
        {
            Id = Id,
            VacancyId = VacancyId,
            ResponsibilityId = ResponsibilityId
        };
    }
}
