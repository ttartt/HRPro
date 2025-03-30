using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;
using HRproDatabaseImplement;
using HRProDatabaseImplement.Models;

namespace HRProDatabaseImplement.Implements
{
    public class DocumentTagStorage : IDocumentTagStorage
    {
        public List<DocumentTagViewModel> GetFullList()
        {
            using var context = new HRproDatabase();
            return context.DocumentTags
                .Select(x => x.GetViewModel)
                .ToList();
        }
        public List<DocumentTagViewModel> GetFilteredList(DocumentTagSearchModel model)
        {
            using var context = new HRproDatabase();
            if (model.DocumentId.HasValue)
            {
                return context.DocumentTags
                .Where(x => x.DocumentId.Equals(model.DocumentId))
                .Select(x => x.GetViewModel)
                .ToList();
            }
            else if (model.TagId.HasValue)
            {
                return context.DocumentTags
                .Where(x => x.TagId.Equals(model.TagId))
                .Select(x => x.GetViewModel)
                .ToList();
            }
            return context.DocumentTags
                .Where(x => x.Id.Equals(model.Id))
                .Select(x => x.GetViewModel)
                .ToList();
        }
        public DocumentTagViewModel? GetElement(DocumentTagSearchModel model)
        {
            if (!model.Id.HasValue)
            {
                return null;
            }
            using var context = new HRproDatabase();
            return context.DocumentTags
                .FirstOrDefault(x => x.Id == model.Id)
                ?.GetViewModel;
        }
        public DocumentTagViewModel? Insert(DocumentTagBindingModel model)
        {
            var newDocumentTag = DocumentTag.Create(model);
            if (newDocumentTag == null)
            {
                return null;
            }
            using var context = new HRproDatabase();
            context.DocumentTags.Add(newDocumentTag);
            context.SaveChanges();
            return newDocumentTag.GetViewModel;
        }
        public DocumentTagViewModel? Update(DocumentTagBindingModel model)
        {
            using var context = new HRproDatabase();
            var criterion = context.DocumentTags.FirstOrDefault(x => x.Id == model.Id);
            if (criterion == null)
            {
                return null;
            }
            criterion.Update(model);
            context.SaveChanges();
            return criterion.GetViewModel;
        }
        public DocumentTagViewModel? Delete(DocumentTagBindingModel model)
        {
            using var context = new HRproDatabase();
            var element = context.DocumentTags.FirstOrDefault(rec => rec.Id == model.Id);
            if (element != null)
            {
                context.DocumentTags.Remove(element);
                context.SaveChanges();
                return element.GetViewModel;
            }
            return null;
        }
    }
}
