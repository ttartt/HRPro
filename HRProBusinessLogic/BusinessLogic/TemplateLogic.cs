﻿using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;
using HRProDataModels.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRProBusinessLogic.BusinessLogic
{
    public class TemplateLogic : ITemplateLogic
    {
        private readonly ILogger _logger;
        private readonly ITemplateStorage _templateStorage;
        private readonly ITagStorage _tagStorage;
        public TemplateLogic(ILogger<TemplateLogic> logger, ITemplateStorage templateStorage, ITagStorage tagStorage)
        {
            _logger = logger;
            _templateStorage = templateStorage;
            _tagStorage = tagStorage;
        }
        public int? Create(TemplateBindingModel model)
        {
            CheckModel(model);
            var templateId = _templateStorage.Insert(model);
            if (templateId == null)
            {
                _logger.LogWarning("Insert operation failed");
                return 0;
            }
            return templateId;
        }

        public bool Delete(TemplateBindingModel model)
        {
            CheckModel(model, false);
            _logger.LogInformation("Delete. Id: {Id}", model.Id);
            if (_templateStorage.Delete(model) == null)
            {
                _logger.LogWarning("Delete operation failed");
                return false;
            }
            return true;
        }

        public TemplateViewModel? ReadElement(TemplateSearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var element = _templateStorage.GetElement(model);
            var tags = _tagStorage.GetFilteredList(new TagSearchModel { TemplateId = element.Id });
            element.Tags = tags;
            if (element == null)
            {
                _logger.LogWarning("ReadElement element not found");
                return null;
            }
            _logger.LogInformation("ReadElement find. Id: {Id}", element.Id);
            return element;
        }

        public List<TemplateViewModel>? ReadList(TemplateSearchModel? model)
        {
            var list = model == null ? _templateStorage.GetFullList() : _templateStorage.GetFilteredList(model);
            if (list == null)
            {
                _logger.LogWarning("ReadList return null list");
                return null;
            }
            foreach (var item in list)
            {
                item.Tags = _tagStorage.GetFilteredList(new TagSearchModel { TemplateId = item.Id });
            }
            _logger.LogInformation("ReadList. Count: {Count}", list.Count);
            return list;
        }

        public bool Update(TemplateBindingModel model)
        {
            CheckModel(model);
            if (_templateStorage.Update(model) == null)
            {
                _logger.LogWarning("Update operation failed");
                return false;
            }
            return true;
        }

        private void CheckModel(TemplateBindingModel model, bool withParams = true)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!withParams)
            {
                return;
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                throw new ArgumentNullException("Название шаблона не может быть пустым", nameof(model.Name));
            }

            if (!Enum.IsDefined(typeof(TemplateTypeEnum), model.Type))
            {
                throw new ArgumentException("Некорректный тип шаблона", nameof(model.Type));
            }

            if (string.IsNullOrEmpty(model.FilePath))
            {
                throw new ArgumentNullException("Путь к файлу не может быть пустым", nameof(model.FilePath));
            }

            if (!File.Exists(model.FilePath))
            {
                throw new ArgumentException("Файл по указанному пути не существует", nameof(model.FilePath));
            }

            var existingTemplate = _templateStorage.GetElement(new TemplateSearchModel { Name = model.Name });
            if (existingTemplate != null && existingTemplate.Id != model.Id)
            {
                throw new InvalidOperationException("Шаблон с таким именем уже существует");
            }
        }
    }
}
