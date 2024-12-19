using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace HRProBusinessLogic.BusinessLogic
{
    public class VacancyLogic : IVacancyLogic
    {
        private readonly ILogger _logger;
        private readonly IVacancyStorage _vacancyStorage;
        private readonly IResumeStorage _resumeStorage;
        private readonly ICompanyStorage _companyStorage;
        private readonly IUserStorage _userStorage;

        public VacancyLogic(ILogger<VacancyLogic> logger, IVacancyStorage vacancyStorage, IResumeStorage resumeStorage, ICompanyStorage companyStorage, IUserStorage userStorage)
        {
            _logger = logger;
            _vacancyStorage = vacancyStorage;
            _resumeStorage = resumeStorage;
            _companyStorage = companyStorage;
            _userStorage = userStorage;
        }
        public bool Create(VacancyBindingModel model)
        {
            CheckModel(model);
            if (_vacancyStorage.Insert(model) == null)
            {
                _logger.LogWarning("Insert operation failed");
                return false;
            }
            return true;
        }

        public bool Delete(VacancyBindingModel model)
        {
            CheckModel(model, false);
            _logger.LogInformation("Delete. Id: {Id}", model.Id);
            if (_vacancyStorage.Delete(model) == null)
            {
                _logger.LogWarning("Delete operation failed");
                return false;
            }
            return true;
        }

        public VacancyViewModel? ReadElement(VacancySearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var element = _vacancyStorage.GetElement(model);
            if (element == null)
            {
                _logger.LogWarning("ReadElement element not found");
                return null;
            }
            var resumes = _resumeStorage.GetFilteredList(new ResumeSearchModel { VacancyId = element.Id });
            var resumeViewModels = resumes?.Select(r => new ResumeViewModel
            {
                Id = r.Id,
                VacancyId = r.VacancyId,
                VacancyName = _vacancyStorage.GetElement(new VacancySearchModel { Id = r.VacancyId }).JobTitle,
                UserName = _userStorage.GetElement(new UserSearchModel { Id = r.UserId}).Name + " " + _userStorage.GetElement(new UserSearchModel { Id = r.UserId }).Surname,
                UserId = r.UserId,
                Title = r.Title,
                Experience = r.Experience,
                Education = r.Education,
                Description = r.Description,
                Skills = r.Skills,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            }).ToList() ?? new List<ResumeViewModel>();

            var vacancyViewModel = new VacancyViewModel
            {
                Id = element.Id,
                CompanyId = element.CompanyId,
                CompanyName = _companyStorage.GetElement(new CompanySearchModel { Id = element.CompanyId}).Name,
                CreatedAt = element.CreatedAt,
                Description = element.Description,
                JobTitle = element.JobTitle,
                JobType = element.JobType,
                Requirements = element.Requirements,
                Responsibilities = element.Responsibilities,                
                Salary = element.Salary,
                Status = element.Status,
                Tags = element.Tags,
                Resumes = resumeViewModels
            };
            _logger.LogInformation("ReadElement find. Id: {Id}", element.Id);
            return vacancyViewModel;
        }

        public List<VacancyViewModel>? ReadList(VacancySearchModel? model)
        {
            var list = model == null ? _vacancyStorage.GetFullList() : _vacancyStorage.GetFilteredList(model);
            if (list == null)
            {
                _logger.LogWarning("ReadList return null list");
                return null;
            }
            foreach (var item in list)
            {
                var companyName = _companyStorage.GetElement(new CompanySearchModel { Id = item.CompanyId }).Name;
                item.CompanyName = companyName;
            }
           
            _logger.LogInformation("ReadList. Count: {Count}", list.Count);
            return list;
        }

        public bool Update(VacancyBindingModel model)
        {
            CheckModel(model);
            if (_vacancyStorage.Update(model) == null)
            {
                _logger.LogWarning("Update operation failed");
                return false;
            }
            return true;
        }

        private void CheckModel(VacancyBindingModel model, bool withParams = true)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!withParams)
            {
                return;
            }

            if (model.CompanyId <= 0)
            {
                throw new ArgumentException("Некорректный идентификатор компании", nameof(model.CompanyId));
            }

            if (string.IsNullOrEmpty(model.JobTitle))
            {
                throw new ArgumentNullException("Нет названия вакансии", nameof(model.JobTitle));
            }

            if (string.IsNullOrEmpty(model.Requirements))
            {
                throw new ArgumentNullException("Нет требований к вакансии", nameof(model.Requirements));
            }

            if (string.IsNullOrEmpty(model.Responsibilities))
            {
                throw new ArgumentNullException("Нет обязанностей вакансии", nameof(model.Responsibilities));
            }

            if (string.IsNullOrEmpty(model.JobType.ToString()))
            {
                throw new ArgumentNullException("Нет типа работы", nameof(model.JobType));
            }

            if (string.IsNullOrEmpty(model.CreatedAt.ToString()))
            {
                throw new ArgumentNullException("Нет даты создания вакансии", nameof(model.CreatedAt));
            }

            if (string.IsNullOrEmpty(model.Status.ToString()))
            {
                throw new ArgumentNullException("Нет статуса вакансии", nameof(model.Status));
            }

            var element = _vacancyStorage.GetElement(new VacancySearchModel
            {
                CompanyId = model.CompanyId,
                JobTitle = model.JobTitle,
                Salary = model.Salary,
            });

            if (element != null && element.Id != model.Id)
            {
                throw new InvalidOperationException("Такая вакансия уже существует");
            }
        }
    }
}
