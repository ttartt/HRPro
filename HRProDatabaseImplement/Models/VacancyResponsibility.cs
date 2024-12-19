using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using HRproDatabaseImplement.Models;
using HRProDataModels.Models;
using System.ComponentModel.DataAnnotations;

namespace HRProDatabaseImplement.Models
{
    public class VacancyResponsibility : IVacancyResponsibility
    {
        [Required]
        public int VacancyId { get; set; }
        [Required]
        public int ResponsibilityId { get; set; }

        public int Id { get; set; }

        /*public virtual Vacancy Vacancy { get; set; }
        public virtual Responsibility Responsibility { get; set; }*/

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
