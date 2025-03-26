using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;
using HRproDatabaseImplement;
using HRProDatabaseImplement.Models;

namespace HRProDatabaseImplement.Implements
{
    public class TemplateStorage : ITemplateStorage
    {
        public List<TemplateViewModel> GetFullList()
        {
            using var context = new HRproDatabase();
            return context.Templates
                .Select(x => x.GetViewModel)
                .ToList();
        }
        public List<TemplateViewModel> GetFilteredList(TemplateSearchModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                return new();
            }
            using var context = new HRproDatabase();
            return context.Templates
                .Where(x => x.Name.Contains(model.Name))
                .Select(x => x.GetViewModel)
                .ToList();
        }
        public TemplateViewModel? GetElement(TemplateSearchModel model)
        {
            using var context = new HRproDatabase();
            if (model.Id.HasValue)
            {
                return context.Templates
                .FirstOrDefault(x => x.Id == model.Id)
                ?.GetViewModel;
            }
            if (string.IsNullOrEmpty(model.Name) && !model.Id.HasValue)
            {
                return null;
            }
            
            return context.Templates
                .FirstOrDefault(x => (!string.IsNullOrEmpty(model.Name) && x.Name == model.Name) || (model.Id.HasValue && x.Id == model.Id))
                ?.GetViewModel;
        }
        public int? Insert(TemplateBindingModel model)
        {
            var newTemplate = Template.Create(model);
            if (newTemplate == null)
            {
                return null;
            }
            using var context = new HRproDatabase();
            context.Templates.Add(newTemplate);
            context.SaveChanges();
            return newTemplate.Id;
        }
        public TemplateViewModel? Update(TemplateBindingModel model)
        {
            using var context = new HRproDatabase();
            var criterion = context.Templates.FirstOrDefault(x => x.Id == model.Id);
            if (criterion == null)
            {
                return null;
            }
            criterion.Update(model);
            context.SaveChanges();
            return criterion.GetViewModel;
        }
        public TemplateViewModel? Delete(TemplateBindingModel model)
        {
            using var context = new HRproDatabase();
            var element = context.Templates.FirstOrDefault(rec => rec.Id == model.Id);
            if (element != null)
            {
                context.Templates.Remove(element);
                context.SaveChanges();
                return element.GetViewModel;
            }
            return null;
        }
    }
}
