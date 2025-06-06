﻿using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;
using Microsoft.Extensions.Logging;

namespace HRProBusinessLogic.BusinessLogic
{
    public class DocumentLogic : IDocumentLogic
    {
        private readonly ILogger _logger;
        private readonly IDocumentStorage _documentStorage;
        private readonly ICompanyStorage _companyStorage;
        private readonly IUserStorage _userStorage;
        private readonly ITemplateStorage _templateStorage;
        public DocumentLogic(ILogger<DocumentLogic> logger, IDocumentStorage documentStorage, ICompanyStorage companyStorage, IUserStorage userStorage, ITemplateStorage templateStorage)
        {
            _logger = logger;
            _documentStorage = documentStorage;
            _companyStorage = companyStorage;
            _userStorage = userStorage;
            _templateStorage = templateStorage;
        }
        public int? Create(DocumentBindingModel model)
        {
            CheckModel(model);
            var documentId = _documentStorage.Insert(model);
            if (documentId == null)
            {
                _logger.LogWarning("Insert operation failed");
                return 0;
            }
            return documentId;
        }

        public bool Delete(DocumentBindingModel model)
        {
            CheckModel(model, false);
            _logger.LogInformation("Delete. Id: {Id}", model.Id);
            if (_documentStorage.Delete(model) == null)
            {
                _logger.LogWarning("Delete operation failed");
                return false;
            }
            return true;
        }

        public DocumentViewModel? ReadElement(DocumentSearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var element = _documentStorage.GetElement(model);
            if (element == null)
            {
                _logger.LogWarning("ReadElement element not found");
                return null;
            }
            _logger.LogInformation("ReadElement find. Id: {Id}", element.Id);
            return element;
        }

        public List<DocumentViewModel>? ReadList(DocumentSearchModel? model)
        {
            var list = model == null ? _documentStorage.GetFullList() : _documentStorage.GetFilteredList(model);
            if (list == null)
            {
                _logger.LogWarning("ReadList return null list");
                return null;
            }
            var result = new List<DocumentViewModel>();
            foreach (var item in list)
            {
                var companyName = _companyStorage.GetElement(new CompanySearchModel { Id = item.CompanyId }).Name;
                var creatorName = _userStorage.GetElement(new UserSearchModel { Id = item.CreatorId }).Name;
                var template = _templateStorage.GetElement(new TemplateSearchModel { Id = item.TemplateId });
                var viewModel = new DocumentViewModel
                {
                    CompanyId = item.CompanyId,
                    CompanyName = companyName == null ? "Компания удалена" : companyName,
                    CreatorName = creatorName == null ? "Создатель удалён" : creatorName,
                    CreatedAt = item.CreatedAt,
                    CreatorId = item.CreatorId,
                    Id = item.Id,
                    Name = item.Name,
                    TemplateId = item.TemplateId,
                    TemplateName = template == null ? "Шаблон удалён" : template.Name,
                    FilePath = item.FilePath
                };
                result.Add(viewModel);
            }
            _logger.LogInformation("ReadList. Count: {Count}", list.Count);
            return result;
        }

        public bool Update(DocumentBindingModel model)
        {
            CheckModel(model);
            if (_documentStorage.Update(model) == null)
            {
                _logger.LogWarning("Update operation failed");
                return false;
            }
            return true;
        }

        private void CheckModel(DocumentBindingModel model, bool withParams = true)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!withParams)
            {
                return;
            }

            if (model.CreatorId <= 0)
            {
                throw new ArgumentException("Некорректный идентификатор создателя", nameof(model.CreatorId));
            }

            if (model.CompanyId <= 0)
            {
                throw new ArgumentException("Некорректный идентификатор компании", nameof(model.CompanyId));
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                throw new ArgumentNullException("Название документа не может быть пустым", nameof(model.Name));
            }

            if (model.CreatedAt == default)
            {
                throw new ArgumentException("Некорректная дата создания документа", nameof(model.CreatedAt));
            }

            if (model.TemplateId <= 0)
            {
                throw new ArgumentException("Некорректный идентификатор шаблона", nameof(model.TemplateId));
            }

            var existingDocument = _documentStorage.GetElement(new DocumentSearchModel
            {
                Name = model.Name,
                CompanyId = model.CompanyId
            });

            if (existingDocument != null && existingDocument.Id != model.Id)
            {
                throw new InvalidOperationException("Документ с таким названием для данной компании уже существует");
            }
        }

    }
}
