using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;
using Microsoft.Extensions.Logging;

namespace HRProBusinessLogic.BusinessLogic
{
    public class CandidateLogic : ICandidateLogic
    {
        private readonly ILogger _logger;
        private readonly ICandidateStorage _candidateStorage;
        public CandidateLogic(ILogger<CandidateLogic> logger, ICandidateStorage candidateStorage)
        {
            _logger = logger;
            _candidateStorage = candidateStorage;
        }
        public bool Create(CandidateBindingModel model)
        {
            CheckModel(model);
            if (_candidateStorage.Insert(model) == null)
            {
                _logger.LogWarning("Insert operation failed");
                return false;
            }
            return true;
        }

        public bool Delete(CandidateBindingModel model)
        {
            CheckModel(model, false);
            _logger.LogInformation("Delete. Id: {Id}", model.Id);
            if (_candidateStorage.Delete(model) == null)
            {
                _logger.LogWarning("Delete operation failed");
                return false;
            }
            return true;
        }

        public CandidateViewModel? ReadElement(CandidateSearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var element = _candidateStorage.GetElement(model);
            if (element == null)
            {
                _logger.LogWarning("ReadElement element not found");
                return null;
            }
            _logger.LogInformation("ReadElement find. Id: {Id}", element.Id);
            return element;
        }

        public List<CandidateViewModel>? ReadList(CandidateSearchModel? model)
        {
            var list = model == null ? _candidateStorage.GetFullList() : _candidateStorage.GetFilteredList(model);
            if (list == null)
            {
                _logger.LogWarning("ReadList return null list");
                return null;
            }
            _logger.LogInformation("ReadList. Count: {Count}", list.Count);
            return list;
        }

        public bool Update(CandidateBindingModel model)
        {
            CheckModel(model);
            if (_candidateStorage.Update(model) == null)
            {
                _logger.LogWarning("Update operation failed");
                return false;
            }
            return true;
        }

        private void CheckModel(CandidateBindingModel model, bool withParams = true)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!withParams)
            {
                return;
            }

            if (string.IsNullOrEmpty(model.FIO))
            {
                throw new ArgumentNullException(nameof(model.FIO), "Нет ФИО кандидата");
            }

            if (string.IsNullOrEmpty(model.Email))
            {
                throw new ArgumentNullException(nameof(model.Email), "Нет электронной почты кандидата");
            }

            if (string.IsNullOrEmpty(model.PhoneNumber))
            {
                throw new ArgumentNullException(nameof(model.PhoneNumber), "Нет номера телефона кандидата");
            }

            if (model.TestTaskId <= 0)
            {
                throw new ArgumentException("Нет идентификатора тестового задания", nameof(model.TestTaskId));
            }

            var element = _candidateStorage.GetElement(new CandidateSearchModel
            {
                FIO = model.FIO,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            });

            if (element != null && element.Id != model.Id)
            {
                throw new InvalidOperationException("Кандидат с такими данными уже существует");
            }
        }
    }
}
