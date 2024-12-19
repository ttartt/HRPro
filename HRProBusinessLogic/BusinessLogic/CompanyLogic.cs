using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace HRProBusinessLogic.BusinessLogic
{
    public class CompanyLogic : ICompanyLogic
    {
        private readonly ILogger _logger;
        private readonly ICompanyStorage _сompanyStorage;
        private readonly IVacancyStorage _vacancyStorage;
        private readonly IUserStorage _userStorage;
        public CompanyLogic(ILogger<CompanyLogic> logger, ICompanyStorage сompanyStorage, IVacancyStorage vacancyStorage, IUserStorage userStorage)
        {
            _logger = logger;
            _сompanyStorage = сompanyStorage;
            _vacancyStorage = vacancyStorage;
            _userStorage = userStorage;
        }
        public int? Create(CompanyBindingModel model)
        {
            CheckModel(model);
            var companyId = _сompanyStorage.Insert(model);
            if (companyId == null)
            {
                _logger.LogWarning("Insert operation failed");
                return 0;
            }
            return companyId;
        }

        public bool Delete(CompanyBindingModel model)
        {
            CheckModel(model, false);
            _logger.LogInformation("Delete. Id: {Id}", model.Id);
            if (_сompanyStorage.Delete(model) == null)
            {
                _logger.LogWarning("Delete operation failed");
                return false;
            }
            return true;
        }

        public CompanyViewModel? ReadElement(CompanySearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var company = _сompanyStorage.GetElement(model); 
            if (company == null)
            {
                _logger.LogWarning("ReadElement: Company not found for Id: {Id}", model.Id);
                return null;
            }
            var vacancies = _vacancyStorage.GetFilteredList(new VacancySearchModel { CompanyId = company.Id});

            var vacancyViewModels = vacancies?.Select(v => new VacancyViewModel
            {
                Id = v.Id,
                CompanyId = v.CompanyId,
                JobTitle = v.JobTitle,
                Requirements = v.Requirements,
                Responsibilities = v.Responsibilities,
                JobType = v.JobType,
                Salary = v.Salary,
                Description = v.Description,
                Status = v.Status,
                CreatedAt = v.CreatedAt,
                Tags = v.Tags
            }).ToList() ?? new List<VacancyViewModel>();

            var employees = _userStorage.GetFilteredList(new UserSearchModel { CompanyId = company.Id, Role = HRProDataModels.Enums.RoleEnum.Сотрудник });

            var employeeViewModels = employees?.Select(u => new UserViewModel
            {
                Id = u.Id,
                CompanyId = u.CompanyId,
                Surname = u.Surname,
                Name = u.Name,
                LastName = u.LastName,
                Email = u.Email,
                Password = u.Password,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role,
                DateOfBirth = u.DateOfBirth
            }).ToList() ?? new List<UserViewModel>();

            var companyViewModel = new CompanyViewModel
            {
                Id = company.Id,
                Name = company.Name,
                Description = company.Description,
                Contacts = company.Contacts,
                Address = company.Address,
                Website = company.Website,
                LogoFilePath = company.LogoFilePath,
                Vacancies = vacancyViewModels,
                Employees = employeeViewModels
            };

            _logger.LogInformation("ReadElement: Company found. Id: {Id}", company.Id);
            return companyViewModel;
        }

        public List<CompanyViewModel>? ReadList(CompanySearchModel? model)
        {
            var list = model == null ? _сompanyStorage.GetFullList() : _сompanyStorage.GetFilteredList(model);
            if (list == null)
            {
                _logger.LogWarning("ReadList return null list");
                return null;
            }
            _logger.LogInformation("ReadList. Count: {Count}", list.Count);
            return list;
        }

        public bool Update(CompanyBindingModel model)
        {
            CheckModel(model);
            if (_сompanyStorage.Update(model) == null)
            {
                _logger.LogWarning("Update operation failed");
                return false;
            }
            return true;
        }

        private void CheckModel(CompanyBindingModel model, bool withParams = true)
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
                throw new ArgumentNullException("Нет названия компании", nameof(model.Name));
            }

            var element = _сompanyStorage.GetElement(new CompanySearchModel
            {
                Name = model.Name
            });

            if (element != null && element.Id != model.Id)
            {
                throw new InvalidOperationException("Компания с таким названием уже есть");
            }
        }
    }
}
