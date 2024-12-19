using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRProBusinessLogic.BusinessLogic
{
    public class ResponsibilityLogic : IResponsibilityLogic
    {
        private readonly ILogger _logger;
        private readonly IResponsibilityStorage _responsibilityStorage;
        public ResponsibilityLogic(ILogger<ResponsibilityLogic> logger, IResponsibilityStorage responsibilityStorage)
        {
            _logger = logger;
            _responsibilityStorage = responsibilityStorage;
        }
        public bool Create(ResponsibilityBindingModel model)
        {
            CheckModel(model);
            if (_responsibilityStorage.Insert(model) == null)
            {
                _logger.LogWarning("Insert operation failed");
                return false;
            }
            return true;
        }

        public bool Delete(ResponsibilityBindingModel model)
        {
            CheckModel(model, false);
            _logger.LogInformation("Delete. Id: {Id}", model.Id);
            if (_responsibilityStorage.Delete(model) == null)
            {
                _logger.LogWarning("Delete operation failed");
                return false;
            }
            return true;
        }

        public ResponsibilityViewModel? ReadElement(ResponsibilitySearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var element = _responsibilityStorage.GetElement(model);
            if (element == null)
            {
                _logger.LogWarning("ReadElement element not found");
                return null;
            }
            _logger.LogInformation("ReadElement find. Id: {Id}", element.Id);
            return element;
        }

        public List<ResponsibilityViewModel>? ReadList(ResponsibilitySearchModel? model)
        {
            var list = model == null ? _responsibilityStorage.GetFullList() : _responsibilityStorage.GetFilteredList(model);
            if (list == null)
            {
                _logger.LogWarning("ReadList return null list");
                return null;
            }
            _logger.LogInformation("ReadList. Count: {Count}", list.Count);
            return list;
        }

        public bool Update(ResponsibilityBindingModel model)
        {
            CheckModel(model);
            if (_responsibilityStorage.Update(model) == null)
            {
                _logger.LogWarning("Update operation failed");
                return false;
            }
            return true;
        }

        private void CheckModel(ResponsibilityBindingModel model, bool withParams = true)
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
                throw new ArgumentNullException("Нет названия ответственности", nameof(model.Name));
            }

            var element = _responsibilityStorage.GetElement(new ResponsibilitySearchModel
            {
                Name = model.Name
            });

            if (element != null && element.Id != model.Id)
            {
                throw new InvalidOperationException("Ответственность с таким названием уже существует");
            }
        }

    }
}
