using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using HRproDatabaseImplement.Models;
using HRProDataModels.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace HRProDatabaseImplement.Models
{
    [DataContract]
    public class Meeting : IMeetingModel
    {
        [DataMember]
        public int? ResumeId { get; set; }

        [Required]
        [DataMember]
        public string Topic { get; set; } = string.Empty;

        [Required]
        [DataMember]
        public DateTime Date { get; set; }

        [Required]
        [DataMember]
        public DateTime TimeFrom { get; set; }

        [Required]
        [DataMember]
        public DateTime TimeTo { get; set; }

        [DataMember]
        public int? VacancyId { get; set; }

        [DataMember]
        public int? CompanyId { get; set; }

        [DataMember]
        public string? Place { get; set; }

        [DataMember]
        public string? Comment { get; set; }

        [DataMember]
        public string? GoogleEventId { get; set; }

        public int Id { get; set; }

        public virtual Vacancy Vacancy { get; set; }
        public virtual Resume Resume { get; set; }
        public virtual Company Company { get; set; }

        public static Meeting? Create(MeetingBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new Meeting()
            {
                Id = model.Id,
                ResumeId = model.ResumeId,
                Topic = model.Topic,
                Date = model.Date.ToUniversalTime(),
                TimeFrom = model.TimeFrom.ToUniversalTime(),
                TimeTo = model.TimeTo.ToUniversalTime(),
                VacancyId = model.VacancyId,
                Place = model.Place,
                Comment = model.Comment,
                CompanyId = model.CompanyId,
                GoogleEventId = model.GoogleEventId
            };
        }

        public static Meeting Create(MeetingViewModel model)
        {
            return new Meeting
            {
                Id = model.Id,
                ResumeId = model.ResumeId,
                Topic = model.Topic,
                Date = model.Date,
                TimeFrom = model.TimeFrom,
                TimeTo = model.TimeTo,
                VacancyId = model.VacancyId,
                Place = model.Place,
                Comment = model.Comment,
                CompanyId = model.CompanyId,
                GoogleEventId = model.GoogleEventId
            };
        }

        public void Update(MeetingBindingModel model)
        {
            if (model == null)
            {
                return;
            }
            Topic = model.Topic;
            Date = model.Date;
            TimeFrom = model.TimeFrom;
            TimeTo = model.TimeTo;
            VacancyId = model.VacancyId;
            Place = model.Place;
            Comment = model.Comment;
            GoogleEventId = model.GoogleEventId;
        }

        public MeetingViewModel GetViewModel => new()
        {
            Id = Id,
            ResumeId = ResumeId,
            Topic = Topic,
            Date = Date,
            TimeFrom = TimeFrom,
            TimeTo = TimeTo,
            VacancyId = VacancyId,
            Place = Place,
            Comment = Comment,
            CompanyId = CompanyId,
            GoogleEventId = GoogleEventId
        };
    }
}
