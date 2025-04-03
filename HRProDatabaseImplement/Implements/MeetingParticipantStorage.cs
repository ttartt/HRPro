using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;
using HRproDatabaseImplement;
using HRProDatabaseImplement.Models;

namespace HRProDatabaseImplement.Implements
{
    public class MeetingParticipantStorage : IMeetingParticipantStorage
    {
        public List<MeetingParticipantViewModel> GetFullList()
        {
            using var context = new HRproDatabase();
            return context.MeetingParticipants
                .Select(x => x.GetViewModel)
                .ToList();
        }
        public List<MeetingParticipantViewModel> GetFilteredList(MeetingParticipantSearchModel model)
        {
            using var context = new HRproDatabase();
            if (model.UserId.HasValue)
            {
                return context.MeetingParticipants
                .Where(x => x.UserId.Equals(model.UserId))
                .Select(x => x.GetViewModel)
                .ToList();
            }
            if (model.MeetingId.HasValue)
            {
                return context.MeetingParticipants
                .Where(x => x.MeetingId.Equals(model.MeetingId))
                .Select(x => x.GetViewModel)
                .ToList();
            }
            return context.MeetingParticipants
                .Where(x => x.Id.Equals(model.Id))
                .Select(x => x.GetViewModel)
                .ToList();
        }
        public MeetingParticipantViewModel? GetElement(MeetingParticipantSearchModel model)
        {
            if (!model.Id.HasValue)
            {
                return null;
            }
            using var context = new HRproDatabase();
            return context.MeetingParticipants
                .FirstOrDefault(x => x.Id == model.Id)
                ?.GetViewModel;
        }
        public MeetingParticipantViewModel? Insert(MeetingParticipantBindingModel model)
        {
            var newMeetingParticipant = MeetingParticipant.Create(model);
            if (newMeetingParticipant == null)
            {
                return null;
            }
            using var context = new HRproDatabase();
            context.MeetingParticipants.Add(newMeetingParticipant);
            context.SaveChanges();
            return newMeetingParticipant.GetViewModel;
        }
        public MeetingParticipantViewModel? Update(MeetingParticipantBindingModel model)
        {
            using var context = new HRproDatabase();
            var criterion = context.MeetingParticipants.FirstOrDefault(x => x.Id == model.Id);
            if (criterion == null)
            {
                return null;
            }
            criterion.Update(model);
            context.SaveChanges();
            return criterion.GetViewModel;
        }
        public MeetingParticipantViewModel? Delete(MeetingParticipantBindingModel model)
        {
            using var context = new HRproDatabase();
            var element = context.MeetingParticipants.FirstOrDefault(rec => rec.Id == model.Id);
            if (element != null)
            {
                context.MeetingParticipants.Remove(element);
                context.SaveChanges();
                return element.GetViewModel;
            }
            return null;
        }
    }
}
