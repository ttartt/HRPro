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
    public class RequirementStorage : IRequirementStorage
    {
        public List<RequirementViewModel> GetFullList()
        {
            using var context = new HRproDatabase();
            return context.Requirements
                .Select(x => x.GetViewModel)
                .ToList();
        }
        public List<RequirementViewModel> GetFilteredList(RequirementSearchModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                return new();
            }
            using var context = new HRproDatabase();
            return context.Requirements
                .Where(x => x.Name.Contains(model.Name))
                .Select(x => x.GetViewModel)
                .ToList();
        }
        public RequirementViewModel? GetElement(RequirementSearchModel model)
        {
            if (string.IsNullOrEmpty(model.Name) && !model.Id.HasValue)
            {
                return null;
            }
            using var context = new HRproDatabase();
            return context.Requirements
                .FirstOrDefault(x => (!string.IsNullOrEmpty(model.Name) && x.Name == model.Name) || (model.Id.HasValue && x.Id == model.Id))
                ?.GetViewModel;
        }
        public int? Insert(RequirementBindingModel model)
        {
            var newRequirement = Requirement.Create(model);
            if (newRequirement == null)
            {
                return null;
            }
            using var context = new HRproDatabase();
            context.Requirements.Add(newRequirement);
            context.SaveChanges();
            return newRequirement.Id;
        }
        public RequirementViewModel? Update(RequirementBindingModel model)
        {
            using var context = new HRproDatabase();
            var criterion = context.Requirements.FirstOrDefault(x => x.Id == model.Id);
            if (criterion == null)
            {
                return null;
            }
            criterion.Update(model);
            context.SaveChanges();
            return criterion.GetViewModel;
        }
        public RequirementViewModel? Delete(RequirementBindingModel model)
        {
            using var context = new HRproDatabase();
            var element = context.Requirements.FirstOrDefault(rec => rec.Id == model.Id);
            if (element != null)
            {
                context.Requirements.Remove(element);
                context.SaveChanges();
                return element.GetViewModel;
            }
            return null;
        }
    }
}
