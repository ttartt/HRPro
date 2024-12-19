using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;
using HRproDatabaseImplement.Models;
using Microsoft.EntityFrameworkCore;

namespace HRproDatabaseImplement.Implements
{
    public class VacancyStorage : IVacancyStorage
    {
        public VacancyViewModel? Delete(VacancyBindingModel model)
        {
            using var context = new HRproDatabase();

            var element = context.Vacancies
                .Include(x => x.Company)
                .FirstOrDefault(rec => rec.Id == model.Id);

            if (element != null)
            {                
                context.Vacancies.Remove(element);
                context.SaveChanges();

                return element.GetViewModel;
            }

            return null;
        }

        public VacancyViewModel? GetElement(VacancySearchModel model)
        {
            if (!model.Id.HasValue)
            {
                return null;
            }
            using var context = new HRproDatabase();
            if (model.CompanyId.HasValue && !string.IsNullOrEmpty(model.JobTitle))
            {
                return context.Vacancies
                    .Include(x => x.Company)
                    .FirstOrDefault(x => x.CompanyId == model.CompanyId && x.JobTitle.Equals(model.JobTitle))
                    ?.GetViewModel;
            }
            if (!string.IsNullOrEmpty(model.Tags))
            {
                return context.Vacancies
                    .Include(x => x.Company)
                    .FirstOrDefault(x => x.Tags.Contains(model.Tags))
                    ?.GetViewModel;
            }
            return context.Vacancies
                .Include(x => x.Company)
                .FirstOrDefault(x => model.Id.HasValue && x.Id == model.Id)
                ?.GetViewModel;
        }

        public List<VacancyViewModel> GetFilteredList(VacancySearchModel model)
        {
            if (model is null)
            {
                return new();
            }
            using var context = new HRproDatabase();
            if (model.CompanyId.HasValue && !string.IsNullOrEmpty(model.JobTitle))
            {
                return context.Vacancies
                    .Include(x => x.Company)
                    .Where(x => x.CompanyId == model.CompanyId && x.JobTitle.Equals(model.JobTitle))
                    .ToList()
                    .Select(x => x.GetViewModel)
                    .ToList();
            }
            if (model.CompanyId.HasValue)
            {
                return context.Vacancies
                    .Include(x => x.Company)
                    .Where(x => x.CompanyId == model.CompanyId)
                    .ToList()
                    .Select(x => x.GetViewModel)
                    .ToList();
            }
            if (!string.IsNullOrEmpty(model.Tags) && model.Status.HasValue)
            {
                var tags = model.Tags.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(t => t.ToLowerInvariant()).ToArray();
                return context.Vacancies
                  .Include(x => x.Company)
                  .Where(x => tags.Any(tag => x.Tags.Contains(tag)) && x.Status == model.Status)
                  .ToList()
                  .Select(x => x.GetViewModel)
                  .ToList();
            }
            return context.Vacancies
                .Include(x => x.Company)
                .ToList()
                .Select(x => x.GetViewModel)
                .ToList();
        }

        public List<VacancyViewModel> GetFullList()
        {
            using var context = new HRproDatabase();

            return context.Vacancies
                .Include(x => x.Company)
                .Select(x => x.GetViewModel).ToList();
        }

        public VacancyViewModel? Insert(VacancyBindingModel model)
        {
            var newVacancy = Vacancy.Create(model);

            if (newVacancy == null)
            {
                return null;
            }

            using var context = new HRproDatabase();

            context.Vacancies.Add(newVacancy);
            context.SaveChanges();

            return context.Vacancies
                .Include(x => x.Company)
                .FirstOrDefault(x => x.Id == newVacancy.Id)
                ?.GetViewModel;
        }

        public VacancyViewModel? Update(VacancyBindingModel model)
        {
            using var context = new HRproDatabase();

            var vacancy = context.Vacancies
                .Include(x => x.Company)
                .FirstOrDefault(x => x.Id == model.Id);

            if (vacancy == null)
            {
                return null;
            }

            vacancy.Update(model);
            context.SaveChanges();

            return vacancy.GetViewModel;
        }
    }
}
