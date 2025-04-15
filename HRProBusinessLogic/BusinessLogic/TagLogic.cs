using HRProContracts.BindingModels;
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
    public class TagLogic : ITagLogic
    {
        private readonly ILogger _logger;
        private readonly ITagStorage _tagStorage;
        public TagLogic(ILogger<TagLogic> logger, ITagStorage tagStorage)
        {
            _logger = logger;
            _tagStorage = tagStorage;
        }
        public int? Create(TagBindingModel model)
        {
            CheckModel(model);
            var tagId = _tagStorage.Insert(model);
            if (tagId == null)
            {
                _logger.LogWarning("Insert operation failed");
                return 0;
            }
            return tagId;
        }

        public bool Delete(TagBindingModel model)
        {
            CheckModel(model, false);
            _logger.LogInformation("Delete. Id: {Id}", model.Id);
            if (_tagStorage.Delete(model) == null)
            {
                _logger.LogWarning("Delete operation failed");
                return false;
            }
            return true;
        }

        public TagViewModel? ReadElement(TagSearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var element = _tagStorage.GetElement(model);
            if (element == null)
            {
                _logger.LogWarning("ReadElement element not found");
                return null;
            }
            _logger.LogInformation("ReadElement find. Id: {Id}", element.Id);
            return element;
        }

        public List<TagViewModel>? ReadList(TagSearchModel? model)
        {
            var list = model == null ? _tagStorage.GetFullList() : _tagStorage.GetFilteredList(model);
            if (list == null)
            {
                _logger.LogWarning("ReadList return null list");
                return null;
            }
            _logger.LogInformation("ReadList. Count: {Count}", list.Count);
            return list;
        }

        public bool Update(TagBindingModel model)
        {
            CheckModel(model);
            if (_tagStorage.Update(model) == null)
            {
                _logger.LogWarning("Update operation failed");
                return false;
            }
            return true;
        }

        private void CheckModel(TagBindingModel model, bool withParams = true)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!withParams)
            {
                return;
            }

            if (string.IsNullOrEmpty(model.TagName))
            {
                throw new ArgumentNullException("Имя тега не может быть пустым", nameof(model.TagName));
            }

            if (!Enum.IsDefined(typeof(DataTypeEnum), model.Type))
            {
                throw new ArgumentException("Некорректный тип данных", nameof(model.Type));
            }

            if (model.TemplateId <= 0)
            {
                throw new ArgumentException("Некорректный идентификатор шаблона", nameof(model.TemplateId));
            }

            var existingTag = _tagStorage.GetElement(new TagSearchModel { TagName = model.TagName });
            if (existingTag != null && existingTag.Id != model.Id)
            {
                throw new InvalidOperationException("Тег с таким именем уже существует");
            }
        }

    }
}
