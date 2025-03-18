using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HRProRestApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly ILogger _logger;
        private readonly IUserLogic _logic;
        public UserController(IUserLogic logic, ILogger<UserController> logger)
        {
            _logger = logger;
            _logic = logic;
        }

        [HttpGet]
        public UserViewModel? Login(string login, string password)
        {
            try
            {
                return _logic.ReadElement(new UserSearchModel
                {
                    Email = login,
                    Password = password
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка входа в систему");
                throw;
            }
        }

        [HttpGet]
        public UserViewModel? Profile(int id)
        {
            try
            {
                return _logic.ReadElement(new UserSearchModel
                {
                    Id = id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения данных");
                throw;
            }
        }

        [HttpGet]
        public List<UserViewModel>? List(int? companyId)
        {
            try
            {
                return _logic.ReadList(new UserSearchModel { CompanyId = companyId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения данных");
                throw;
            }
        }

        [HttpPost]
        public void Register(UserBindingModel model)
        {
            try
            {
                _logic.Create(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка регистрации");
                throw;
            }
        }

        [HttpPost]
        public void Update(UserBindingModel model)
        {
            try
            {
                _logic.Update(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обновления данных");
                throw;
            }
        }

        [HttpPost]
        public void Delete(UserBindingModel model)
        {
            try
            {
                _logic.Delete(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка удаления профиля пользователя");
                throw;
            }
        }
    }
}
