using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;
using Microsoft.Extensions.Logging;

namespace HRProBusinessLogic.BusinessLogic
{
    public class DocumentTagLogic : IDocumentTagLogic
    {
        private readonly ILogger _logger;
        private readonly IDocumentTagStorage _documentTagStorage;
        public DocumentTagLogic(ILogger<DocumentTagLogic> logger, IDocumentTagStorage documentTagStorage)
        {
            _logger = logger;
            _documentTagStorage = documentTagStorage;
        }
        public bool Create(DocumentTagBindingModel model)
        {
            CheckModel(model);
            if (_documentTagStorage.Insert(model) == null)
            {
                _logger.LogWarning("Insert operation failed");
                return false;
            }
            return true;
        }

        public bool Delete(DocumentTagBindingModel model)
        {
            CheckModel(model, false);
            _logger.LogInformation("Delete. Id: {Id}", model.Id);
            if (_documentTagStorage.Delete(model) == null)
            {
                _logger.LogWarning("Delete operation failed");
                return false;
            }
            return true;
        }

        public DocumentTagViewModel? ReadElement(DocumentTagSearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var element = _documentTagStorage.GetElement(model);
            if (element == null)
            {
                _logger.LogWarning("ReadElement element not found");
                return null;
            }
            _logger.LogInformation("ReadElement find. Id: {Id}", element.Id);
            return element;
        }

        public List<DocumentTagViewModel>? ReadList(DocumentTagSearchModel? model)
        {
            var list = model == null ? _documentTagStorage.GetFullList() : _documentTagStorage.GetFilteredList(model);
            if (list == null)
            {
                _logger.LogWarning("ReadList return null list");
                return null;
            }
            _logger.LogInformation("ReadList. Count: {Count}", list.Count);
            return list;
        }

        public bool Update(DocumentTagBindingModel model)
        {
            CheckModel(model);
            if (_documentTagStorage.Update(model) == null)
            {
                _logger.LogWarning("Update operation failed");
                return false;
            }
            return true;
        }

        private void CheckModel(DocumentTagBindingModel model, bool withParams = true)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!withParams)
            {
                return;
            }

            if (model.TagId <= 0)
            {
                throw new ArgumentException("Нет идентификатора тега", nameof(model.TagId));
            }

            if (model.DocumentId <= 0)
            {
                throw new ArgumentException("Нет идентификатора документа", nameof(model.DocumentId));
            }

            if (string.IsNullOrEmpty(model.Value))
            {
                throw new ArgumentNullException("Значение тега не может быть пустым", nameof(model.Value));
            }

            var element = _documentTagStorage.GetElement(new DocumentTagSearchModel
            {
                TagId = model.TagId,
                DocumentId = model.DocumentId
            });

            if (element != null && element.Id != model.Id)
            {
                throw new InvalidOperationException("Этот тэг уже был заполнен");
            }
        }
    }
}
