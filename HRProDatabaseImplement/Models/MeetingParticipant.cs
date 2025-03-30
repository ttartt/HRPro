using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using HRProDataModels.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace HRProDatabaseImplement.Models
{
    [DataContract]
    public class MeetingParticipant : IMeetingParticipantModel
    {
        [Required]
        [DataMember]
        public int UserId { get; set; }

        [Required]
        [DataMember]
        public int MeetingId { get; set; }
        [DataMember]
        public int Id { get; set; }

        public static MeetingParticipant? Create(MeetingParticipantBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new MeetingParticipant
            {
                Id = model.Id,
                UserId = model.UserId,
                MeetingId = model.MeetingId
            };
        }

        public static MeetingParticipant Create(MeetingParticipantViewModel model)
        {
            return new MeetingParticipant
            {
                Id = model.Id,
                UserId = model.UserId,
                MeetingId = model.MeetingId
            };
        }

        public void Update(MeetingParticipantBindingModel model)
        {
            if (model == null)
            {
                return;
            }
            UserId = model.UserId;
            MeetingId = model.MeetingId;
        }

        public MeetingParticipantViewModel GetViewModel => new()
        {
            Id = Id,
            UserId = UserId,
            MeetingId = MeetingId
        };
    }
}
