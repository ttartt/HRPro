using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;
using HRproDatabaseImplement;
using HRProDatabaseImplement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (string.IsNullOrEmpty(model.Topic))
            {
                return new();
            }
            using var context = new HRproDatabase();
            return context.Meetings
                .Where(x => x.Topic.Contains(model.Topic))
                .Select(x => x.GetViewModel)
                .ToList();
        }
        public MeetingViewModel? GetElement(MeetingSearchModel model)
        {
            if (string.IsNullOrEmpty(model.Topic) && !model.Id.HasValue)
            {
                return null;
            }
            using var context = new HRproDatabase();
            return context.Meetings
                .FirstOrDefault(x => (!string.IsNullOrEmpty(model.Topic) && x.Topic == model.Topic) || (model.Id.HasValue && x.Id == model.Id))
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
            var criterion = context.Meetings.FirstOrDefault(x => x.Id == model.Id);
            if (criterion == null)
            {
                return null;
            }
            criterion.Update(model);
            context.SaveChanges();
            return criterion.GetViewModel;
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
