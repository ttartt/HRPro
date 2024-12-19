using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;
using HRproDatabaseImplement;
using HRProDatabaseImplement.Models;

namespace HRProDatabaseImplement.Implements
{
    public class DocumentStorage : IDocumentStorage
    {
        public List<DocumentViewModel> GetFullList()
        {
            using var context = new HRproDatabase();
            return context.Documents
                .Select(x => x.GetViewModel)
                .ToList();
        }
        public List<DocumentViewModel> GetFilteredList(DocumentSearchModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                return new();
            }
            using var context = new HRproDatabase();
            return context.Documents
                .Where(x => x.Name.Contains(model.Name))
                .Select(x => x.GetViewModel)
                .ToList();
        }
        public DocumentViewModel? GetElement(DocumentSearchModel model)
        {
            if (string.IsNullOrEmpty(model.Name) && !model.Id.HasValue)
            {
                return null;
            }
            using var context = new HRproDatabase();
            return context.Documents
                .FirstOrDefault(x => (!string.IsNullOrEmpty(model.Name) && x.Name == model.Name) || (model.Id.HasValue && x.Id == model.Id))
                ?.GetViewModel;
        }
        public DocumentViewModel? Insert(DocumentBindingModel model)
        {
            var newDocument = Document.Create(model);
            if (newDocument == null)
            {
                return null;
            }
            using var context = new HRproDatabase();
            context.Documents.Add(newDocument);
            context.SaveChanges();
            return newDocument.GetViewModel;
        }
        public DocumentViewModel? Update(DocumentBindingModel model)
        {
            using var context = new HRproDatabase();
            var criterion = context.Documents.FirstOrDefault(x => x.Id == model.Id);
            if (criterion == null)
            {
                return null;
            }
            criterion.Update(model);
            context.SaveChanges();
            return criterion.GetViewModel;
        }
        public DocumentViewModel? Delete(DocumentBindingModel model)
        {
            using var context = new HRproDatabase();
            var element = context.Documents.FirstOrDefault(rec => rec.Id == model.Id);
            if (element != null)
            {
                context.Documents.Remove(element);
                context.SaveChanges();
                return element.GetViewModel;
            }
            return null;
        }
    }
}
