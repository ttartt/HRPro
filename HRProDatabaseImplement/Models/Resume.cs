using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using HRProDataModels.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace HRproDatabaseImplement.Models
{
    [DataContract]
    public class Resume : IResumeModel
    {
        [DataMember]
        public int? VacancyId { get; set; }
        [DataMember]
        public int? CompanyId { get; set; }
        [DataMember]
        public string? Title { get; set; } = string.Empty;
        [DataMember]
        public string? City { get; set; } = string.Empty;
        [DataMember]
        public string? Url { get; set; } = string.Empty;
        [DataMember]
        public string? Experience { get; set; } = string.Empty;
        [DataMember]
        public string? Education { get; set; } = string.Empty;
        [DataMember]
        public string? Description { get; set; }
        [DataMember]
        public string? Skills { get; set; } = string.Empty;
        [Required]
        [DataMember]
        public DateTime CreatedAt { get; set; }
        [DataMember]
        public string? Salary { get; set; } = string.Empty;
        [DataMember]
        public string? CandidateInfo { get; set; } = string.Empty;
        [DataMember]
        public int Id { get; set; }

        public virtual Vacancy Vacancy { get; set; }
        public virtual Company Company { get; set; }

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
                Url = model.Url,
                Title = model.Title,
                City = model.City,
                Experience = model.Experience,
                Education = model.Education,
                Description = model.Description,
                Skills = model.Skills,
                CreatedAt = DateTime.Now.ToUniversalTime(),
                Salary = model.Salary,
                CandidateInfo = model.CandidateInfo,
                CompanyId = model.CompanyId
            };
        }
        public static Resume Create(ResumeViewModel model)
        {
            return new Resume
            {
                Id = model.Id,
                VacancyId = model.VacancyId,
                Url = model.Url,
                Title = model.Title,
                City = model.City,
                Experience = model.Experience,
                Education = model.Education,
                Description = model.Description,
                Skills = model.Skills,
                CreatedAt = DateTime.Now.ToUniversalTime(),
                Salary = model.Salary,
                CandidateInfo = model.CandidateInfo,
                CompanyId = model.CompanyId
            };
        }
        public void Update(ResumeBindingModel model)
        {
            if (model == null)
            {
                return;
            }
            Title = model.Title;
            City = model.City;
            Url = model.Url;
            Experience = model.Experience;
            Education = model.Education;
            Description = model.Description;
            Skills = model.Skills;
            Salary = model.Salary;
            CandidateInfo = model.CandidateInfo;
        }
        public ResumeViewModel GetViewModel => new()
        {
            Id = Id,
            VacancyId = VacancyId,
            Url = Url,
            Title = Title,
            City = City,
            Experience = Experience,
            Education = Education,
            Description = Description,
            Skills = Skills,
            CreatedAt = DateTime.Now.ToUniversalTime(),
            Salary = Salary,
            CandidateInfo = CandidateInfo,
            CompanyId = CompanyId
        };
    }
}