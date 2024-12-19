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
    public class RequirementLogic : IRequirementLogic
    {
        private readonly ILogger _logger;
        private readonly IRequirementStorage _requirementStorage;
        public RequirementLogic(ILogger<RequirementLogic> logger, IRequirementStorage requirementStorage)
        {
            _logger = logger;
            _requirementStorage = requirementStorage;
        }
        public bool Create(RequirementBindingModel model)
        {
            CheckModel(model);
            if (_requirementStorage.Insert(model) == null)
            {
                _logger.LogWarning("Insert operation failed");
                return false;
            }
            return true;
        }

        public bool Delete(RequirementBindingModel model)
        {
            CheckModel(model, false);
            _logger.LogInformation("Delete. Id: {Id}", model.Id);
            if (_requirementStorage.Delete(model) == null)
            {
                _logger.LogWarning("Delete operation failed");
                return false;
            }
            return true;
        }

        public RequirementViewModel? ReadElement(RequirementSearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var element = _requirementStorage.GetElement(model);
            if (element == null)
            {
                _logger.LogWarning("ReadElement element not found");
                return null;
            }
            _logger.LogInformation("ReadElement find. Id: {Id}", element.Id);
            return element;
        }

        public List<RequirementViewModel>? ReadList(RequirementSearchModel? model)
        {
            var list = model == null ? _requirementStorage.GetFullList() : _requirementStorage.GetFilteredList(model);
            if (list == null)
            {
                _logger.LogWarning("ReadList return null list");
                return null;
            }
            _logger.LogInformation("ReadList. Count: {Count}", list.Count);
            return list;
        }

        public bool Update(RequirementBindingModel model)
        {
            CheckModel(model);
            if (_requirementStorage.Update(model) == null)
            {
                _logger.LogWarning("Update operation failed");
                return false;
            }
            return true;
        }

        private void CheckModel(RequirementBindingModel model, bool withParams = true)
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
                throw new ArgumentNullException(nameof(model.Name), "Нет имени требования");
            }

            var element = _requirementStorage.GetElement(new RequirementSearchModel
            {
                Name = model.Name
            });

            if (element != null && element.Id != model.Id)
            {
                throw new InvalidOperationException("Требование с таким именем уже существует");
            }
        }

    }
}
