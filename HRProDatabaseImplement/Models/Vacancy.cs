using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using HRProDataModels.Enums;
using HRProDataModels.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace HRproDatabaseImplement.Models
{
    [DataContract]
    public class Vacancy : IVacancyModel
    {
        [DataMember]
        [Required]
        public int CompanyId { get; set; }
        [DataMember]
        [Required]
        public string JobTitle { get; set; } = string.Empty;
        [DataMember]
        [Required]
        public JobTypeEnum JobType { get; set; }
        [DataMember]
        public string? Salary { get; set; }
        [DataMember]
        public string? Description { get; set; }
        [DataMember]
        [Required]
        public VacancyStatusEnum Status { get; set; }
        [DataMember]
        [Required]
        public DateTime CreatedAt { get; set; }
        [DataMember]
        public string? Tags { get; set; }
        [DataMember]
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
