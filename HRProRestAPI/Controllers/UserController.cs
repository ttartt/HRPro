using HRProBusinessLogic.MailWorker;
using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HRProRestApi.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly ILogger _logger;
        private readonly IUserLogic _logic;
        private readonly IConfiguration _config;
        private readonly IMessageInfoLogic _mailLogic;
        private readonly AbstractMailWorker _mailWorker;
        private static Dictionary<int, string> _confirmationCodes = new Dictionary<int, string>();
        public UserController(IUserLogic logic, ILogger<UserController> logger, IConfiguration config, IMessageInfoLogic mailLogic, AbstractMailWorker mailWorker)
        {
            _logger = logger;
            _logic = logic;
            _config = config;
            _mailLogic = mailLogic;
            _mailWorker = mailWorker;
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] HRProContracts.BindingModels.LoginRequest request)
        {
            try
            {
                var user = _logic.ReadElement(new UserSearchModel
                {
                    Email = request.Login,
                    Password = request.Password
                });

                if (user == null)
                {
                    return Unauthorized("Неверный логин или пароль.");
                }

                var token = GenerateJwtToken(user);

                return Ok(new HRProContracts.BindingModels.LoginResponse
                {
                    Token = token,
                    User = user
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка входа в систему");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpGet]
        public UserViewModel Check(string login)
        {
            try
            {
                return _logic.ReadElement(new UserSearchModel
                {
                    Email = login
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения пользователя");
                throw;
            }
        }

        [HttpGet]
        public IActionResult ConfirmEmail(int userId, string code)
        {
            try
            {
                if (_confirmationCodes.TryGetValue(userId, out var savedCode))
                {
                    bool isValid = savedCode == code;
                    if (isValid)
                    {
                        _confirmationCodes.Remove(userId); 
                    }

                    return Json(new
                    {
                        success = isValid,
                        message = isValid ? "Email успешно подтверждён" : "Неверный код подтверждения"
                    });
                }

                return Json(new
                {
                    success = false,
                    message = "Код подтверждения не найден или устарел"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка подтверждения Email");
                return Json(new
                {
                    success = false,
                    message = "Ошибка сервера при подтверждении email"
                });
            }
        }

        [HttpPost]
        public IActionResult SendConfirmationCode(UserViewModel model)
        {
            try
            {
                var code = Guid.NewGuid().ToString("N")[..6].ToUpper();
                var userId = model.Id;
                _confirmationCodes[userId] = code;

                _mailWorker.MailSendAsync(new MailSendInfoBindingModel
                {
                    MailAddress = model.Email,
                    Subject = "Код подтверждения",
                    Text = $"Ваш код подтверждения: {code}"
                });

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отправке кода");
                return StatusCode(500);
            }
        }

        /*[HttpPost]
        public IActionResult SendToMail(MailSendInfoBindingModel model)
        {
            try
            {
                _mailWorker.MailSendAsync(model);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка отправки email");
                return StatusCode(500);
            }
        }*/

        private string GenerateJwtToken(UserViewModel user)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiryInMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok($"Профиль пользователя ID = {userId}");
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

        [HttpGet]
        public List<MessageInfoViewModel>? GetMessages(int clientId)
        {
            try
            {
                return _mailLogic.ReadList(new MessageInfoSearchModel
                {
                    UserId = clientId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения писем пользователя");
                throw;
            }
        }
    }
}
