using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using HRproDatabaseImplement.Models;
using HRProDataModels.Enums;
using HRProDataModels.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace HRProDatabaseImplement.Models
{
    [DataContract]
    public class ExternalVacancy
    {
        [DataMember]
        public int CompanyId { get; set; }
        [DataMember]
        public string? Title { get; set; }
        [DataMember]
        public string? Salary { get; set; }
        [DataMember]
        public string? City { get; set; }
        [DataMember]
        public string? Url { get; set; }
        [DataMember]
        public int Id { get; set; }
        public virtual Company Company { get; set; }

        public static ExternalVacancy? Create(ExternalVacancyBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new ExternalVacancy()
            {
                Id = model.Id,
                CompanyId = model.CompanyId,
                Title = model.Title,
                City = model.City,
                Salary = model.Salary,
                Url = model.Url
            };
        }
        public static ExternalVacancy Create(ExternalVacancyViewModel model)
        {
            return new ExternalVacancy
            {
                Id = model.Id,
                CompanyId = model.CompanyId,
                Title = model.Title,
                City = model.City,
                Salary = model.Salary,
                Url = model.Url
            };
        }
        public void Update(ExternalVacancyBindingModel model)
        {
            if (model == null)
            {
                return;
            }
            CompanyId = model.CompanyId;
            Title = model.Title;
            City = model.City;
            Salary = model.Salary;
            Url = model.Url;
        }
        public ExternalVacancyViewModel GetViewModel => new()
        {
            Id = Id,
            CompanyId = CompanyId,
            Title = Title,
            City = City,
            Salary = Salary,
            Url = Url
        };
    }
}
