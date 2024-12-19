using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using HRProDataModels.Enums;
using HRProDataModels.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRproDatabaseImplement.Models
{
    public class Resume : IResumeModel
    {
        [Required]
        public int VacancyId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Experience { get; set; } = string.Empty;
        [Required]
        public string Education { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required]
        public string Skills { get; set; } = string.Empty;
        [Required]
        public ResumeStatusEnum Status { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }

        public int Id { get; set; }

        public virtual Vacancy Vacancy { get; set; }
        public virtual User User { get; set; }

        public static Resume? Create(ResumeBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new Resume()
            {
                Id = model.Id,
                VacancyId = model.VacancyId,
                UserId = model.UserId,
                Title = model.Title,
                Experience = model.Experience,
                Education = model.Education,
                Description = model.Description,
                Skills = model.Skills,
                Status = model.Status,
                CreatedAt = DateTime.Now.ToUniversalTime().AddHours(4)
            };
        }
        public static Resume Create(ResumeViewModel model)
        {
            return new Resume
            {
                Id = model.Id,
                VacancyId = model.VacancyId,
                UserId = model.UserId,
                Title = model.Title,
                Experience = model.Experience,
                Education = model.Education,
                Description = model.Description,
                Skills = model.Skills,
                Status = model.Status,
                CreatedAt = DateTime.Now.ToUniversalTime().AddHours(4)
            };
        }
        public void Update(ResumeBindingModel model)
        {
            if (model == null)
            {
                return;
            }
            VacancyId = model.VacancyId;
            UserId = model.UserId;
            Title = model.Title;
            Experience = model.Experience;
            Education = model.Education;
            Description = model.Description;
            Skills = model.Skills;
            Status = model.Status;
        }
        public ResumeViewModel GetViewModel => new()
        {
            Id = Id,
            VacancyId = VacancyId,
            UserId = UserId,
            Title = Title,
            Experience = Experience,
            Education = Education,
            Description = Description,
            Skills = Skills,
            Status = Status,
            CreatedAt = CreatedAt
        };
    }
}
