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
    public class VacancyRequirementStorage : IVacancyRequirementStorage
    {
        public List<VacancyRequirementViewModel> GetFullList()
        {
            using var context = new HRproDatabase();
            return context.VacancyRequirements
                .Select(x => x.GetViewModel)
                .ToList();
        }
        public List<VacancyRequirementViewModel> GetFilteredList(VacancyRequirementSearchModel model)
        {
            if (!model.Id.HasValue)
            {
                return new();
            }
            using var context = new HRproDatabase();
            return context.VacancyRequirements
                .Where(x => x.Id.Equals(model.Id))
                .Select(x => x.GetViewModel)
                .ToList();
        }
        public VacancyRequirementViewModel? GetElement(VacancyRequirementSearchModel model)
        {
            if (!model.Id.HasValue)
            {
                return null;
            }
            using var context = new HRproDatabase();
            return context.VacancyRequirements
                .FirstOrDefault(x => x.Id == model.Id)
                ?.GetViewModel;
        }
        public VacancyRequirementViewModel? Insert(VacancyRequirementBindingModel model)
        {
            var newVacancyRequirement = VacancyRequirement.Create(model);
            if (newVacancyRequirement == null)
            {
                return null;
            }
            using var context = new HRproDatabase();
            context.VacancyRequirements.Add(newVacancyRequirement);
            context.SaveChanges();
            return newVacancyRequirement.GetViewModel;
        }
        public VacancyRequirementViewModel? Update(VacancyRequirementBindingModel model)
        {
            using var context = new HRproDatabase();
            var criterion = context.VacancyRequirements.FirstOrDefault(x => x.Id == model.Id);
            if (criterion == null)
            {
                return null;
            }
            criterion.Update(model);
            context.SaveChanges();
            return criterion.GetViewModel;
        }
        public VacancyRequirementViewModel? Delete(VacancyRequirementBindingModel model)
        {
            using var context = new HRproDatabase();
            var element = context.VacancyRequirements.FirstOrDefault(rec => rec.Id == model.Id);
            if (element != null)
            {
                context.VacancyRequirements.Remove(element);
                context.SaveChanges();
                return element.GetViewModel;
            }
            return null;
        }
    }
}
