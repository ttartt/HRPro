using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using HRProDataModels.Enums;
using HRProDataModels.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRproDatabaseImplement.Models
{
    public class Vacancy : IVacancyModel
    {
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public string JobTitle { get; set; } = string.Empty;
        [Required]
        public JobTypeEnum JobType { get; set; }

        public string? Salary { get; set; }

        public string? Description { get; set; }
        [Required]
        public VacancyStatusEnum Status { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }

        public string? Tags { get; set; }

        public int Id { get; set; }
        public virtual Company Company { get; set; }

        public static Vacancy? Create(VacancyBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new Vacancy()
            {
                Id = model.Id,
                CompanyId = model.CompanyId,
                JobTitle = model.JobTitle,
                JobType = model.JobType,
                Salary = model.Salary,
                Description = model.Description,
                Status = model.Status,
                CreatedAt = DateTime.Now.ToUniversalTime(),
                Tags = model.Tags
            };
        }
        public static Vacancy Create(VacancyViewModel model)
        {
            return new Vacancy
            {
                Id = model.Id,
                CompanyId = model.CompanyId,
                JobTitle = model.JobTitle,
                JobType = model.JobType,
                Salary = model.Salary,
                Description = model.Description,
                Status = model.Status,
                CreatedAt = DateTime.Now.ToUniversalTime(),
                Tags = model.Tags
            };
        }
        public void Update(VacancyBindingModel model)
        {
            if (model == null)
            {
                return;
            }
            CompanyId = model.CompanyId;
            JobTitle = model.JobTitle;
            JobType = model.JobType;
            Salary = model.Salary;
            Description = model.Description;
            Status = model.Status;
            Tags = model.Tags;
        }
        public VacancyViewModel GetViewModel => new()
        {
            Id = Id,
            CompanyId = CompanyId,
            JobTitle = JobTitle,
            JobType = JobType,
            Salary = Salary,
            Description = Description,
            Status = Status,
            CreatedAt = CreatedAt,
            Tags = Tags
        };
    }
}
