using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using HRproDatabaseImplement;
using HRproDatabaseImplement.Models;
using HRProDataModels.Models;
using Microsoft.IdentityModel.Tokens;
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
            Date = model.Date.ToUniversalTime();
            TimeFrom = model.TimeFrom.ToUniversalTime();
            TimeTo = model.TimeTo.ToUniversalTime();
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

        public void UpdateParticipants(HRproDatabase context, MeetingBindingModel model)
        {
            var existingParticipants = context.MeetingParticipants
                .Where(p => p.MeetingId == model.Id)
                .ToList();

            var participantsToRemove = existingParticipants
                .Where(ep => !model.SelectedParticipantIds.Contains(ep.UserId))
                .ToList();

            var participantsToAdd = model.SelectedParticipantIds
                .Where(id => !existingParticipants.Any(ep => ep.UserId == id))
                .Select(id => new MeetingParticipant
                {
                    MeetingId = model.Id,
                    UserId = id
                })
                .ToList();

            if (participantsToRemove.Any())
            {
                context.MeetingParticipants.RemoveRange(participantsToRemove);
            }

            if (participantsToAdd.Any())
            {
                var existingUserIds = context.Users
                    .Where(u => participantsToAdd.Select(p => p.UserId).Contains(u.Id))
                    .Select(u => u.Id)
                    .ToList();

                var validParticipants = participantsToAdd
                    .Where(p => existingUserIds.Contains(p.UserId))
                    .ToList();

                context.MeetingParticipants.AddRange(validParticipants);
            }

            context.SaveChanges();
        }
    }
}
