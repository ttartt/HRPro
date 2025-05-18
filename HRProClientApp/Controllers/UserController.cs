using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using HRProClientApp.Models;
using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HRProClientApp.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult UserProfile()
        {
            if (APIClient.User == null)
            {
                return Redirect("/Home/Enter");
            }

            var model = APIClient.GetRequest<UserViewModel>($"api/user/profile?id={APIClient.User.Id}"); 

            if (model == null)
            {
                return Redirect("/Home/Index");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Employees(int? companyId)
        {
            if (APIClient.User == null)
            {
                return Redirect("/Home/Enter");
            }

            var users = APIClient.GetRequest<List<UserViewModel>>($"api/user/list?companyId={companyId}")
                       ?? new List<UserViewModel>();

            var filteredUsers = users.Where(e => e.Role != HRProDataModels.Enums.RoleEnum.Администратор)
                                   .ToList();

            return View(filteredUsers);
        }

        [HttpGet]
        public IActionResult UserProfileEdit(int? id)
        {
            if (APIClient.User == null)
            {
                return Redirect("/Home/Enter");
            }

            if (!id.HasValue)
            {
                var model = new UserViewModel
                {
                    CompanyId = APIClient.Company.Id
                };
                return View(model);
            }
            else if (id.HasValue)
            {
                var employee = APIClient.GetRequest<UserViewModel?>($"api/user/profile?id={id}");
                return View(employee);
            }
            else
            {
                var model = APIClient.GetRequest<UserViewModel?>($"api/user/profile?id={APIClient.User.Id}");
                return View(model);
            }            
        }

        [HttpPost]
        public IActionResult UserProfileEdit(UserBindingModel model, string redirectUrl)
        {
            try
            {
                if (APIClient.User == null)
                {
                    return Redirect("/Home/Enter");
                }
                if (string.IsNullOrEmpty(model.Surname))
                {
                    throw new ArgumentException("Нет фамилии пользователя");
                }

                if (string.IsNullOrEmpty(model.Name))
                {
                    throw new ArgumentException("Нет имени пользователя");
                }

                if (model.DateOfBirth >= DateTime.UtcNow.AddHours(4))
                {
                    throw new ArgumentOutOfRangeException("Дата рождения не может быть позже текущей");
                }

                if (string.IsNullOrEmpty(model.Email))
                {
                    throw new ArgumentNullException("Нет почты пользователя");
                }

                if (!Regex.IsMatch(model.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase))
                {
                    throw new ArgumentException("Неправильно введенный email");
                }

                if (model.Id != 0)
                {
                    APIClient.PostRequest("api/user/update", model);
                }
                else
                {
                    var existingUser = APIClient.GetRequest<UserViewModel?>($"api/user/check?login={model.Email}");
                    if (existingUser != null)
                    {
                        throw new Exception("Такой пользователь уже существует");
                    }
                    if (string.IsNullOrEmpty(model.Password))
                    {
                        throw new ArgumentException("Нет пароля пользователя");
                    }

                    if (!Regex.IsMatch(model.Password, @"^^((\w+\d+\W+)|(\w+\W+\d+)|(\d+\w+\W+)|(\d+\W+\w+)|(\W+\w+\d+)|(\W+\d+\w+))[\w\d\W]*$", RegexOptions.IgnoreCase))
                    {
                        throw new ArgumentException("Неправильно введенный пароль");
                    }

                    APIClient.PostRequest("api/user/register", model);

                    if (APIClient.Company != null)
                    {
                        APIClient.Company.Employees.Add(new UserViewModel
                        {
                            Id = model.Id,
                            Surname = model.Surname,
                            Name = model.Name,
                            LastName = model.LastName,
                            CompanyId = APIClient.Company.Id,
                            Email = model.Email,
                            Password = model.Password,
                            Role = model.Role,
                            PhoneNumber = model.PhoneNumber,
                            DateOfBirth = model.DateOfBirth,
                            IsEmailConfirmed = model.IsEmailConfirmed
                        });
                    }
                }

                return Json(new { success = true, redirectUrl });
            }
            
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }            
        }

        public IActionResult DeleteEmployee(int id)
        {
            if (APIClient.User == null)
            {
                return Redirect("/Home/Enter");
            }
            APIClient.PostRequest("api/user/delete", new UserBindingModel
            {
                Id = id
            });
            APIClient.Company = APIClient.GetRequest<CompanyViewModel?>($"api/company/profile?id={APIClient.User?.CompanyId}");

            string redirectUrl = $"/Company/CompanyProfile?id={APIClient.Company?.Id}";
            return Redirect(redirectUrl);
        }

        /*private void SendEmail(string email, string code)
        {
            APIClient.PostRequest("api/user/SendToMail", new MailSendInfoBindingModel
            {
                MailAddress = email,
                Subject = "Подтверждение электронной почты",
                Text = $"Ваш код подтверждения: {code}"
            });
        }*/
        [HttpGet]
        public IActionResult ConfirmEmail(int userId, string code)
        {
            try
            {
                if (APIClient.User == null)
                    return Json(new { success = false, message = "Требуется авторизация" });

                if (string.IsNullOrEmpty(code))
                    return Json(new { success = false, message = "Введите код" });

                var validationResult = APIClient.GetRequest<dynamic>($"api/user/ConfirmEmail?userId={userId}&code={code}");

                if (validationResult?.success != true)
                {
                    return Json(new
                    {
                        success = false,
                        message = validationResult?.message ?? "Неверный код подтверждения"
                    });
                }

                var user = APIClient.GetRequest<UserViewModel>($"api/user/profile?id={userId}");

                try
                {
                    APIClient.PostRequest("api/user/update", new UserBindingModel
                    {
                        Id = user.Id,
                        Surname = user.Surname,
                        Name = user.Name,
                        LastName = user.LastName,
                        CompanyId = user.CompanyId,
                        Email = user.Email,
                        Password = user.Password,
                        Role = user.Role,
                        PhoneNumber = user.PhoneNumber,
                        DateOfBirth = user.DateOfBirth,
                        IsEmailConfirmed = true
                    });

                    APIClient.User.IsEmailConfirmed = true;

                    return Json(new
                    {
                        success = true,
                        message = "Email успешно подтверждён",
                        redirectUrl = $"/User/UserProfileEdit?id={userId}"
                    });
                }
                catch (Exception updateEx)
                {
                    _logger.LogError(updateEx, "Ошибка обновления пользователя");
                    return Json(new
                    {
                        success = false,
                        message = "Ошибка при обновлении данных пользователя"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка подтверждения email");
                return Json(new
                {
                    success = false,
                    message = "Ошибка подтверждения: " + ex.Message
                });
            }
        }

        [HttpPost]
        public IActionResult ResetPassword(string newPass)
        {
            try
            {
                if (APIClient.User == null)
                    return Json(new { success = false, message = "Требуется авторизация" });

                if (string.IsNullOrEmpty(newPass))
                    return Json(new { success = false, message = "Введите пароль" });

                if (!Regex.IsMatch(newPass, @"^^((\w+\d+\W+)|(\w+\W+\d+)|(\d+\w+\W+)|(\d+\W+\w+)|(\W+\w+\d+)|(\W+\d+\w+))[\w\d\W]*$", RegexOptions.IgnoreCase))
                {
                    throw new ArgumentException("Неправильно введенный пароль");
                }

                var user = APIClient.GetRequest<UserViewModel>($"api/user/profile?id={APIClient.User.Id}");

                try
                {
                    APIClient.PostRequest("api/user/update", new UserBindingModel
                    {
                        Id = user.Id,
                        Surname = user.Surname,
                        Name = user.Name,
                        LastName = user.LastName,
                        CompanyId = user.CompanyId,
                        Email = user.Email,
                        Password = newPass,
                        Role = user.Role,
                        PhoneNumber = user.PhoneNumber,
                        DateOfBirth = user.DateOfBirth,
                        IsEmailConfirmed = user.IsEmailConfirmed
                    });
                    return Json(new
                    {
                        success = true,
                        message = "Пароль успешно изменен",
                        redirectUrl = $"/User/UserProfileEdit?id={APIClient.User.Id}"
                    });
                }
                catch (Exception updateEx)
                {
                    _logger.LogError(updateEx, "Ошибка обновления пользователя");
                    return Json(new
                    {
                        success = false,
                        message = "Ошибка при обновлении данных пользователя"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка подтверждения email");
                return Json(new
                {
                    success = false,
                    message = "Ошибка подтверждения: " + ex.Message
                });
            }
        }

        public IActionResult SendConfirmationCode(int userId)
        {
            try
            {
                if (APIClient.User == null) return Redirect("/Home/Enter");
                var reciever = APIClient.GetRequest<UserViewModel>($"api/user/profile?id={userId}");

                APIClient.PostRequest("api/user/SendConfirmationCode", reciever);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка отправки кода");
                return Json(new { success = false, message = "Ошибка отправки" });
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            APIClient.User = null;
            APIClient.Company = null;
            APIClient.Token = null;

            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Enter", "Home");
        }

        public IActionResult Delete(int id)
        {
            try
            {
                if (APIClient.User == null)
                {
                    return Json(new { success = false, message = "Доступно только авторизованным пользователям" });
                }

                if (APIClient.Company == null)
                {
                    return Json(new { success = false, message = "Компания не определена" });
                }

                APIClient.PostRequest($"api/user/delete", new UserBindingModel { Id = id });
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
