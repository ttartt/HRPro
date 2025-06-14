﻿using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;
using HRproDatabaseImplement;
using HRProDatabaseImplement.Models;
using Microsoft.EntityFrameworkCore;

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
            if (!string.IsNullOrEmpty(model.GoogleEventId))
            {
                return context.Meetings
                .Where(x => x.GoogleEventId.Equals(model.GoogleEventId))
                .Select(x => x.GetViewModel)
                .ToList();
            }
            if (model.CompanyId.HasValue)
            {
                return context.Meetings
                .Where(x => x.CompanyId == model.CompanyId)
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
            if (model.Date.HasValue && model.TimeFrom.HasValue && model.TimeTo.HasValue)
            {
                var utcDate = model.Date.Value.ToUniversalTime();
                var utcTimeFrom = model.TimeFrom.Value.ToUniversalTime();
                var utcTimeTo = model.TimeTo.Value.ToUniversalTime();

                return context.Meetings
                    .FirstOrDefault(x => x.Date == utcDate &&
                                        x.TimeFrom == utcTimeFrom &&
                                        x.TimeTo == utcTimeTo)
                    ?.GetViewModel;
            }
            if (model.Id.HasValue)
            {
                return context.Meetings
                .FirstOrDefault(x => x.Id == model.Id)
                ?.GetViewModel;
            }
            return context.Meetings
                .FirstOrDefault(x => x.Id == model.Id)
                ?.GetViewModel;
        }
        public int? Insert(MeetingBindingModel model)
        {
            var newMeeting = Meeting.Create(model);
            if (newMeeting == null)
            {
                return null;
            }
            using var context = new HRproDatabase();
            context.Meetings.Add(newMeeting);
            context.SaveChanges();
            return newMeeting.Id;
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
            meeting.UpdateParticipants(context, model);
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
