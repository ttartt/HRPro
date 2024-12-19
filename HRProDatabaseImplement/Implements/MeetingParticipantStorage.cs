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
            if (!model.Id.HasValue)
            {
                return new();
            }
            using var context = new HRproDatabase();
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
