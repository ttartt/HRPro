using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;
using HRproDatabaseImplement.Models;
using Microsoft.EntityFrameworkCore;

namespace HRproDatabaseImplement.Implements
{
    public class ResumeStorage : IResumeStorage
    {
        public ResumeViewModel? Delete(ResumeBindingModel model)
        {
            using var context = new HRproDatabase();

            var element = context.Resumes
                .Include(x => x.Vacancy)
                .Include(x => x.User)
                .FirstOrDefault(rec => rec.Id == model.Id);

            if (element != null)
            {                
                context.Resumes.Remove(element);
                context.SaveChanges();

                return element.GetViewModel;
            }

            return null;
        }

        public ResumeViewModel? GetElement(ResumeSearchModel model)
        {            
            using var context = new HRproDatabase();
            if (model.VacancyId.HasValue && model.UserId.HasValue)
            {
                return context.Resumes
                    .Include(x => x.Vacancy)
                    .Include(x => x.User)
                    .FirstOrDefault(x => x.VacancyId == model.VacancyId && x.UserId == model.UserId)
                    ?.GetViewModel;
            }

            if (model.VacancyId.HasValue)
            {
                return context.Resumes
                    .Include(x => x.Vacancy)
                    .Include(x => x.User)
                    .FirstOrDefault(x => x.VacancyId == model.VacancyId)
                    ?.GetViewModel;
            }

            if (model.UserId.HasValue)
            {
                return context.Resumes
                    .Include(x => x.Vacancy)
                    .Include(x => x.User)
                    .FirstOrDefault(x => x.UserId == model.UserId)
                    ?.GetViewModel;
            }
            if (!model.Id.HasValue)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(model.Title))
            {
                return context.Resumes
                    .Include(x => x.Vacancy)
                    .Include(x => x.User)
                    .FirstOrDefault(x => x.Title == model.Title)
                    ?.GetViewModel;
            }

            return context.Resumes
                .Include(x => x.Vacancy)
                .Include(x => x.User)
                .FirstOrDefault(x => model.Id.HasValue && x.Id == model.Id)
                ?.GetViewModel;
        }

        public List<ResumeViewModel> GetFilteredList(ResumeSearchModel model)
        {
            if (model is null)
            {
                return new();
            }
            using var context = new HRproDatabase();
            if (model.VacancyId.HasValue)
            {
                return context.Resumes
                    .Include(x => x.Vacancy)
                    .Include(x => x.User)
                    .Where(x => x.VacancyId == model.VacancyId)
                    .ToList()
                    .Select(x => x.GetViewModel)
                    .ToList();
            }

            if (model.UserId.HasValue)
            {
                return context.Resumes
                    .Include(x => x.Vacancy)
                    .Include(x => x.User)
                    .Where(x => x.UserId == model.UserId)
                    .ToList()
                    .Select(x => x.GetViewModel)
                    .ToList();
            }

            if (!string.IsNullOrEmpty(model.Title))
            {
                return context.Resumes
                    .Include(x => x.Vacancy)
                    .Include(x => x.User)
                    .Where(x => x.Title == model.Title)
                    .ToList()
                    .Select(x => x.GetViewModel)
                    .ToList();
            }
            return context.Resumes
                .Include(x => x.Vacancy)
                .Include(x => x.User)
                .ToList()
                .Select(x => x.GetViewModel)  
                .ToList();
        }

        public List<ResumeViewModel> GetFullList()
        {
            using var context = new HRproDatabase();

            return context.Resumes
                .Include(x => x.Vacancy)
                .Include(x => x.User)
                .Select(x => x.GetViewModel).ToList();
        }

        public ResumeViewModel? Insert(ResumeBindingModel model)
        {
            var newResume = Resume.Create(model);

            if (newResume == null)
            {
                return null;
            }

            using var context = new HRproDatabase();

            context.Resumes.Add(newResume);
            context.SaveChanges();

            return context.Resumes
                .Include(x => x.Vacancy)
                .Include(x => x.User)
                .FirstOrDefault(x => x.Id == newResume.Id)
                ?.GetViewModel;
        }

        public ResumeViewModel? Update(ResumeBindingModel model)
        {
            using var context = new HRproDatabase();

            var resume = context.Resumes
                .Include(x => x.Vacancy)
                .Include(x => x.User)
                .FirstOrDefault(x => x.Id == model.Id);

            if (resume == null)
            {
                return null;
            }

            resume.Update(model);
            context.SaveChanges();

            return resume.GetViewModel;
        }
    }
}
