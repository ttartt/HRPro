using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;

namespace HRProBusinessLogic.BusinessLogic
{
    public class ResumeLogic : IResumeLogic
    {
        private readonly ILogger _logger;
        private readonly IResumeStorage _resumeStorage;
        private readonly IVacancyStorage _vacancyStorage;
        private readonly IUserStorage _userStorage;
        public ResumeLogic(ILogger<ResumeLogic> logger, IResumeStorage resumeStorage, IUserStorage userStorage, IVacancyStorage vacancyStorage)
        {
            _logger = logger;
            _resumeStorage = resumeStorage;
            _userStorage = userStorage;
            _vacancyStorage = vacancyStorage;
        }
        public bool Create(ResumeBindingModel model)
        {
            CheckModel(model);
            if (_resumeStorage.Insert(model) == null)
            {
                _logger.LogWarning("Insert operation failed");
                return false;
            }
            return true;
        }

        public bool Delete(ResumeBindingModel model)
        {
            CheckModel(model, false);
            _logger.LogInformation("Delete. Id: {Id}", model.Id);
            if (_resumeStorage.Delete(model) == null)
            {
                _logger.LogWarning("Delete operation failed");
                return false;
            }
            return true;
        }

        public ResumeViewModel? ReadElement(ResumeSearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var element = _resumeStorage.GetElement(model);
            if (element == null)
            {
                _logger.LogWarning("ReadElement element not found");
                return null;
            }

            _logger.LogInformation("ReadElement find. Id: {Id}", element.Id);
            return element;
        }

        public List<ResumeViewModel>? ReadList(ResumeSearchModel? model)
        {
            var list = model == null ? _resumeStorage.GetFullList() : _resumeStorage.GetFilteredList(model);
            if (list == null)
            {
                _logger.LogWarning("ReadList return null list");
                return null;
            }
            List<ResumeViewModel> result = new();
            foreach (var element in list)
            {
                var resumeViewModel = new ResumeViewModel
                {
                    Id = element.Id,
                    VacancyId = element.VacancyId,
                    //VacancyName = _vacancyStorage.GetElement(new VacancySearchModel { Id = element.VacancyId }).JobTitle,
                    Title = element.Title,
                    Experience = element.Experience,
                    Education = element.Education,
                    Description = element.Description,
                    Skills = element.Skills,
                    Status = element.Status,
                    CreatedAt = element.CreatedAt,
                    Salary = element.Salary,
                    CandidateInfo = element.CandidateInfo
                };
                result.Add(resumeViewModel);
            }
            _logger.LogInformation("ReadList. Count: {Count}", result.Count);
            return result;
        }

        public bool Update(ResumeBindingModel model)
        {
            CheckModel(model);
            if (_resumeStorage.Update(model) == null)
            {
                _logger.LogWarning("Update operation failed");
                return false;
            }
            return true;
        }

        private void CheckModel(ResumeBindingModel model, bool withParams = true)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!withParams)
            {
                return;
            }

            var element = _resumeStorage.GetElement(new ResumeSearchModel
            {
                VacancyId = model.VacancyId,
                Title = model.Title
            });

            if (element != null && element.Id != model.Id)
            {
                throw new InvalidOperationException("Такое резюме уже существует");
            }
        }
    }
}
