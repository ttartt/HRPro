using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using HRProDataModels.Enums;
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
        public string? LastWorkPlace { get; set; } = string.Empty;
        [DataMember]
        public string? LastJobTitle { get; set; } = string.Empty;
        [DataMember]
        public ResumeSourceEnum? Source { get; set; }
        [DataMember]
        public string? Age { get; set; } = string.Empty;
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
                LastWorkPlace = model.LastWorkPlace,
                LastJobTitle = model.LastJobTitle,
                Age = model.Age,
                CreatedAt = DateTime.Now.ToUniversalTime(),
                Salary = model.Salary,
                CandidateInfo = model.CandidateInfo,
                CompanyId = model.CompanyId,
                Source = model.Source
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
                LastWorkPlace = model.LastWorkPlace,
                LastJobTitle = model.LastJobTitle,
                Age = model.Age,
                CreatedAt = DateTime.Now.ToUniversalTime(),
                Salary = model.Salary,
                CandidateInfo = model.CandidateInfo,
                CompanyId = model.CompanyId,
                Source = model.Source
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
            LastWorkPlace = model.LastWorkPlace;
            LastJobTitle = model.LastJobTitle;
            Age = model.Age;
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
            LastWorkPlace = LastWorkPlace,
            LastJobTitle = LastJobTitle,
            Age = Age,
            CreatedAt = DateTime.Now.ToUniversalTime(),
            Salary = Salary,
            CandidateInfo = CandidateInfo,
            CompanyId = CompanyId,
            Source = Source
        };
    }
}
