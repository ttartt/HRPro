using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using HRProContracts.StoragesContracts;
using System.Text;
using System.Security.Cryptography;

namespace HRProBusinessLogic.BusinessLogic
{
    public class UserLogic : IUserLogic
    {
        private readonly ILogger _logger;
        private readonly IUserStorage _userStorage;
        private readonly IResumeStorage _resumeStorage;
        private readonly IVacancyStorage _vacancyStorage;
        public UserLogic(ILogger<UserLogic> logger, IUserStorage userStorage, IResumeStorage resumeStorage, IVacancyStorage vacancyStorage)
        {
            _logger = logger;
            _userStorage = userStorage;
            _resumeStorage = resumeStorage;
            _vacancyStorage = vacancyStorage;
        }

        private string EncryptPassword(string password)
        {
            byte[] hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
        public bool Create(UserBindingModel model)
        {
            CheckModel(model);
            CheckPassword(model);
            model.Password = EncryptPassword(model.Password);
            if (_userStorage.Insert(model) == null)
            {
                _logger.LogWarning("Insert operation failed");
                return false;
            }
            return true;
        }

        public bool Delete(UserBindingModel model)
        {
            CheckModel(model, false);
            _logger.LogInformation("Delete. Id: {Id}", model.Id);
            if (_userStorage.Delete(model) == null)
            {
                _logger.LogWarning("Delete operation failed");
                return false;
            }
            return true;
        }

        public UserViewModel? ReadElement(UserSearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var element = _userStorage.GetElement(model);
            if (element == null)
            {
                _logger.LogWarning("ReadElement: User not found for Id: {Id}", model.Id);
                return null;
            }

            string hashedPassword = element.Password;
            if (model.Password != element.Password && model.Password != null)
            {
                hashedPassword = EncryptPassword(model.Password);
            }

            var userViewModel = new UserViewModel
            {
                Id = element.Id,
                Surname = element.Surname,
                Name = element.Name,
                LastName = element.LastName,
                Email = element.Email,
                Password = hashedPassword,
                CompanyId = element.CompanyId,
                PhoneNumber = element.PhoneNumber,
                Role = element.Role,
                DateOfBirth = element.DateOfBirth
            };
            _logger.LogInformation("ReadElement: User found. Id: {Id}", element.Id);
            return userViewModel;
        }

        public List<UserViewModel>? ReadList(UserSearchModel? model)
        {
            var list = model == null ? _userStorage.GetFullList() : _userStorage.GetFilteredList(model);
            if (list == null)
            {
                _logger.LogWarning("ReadList return null list");
                return null;
            }
            _logger.LogInformation("ReadList. Count: {Count}", list.Count);
            return list;
        }

        public bool Update(UserBindingModel model)
        {
            CheckModel(model);
            var elem = _userStorage.GetElement(new UserSearchModel
            {
                Id = model.Id
            });
            if (elem != null && model.Password != elem.Password)
            {
                if (!Regex.IsMatch(model.Password, @"^^((\w+\d+\W+)|(\w+\W+\d+)|(\d+\w+\W+)|(\d+\W+\w+)|(\W+\w+\d+)|(\W+\d+\w+))[\w\d\W]*$", RegexOptions.IgnoreCase))
                {
                    throw new ArgumentException("Неправильно введенный пароль", nameof(model.Password));
                }
                model.Password = EncryptPassword(model.Password);
            }
            if (_userStorage.Update(model) == null)
            {
                _logger.LogWarning("Update operation failed");
                return false;
            }
            return true;
        }

        private void CheckModel(UserBindingModel model, bool withParams = true)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!withParams)
            {
                return;
            }

            if (string.IsNullOrEmpty(model.Surname))
            {
                throw new ArgumentNullException("Нет фамилии пользователя", nameof(model.Surname));
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                throw new ArgumentNullException("Нет имени пользователя", nameof(model.Name));
            }
            
            if (model.DateOfBirth >= DateTime.Now.ToUniversalTime().AddHours(4))
            {
                throw new ArgumentOutOfRangeException("Дата рождения не может быть позже текущей", nameof(model.DateOfBirth));
            }

            if (string.IsNullOrEmpty(model.Email))
            {
                throw new ArgumentNullException("Нет почты пользователя", nameof(model.Email));
            }

            if (!Regex.IsMatch(model.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase))
            {
                throw new ArgumentException("Неправильно введенный email", nameof(model.Email));
            }

            var element = _userStorage.GetElement(new UserSearchModel
            {
                Email = model.Email
            });

            if (element != null && element.Id != model.Id)
            {
                throw new InvalidOperationException("Пользователь с такой почтой уже есть");
            }
        }

        private void CheckPassword(UserBindingModel model)
        {
            if (string.IsNullOrEmpty(model.Password))
            {
                throw new ArgumentNullException("Нет пароля пользователя", nameof(model.Password));
            }

            if (!Regex.IsMatch(model.Password, @"^^((\w+\d+\W+)|(\w+\W+\d+)|(\d+\w+\W+)|(\d+\W+\w+)|(\W+\w+\d+)|(\W+\d+\w+))[\w\d\W]*$", RegexOptions.IgnoreCase))
            {
                throw new ArgumentException("Неправильно введенный пароль", nameof(model.Password));
            }
        }
    }
}
