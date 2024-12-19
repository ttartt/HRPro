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
    public class ResponsibilityStorage : IResponsibilityStorage
    {
        public List<ResponsibilityViewModel> GetFullList()
        {
            using var context = new HRproDatabase();
            return context.Responsibilities
                .Select(x => x.GetViewModel)
                .ToList();
        }
        public List<ResponsibilityViewModel> GetFilteredList(ResponsibilitySearchModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                return new();
            }
            using var context = new HRproDatabase();
            return context.Responsibilities
                .Where(x => x.Name.Contains(model.Name))
                .Select(x => x.GetViewModel)
                .ToList();
        }
        public ResponsibilityViewModel? GetElement(ResponsibilitySearchModel model)
        {
            if (string.IsNullOrEmpty(model.Name) && !model.Id.HasValue)
            {
                return null;
            }
            using var context = new HRproDatabase();
            return context.Responsibilities
                .FirstOrDefault(x => (!string.IsNullOrEmpty(model.Name) && x.Name == model.Name) || (model.Id.HasValue && x.Id == model.Id))
                ?.GetViewModel;
        }
        public ResponsibilityViewModel? Insert(ResponsibilityBindingModel model)
        {
            var newResponsibility = Responsibility.Create(model);
            if (newResponsibility == null)
            {
                return null;
            }
            using var context = new HRproDatabase();
            context.Responsibilities.Add(newResponsibility);
            context.SaveChanges();
            return newResponsibility.GetViewModel;
        }
        public ResponsibilityViewModel? Update(ResponsibilityBindingModel model)
        {
            using var context = new HRproDatabase();
            var criterion = context.Responsibilities.FirstOrDefault(x => x.Id == model.Id);
            if (criterion == null)
            {
                return null;
            }
            criterion.Update(model);
            context.SaveChanges();
            return criterion.GetViewModel;
        }
        public ResponsibilityViewModel? Delete(ResponsibilityBindingModel model)
        {
            using var context = new HRproDatabase();
            var element = context.Responsibilities.FirstOrDefault(rec => rec.Id == model.Id);
            if (element != null)
            {
                context.Responsibilities.Remove(element);
                context.SaveChanges();
                return element.GetViewModel;
            }
            return null;
        }
    }
}
