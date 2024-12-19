using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;
using HRproDatabaseImplement;
using HRProDatabaseImplement.Models;

namespace HRProDatabaseImplement.Implements
{
    public class CandidateStorage : ICandidateStorage
    {
        public List<CandidateViewModel> GetFullList()
        {
            using var context = new HRproDatabase();
            return context.Candidates
                .Select(x => x.GetViewModel)
                .ToList();
        }
        public List<CandidateViewModel> GetFilteredList(CandidateSearchModel model)
        {
            if (string.IsNullOrEmpty(model.FIO))
            {
                return new();
            }
            using var context = new HRproDatabase();
            return context.Candidates
                .Where(x => x.FIO.Contains(model.FIO))
                .Select(x => x.GetViewModel)
                .ToList();
        }
        public CandidateViewModel? GetElement(CandidateSearchModel model)
        {
            if (string.IsNullOrEmpty(model.FIO) && !model.Id.HasValue)
            {
                return null;
            }
            using var context = new HRproDatabase();
            return context.Candidates
                .FirstOrDefault(x => (!string.IsNullOrEmpty(model.FIO) && x.FIO == model.FIO) || (model.Id.HasValue && x.Id == model.Id))
                ?.GetViewModel;
        }
        public CandidateViewModel? Insert(CandidateBindingModel model)
        {
            var newCandidate = Candidate.Create(model);
            if (newCandidate == null)
            {
                return null;
            }
            using var context = new HRproDatabase();
            context.Candidates.Add(newCandidate);
            context.SaveChanges();
            return newCandidate.GetViewModel;
        }
        public CandidateViewModel? Update(CandidateBindingModel model)
        {
            using var context = new HRproDatabase();
            var criterion = context.Candidates.FirstOrDefault(x => x.Id == model.Id);
            if (criterion == null)
            {
                return null;
            }
            criterion.Update(model);
            context.SaveChanges();
            return criterion.GetViewModel;
        }
        public CandidateViewModel? Delete(CandidateBindingModel model)
        {
            using var context = new HRproDatabase();
            var element = context.Candidates.FirstOrDefault(rec => rec.Id == model.Id);
            if (element != null)
            {
                context.Candidates.Remove(element);
                context.SaveChanges();
                return element.GetViewModel;
            }
            return null;
        }
    }
}
