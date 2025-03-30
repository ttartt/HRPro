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
        [Required]
        [DataMember]
        public int CandidateId { get; set; }

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

        [Required]
        [DataMember]
        public int VacancyId { get; set; }

        [Required]
        [DataMember]
        public string Place { get; set; } = string.Empty;

        public string? Comment { get; set; }

        public int Id { get; set; }

        public virtual Candidate Candidate { get; set; }
        public virtual Vacancy Vacancy { get; set; }

        public static Meeting? Create(MeetingBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new Meeting()
            {
                Id = model.Id,
                CandidateId = model.CandidateId,
                Topic = model.Topic,
                Date = model.Date.ToUniversalTime().AddHours(4),
                TimeFrom = model.TimeFrom.ToUniversalTime().AddHours(4),
                TimeTo = model.TimeTo.ToUniversalTime().AddHours(4),
                VacancyId = model.VacancyId,
                Place = model.Place,
                Comment = model.Comment
            };
        }

        public static Meeting Create(MeetingViewModel model)
        {
            return new Meeting
            {
                Id = model.Id,
                CandidateId = model.CandidateId,
                Topic = model.Topic,
                Date = model.Date,
                TimeFrom = model.TimeFrom,
                TimeTo = model.TimeTo,
                VacancyId = model.VacancyId,
                Place = model.Place,
                Comment = model.Comment
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
        }

        public MeetingViewModel GetViewModel => new()
        {
            Id = Id,
            CandidateId = CandidateId,
            Topic = Topic,
            Date = Date,
            TimeFrom = TimeFrom,
            TimeTo = TimeTo,
            VacancyId = VacancyId,
            Place = Place,
            Comment = Comment
        };
    }
}
