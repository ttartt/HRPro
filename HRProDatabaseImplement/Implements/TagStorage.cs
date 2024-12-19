using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;
using HRproDatabaseImplement;
using HRProDatabaseImplement.Models;

namespace HRProDatabaseImplement.Implements
{
    public class TagStorage : ITagStorage
    {
        public List<TagViewModel> GetFullList()
        {
            using var context = new HRproDatabase();
            return context.Tags
                .Select(x => x.GetViewModel)
                .ToList();
        }
        public List<TagViewModel> GetFilteredList(TagSearchModel model)
        {
            if (string.IsNullOrEmpty(model.TagName))
            {
                return new();
            }
            using var context = new HRproDatabase();
            return context.Tags
                .Where(x => x.Name.Contains(model.TagName))
                .Select(x => x.GetViewModel)
                .ToList();
        }
        public TagViewModel? GetElement(TagSearchModel model)
        {
            if (string.IsNullOrEmpty(model.TagName) && !model.Id.HasValue)
            {
                return null;
            }
            using var context = new HRproDatabase();
            return context.Tags
                .FirstOrDefault(x => (!string.IsNullOrEmpty(model.TagName) && x.TagName == model.TagName) || (model.Id.HasValue && x.Id == model.Id))
                ?.GetViewModel;
        }
        public TagViewModel? Insert(TagBindingModel model)
        {
            var newTag = Tag.Create(model);
            if (newTag == null)
            {
                return null;
            }
            using var context = new HRproDatabase();
            context.Tags.Add(newTag);
            context.SaveChanges();
            return newTag.GetViewModel;
        }
        public TagViewModel? Update(TagBindingModel model)
        {
            using var context = new HRproDatabase();
            var criterion = context.Tags.FirstOrDefault(x => x.Id == model.Id);
            if (criterion == null)
            {
                return null;
            }
            criterion.Update(model);
            context.SaveChanges();
            return criterion.GetViewModel;
        }
        public TagViewModel? Delete(TagBindingModel model)
        {
            using var context = new HRproDatabase();
            var element = context.Tags.FirstOrDefault(rec => rec.Id == model.Id);
            if (element != null)
            {
                context.Tags.Remove(element);
                context.SaveChanges();
                return element.GetViewModel;
            }
            return null;
        }
    }
}
