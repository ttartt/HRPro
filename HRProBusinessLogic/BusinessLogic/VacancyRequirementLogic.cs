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
    public class VacancyRequirementLogic : IVacancyRequirementLogic
    {
        private readonly ILogger _logger;
        private readonly IVacancyRequirementStorage _vacancyRequirementStorage;
        public VacancyRequirementLogic(ILogger<VacancyRequirementLogic> logger, IVacancyRequirementStorage vacancyRequirementStorage)
        {
            _logger = logger;
            _vacancyRequirementStorage = vacancyRequirementStorage;
        }
        public bool Create(VacancyRequirementBindingModel model)
        {
            CheckModel(model);
            if (_vacancyRequirementStorage.Insert(model) == null)
            {
                _logger.LogWarning("Insert operation failed");
                return false;
            }
            return true;
        }

        public bool Delete(VacancyRequirementBindingModel model)
        {
            CheckModel(model, false);
            _logger.LogInformation("Delete. Id: {Id}", model.Id);
            if (_vacancyRequirementStorage.Delete(model) == null)
            {
                _logger.LogWarning("Delete operation failed");
                return false;
            }
            return true;
        }

        public VacancyRequirementViewModel? ReadElement(VacancyRequirementSearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var element = _vacancyRequirementStorage.GetElement(model);
            if (element == null)
            {
                _logger.LogWarning("ReadElement element not found");
                return null;
            }
            _logger.LogInformation("ReadElement find. Id: {Id}", element.Id);
            return element;
        }

        public List<VacancyRequirementViewModel>? ReadList(VacancyRequirementSearchModel? model)
        {
            var list = model == null ? _vacancyRequirementStorage.GetFullList() : _vacancyRequirementStorage.GetFilteredList(model);
            if (list == null)
            {
                _logger.LogWarning("ReadList return null list");
                return null;
            }
            _logger.LogInformation("ReadList. Count: {Count}", list.Count);
            return list;
        }

        public bool Update(VacancyRequirementBindingModel model)
        {
            CheckModel(model);
            if (_vacancyRequirementStorage.Update(model) == null)
            {
                _logger.LogWarning("Update operation failed");
                return false;
            }
            return true;
        }

        private void CheckModel(VacancyRequirementBindingModel model, bool withParams = true)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!withParams)
            {
                return;
            }

            if (model.VacancyId <= 0)
            {
                throw new ArgumentException("Нет идентификатора вакансии", nameof(model.VacancyId));
            }

            if (model.RequirementId <= 0)
            {
                throw new ArgumentException("Нет идентификатора требования", nameof(model.RequirementId));
            }

            var element = _vacancyRequirementStorage.GetElement(new VacancyRequirementSearchModel
            {
                VacancyId = model.VacancyId,
                RequirementId = model.RequirementId
            });

            if (element != null && element.Id != model.Id)
            {
                throw new InvalidOperationException("Требование для этой вакансии уже существует");
            }
        }

    }
}
