using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using HRproDatabaseImplement.Models;
using HRProDataModels.Models;
using System.ComponentModel.DataAnnotations;

namespace HRProDatabaseImplement.Models
{
    public class MeetingParticipant : IMeetingParticipantModel
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int MeetingId { get; set; }

        public int Id { get; set; }

        /*public virtual Meeting Meeting { get; set; }
        public virtual User User { get; set; }*/

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
