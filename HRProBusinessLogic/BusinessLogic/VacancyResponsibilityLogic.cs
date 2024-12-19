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
    public class VacancyResponsibilityLogic : IVacancyResponsibilityLogic
    {
        private readonly ILogger _logger;
        private readonly IVacancyResponsibilityStorage _vacancyResponsibilityStorage;
        public VacancyResponsibilityLogic(ILogger<VacancyResponsibilityLogic> logger, IVacancyResponsibilityStorage vacancyResponsibilityStorage)
        {
            _logger = logger;
            _vacancyResponsibilityStorage = vacancyResponsibilityStorage;
        }
        public bool Create(VacancyResponsibilityBindingModel model)
        {
            CheckModel(model);
            if (_vacancyResponsibilityStorage.Insert(model) == null)
            {
                _logger.LogWarning("Insert operation failed");
                return false;
            }
            return true;
        }

        public bool Delete(VacancyResponsibilityBindingModel model)
        {
            CheckModel(model, false);
            _logger.LogInformation("Delete. Id: {Id}", model.Id);
            if (_vacancyResponsibilityStorage.Delete(model) == null)
            {
                _logger.LogWarning("Delete operation failed");
                return false;
            }
            return true;
        }

        public VacancyResponsibilityViewModel? ReadElement(VacancyResponsibilitySearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var element = _vacancyResponsibilityStorage.GetElement(model);
            if (element == null)
            {
                _logger.LogWarning("ReadElement element not found");
                return null;
            }
            _logger.LogInformation("ReadElement find. Id: {Id}", element.Id);
            return element;
        }

        public List<VacancyResponsibilityViewModel>? ReadList(VacancyResponsibilitySearchModel? model)
        {
            var list = model == null ? _vacancyResponsibilityStorage.GetFullList() : _vacancyResponsibilityStorage.GetFilteredList(model);
            if (list == null)
            {
                _logger.LogWarning("ReadList return null list");
                return null;
            }
            _logger.LogInformation("ReadList. Count: {Count}", list.Count);
            return list;
        }

        public bool Update(VacancyResponsibilityBindingModel model)
        {
            CheckModel(model);
            if (_vacancyResponsibilityStorage.Update(model) == null)
            {
                _logger.LogWarning("Update operation failed");
                return false;
            }
            return true;
        }

        private void CheckModel(VacancyResponsibilityBindingModel model, bool withParams = true)
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
                throw new ArgumentException("Некорректный идентификатор вакансии", nameof(model.VacancyId));
            }

            if (model.ResponsibilityId <= 0)
            {
                throw new ArgumentException("Некорректный идентификатор ответственности", nameof(model.ResponsibilityId));
            }

            var existingVacancyResponsibility = _vacancyResponsibilityStorage.GetElement(new VacancyResponsibilitySearchModel
            {
                VacancyId = model.VacancyId,
                ResponsibilityId = model.ResponsibilityId
            });

            if (existingVacancyResponsibility != null && existingVacancyResponsibility.Id != model.Id)
            {
                throw new InvalidOperationException("Данная ответственность уже связана с этой вакансией");
            }
        }

    }
}
