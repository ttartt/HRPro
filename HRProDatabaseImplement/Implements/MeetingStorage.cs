using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;
using HRproDatabaseImplement;
using HRProDatabaseImplement.Models;

namespace HRProDatabaseImplement.Implements
{
    public class MeetingStorage : IMeetingStorage
    {
        public List<MeetingViewModel> GetFullList()
        {
            using var context = new HRproDatabase();
            return context.Meetings
                .Select(x => x.GetViewModel)
                .ToList();
        }
        public List<MeetingViewModel> GetFilteredList(MeetingSearchModel model)
        {
            using var context = new HRproDatabase();
            if (model.Id.HasValue)
            {
                return context.Meetings
                .Where(x => x.Id == model.Id)
                .Select(x => x.GetViewModel)
                .ToList();
            }
            return context.Meetings
                .Where(x => x.Topic.Contains(model.Topic))
                .Select(x => x.GetViewModel)
                .ToList();
        }
        public MeetingViewModel? GetElement(MeetingSearchModel model)
        {
            using var context = new HRproDatabase();
            return context.Meetings
                .FirstOrDefault(x => x.Id == model.Id)
                ?.GetViewModel;
        }
        public MeetingViewModel? Insert(MeetingBindingModel model)
        {
            var newMeeting = Meeting.Create(model);
            if (newMeeting == null)
            {
                return null;
            }
            using var context = new HRproDatabase();
            context.Meetings.Add(newMeeting);
            context.SaveChanges();
            return newMeeting.GetViewModel;
        }
        public MeetingViewModel? Update(MeetingBindingModel model)
        {
            using var context = new HRproDatabase();
            var meeting = context.Meetings.FirstOrDefault(x => x.Id == model.Id);
            if (meeting == null)
            {
                return null;
            }
            meeting.Update(model);
            context.SaveChanges();
            return meeting.GetViewModel;
        }
        public MeetingViewModel? Delete(MeetingBindingModel model)
        {
            using var context = new HRproDatabase();
            var element = context.Meetings.FirstOrDefault(rec => rec.Id == model.Id);
            if (element != null)
            {
                context.Meetings.Remove(element);
                context.SaveChanges();
                return element.GetViewModel;
            }
            return null;
        }
    }
}
