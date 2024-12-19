using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using HRproDatabaseImplement.Models;
using HRProDataModels.Models;
using System.ComponentModel.DataAnnotations;

namespace HRProDatabaseImplement.Models
{
    public class VacancyRequirement : IVacancyRequirement
    {
        [Required]
        public int VacancyId { get; set; }

        [Required]
        public int RequirementId { get; set; }

        public int Id { get; set; }

        /*public virtual Vacancy Vacancy { get; set; }
        public virtual Requirement Requirement { get; set; }*/

        public static VacancyRequirement? Create(VacancyRequirementBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new VacancyRequirement
            {
                Id = model.Id,
                VacancyId = model.VacancyId,
                RequirementId = model.RequirementId
            };
        }

        public static VacancyRequirement Create(VacancyRequirementViewModel model)
        {
            return new VacancyRequirement
            {
                Id = model.Id,
                VacancyId = model.VacancyId,
                RequirementId = model.RequirementId
            };
        }

        public void Update(VacancyRequirementBindingModel model)
        {
            if (model == null)
            {
                return;
            }
            VacancyId = model.VacancyId;
            RequirementId = model.RequirementId;
        }

        public VacancyRequirementViewModel GetViewModel => new()
        {
            Id = Id,
            VacancyId = VacancyId,
            RequirementId = RequirementId
        };
    }
}
