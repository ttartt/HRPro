using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;
using HRProDatabaseImplement.Models;
using HRproDatabaseImplement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRProDatabaseImplement.Implements
{
    public class VacancyResponsibilityStorage : IVacancyResponsibilityStorage
    {
        public List<VacancyResponsibilityViewModel> GetFullList()
        {
            using var context = new HRproDatabase();
            return context.VacancyResponsibilities
                .Select(x => x.GetViewModel)
                .ToList();
        }
        public List<VacancyResponsibilityViewModel> GetFilteredList(VacancyResponsibilitySearchModel model)
        {
            if (!model.Id.HasValue)
            {
                return new();
            }
            using var context = new HRproDatabase();
            return context.VacancyResponsibilities
                .Where(x => x.Id.Equals(model.Id))
                .Select(x => x.GetViewModel)
                .ToList();
        }
        public VacancyResponsibilityViewModel? GetElement(VacancyResponsibilitySearchModel model)
        {
            if (!model.Id.HasValue)
            {
                return null;
            }
            using var context = new HRproDatabase();
            return context.VacancyResponsibilities
                .FirstOrDefault(x => x.Id == model.Id)
                ?.GetViewModel;
        }
        public VacancyResponsibilityViewModel? Insert(VacancyResponsibilityBindingModel model)
        {
            var newVacancyResponsibility = VacancyResponsibility.Create(model);
            if (newVacancyResponsibility == null)
            {
                return null;
            }
            using var context = new HRproDatabase();
            context.VacancyResponsibilities.Add(newVacancyResponsibility);
            context.SaveChanges();
            return newVacancyResponsibility.GetViewModel;
        }
        public VacancyResponsibilityViewModel? Update(VacancyResponsibilityBindingModel model)
        {
            using var context = new HRproDatabase();
            var criterion = context.VacancyResponsibilities.FirstOrDefault(x => x.Id == model.Id);
            if (criterion == null)
            {
                return null;
            }
            criterion.Update(model);
            context.SaveChanges();
            return criterion.GetViewModel;
        }
        public VacancyResponsibilityViewModel? Delete(VacancyResponsibilityBindingModel model)
        {
            using var context = new HRproDatabase();
            var element = context.VacancyResponsibilities.FirstOrDefault(rec => rec.Id == model.Id);
            if (element != null)
            {
                context.VacancyResponsibilities.Remove(element);
                context.SaveChanges();
                return element.GetViewModel;
            }
            return null;
        }
    }
}
