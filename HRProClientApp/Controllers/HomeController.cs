using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace HRProClientApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Enter()
        {
            return View();
        }
/*
        private void SendEmail(string email)
        {
            APIClient.PostRequest("api/user/SendToMail", new MailSendInfoBindingModel
            {
                MailAddress = email,
                Subject = "Подтверждение почты",
                Text = "232323"
            });
        }*/

        [HttpPost]
        public async Task<IActionResult> Enter(string login, string password)
        {
            string redirectUrl = "/Home/Index";
            try
            {
                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                {
                    throw new Exception("Введите логин и пароль");
                }

                var response = await APIClient.PostRequestAsync<HRProContracts.BindingModels.LoginRequest, LoginResponse>(
                    "api/user/login",
                    new HRProContracts.BindingModels.LoginRequest { Login = login, Password = password });

                if (response == null || response.User == null)
                {
                    throw new Exception("Неверный логин/пароль");
                }

                APIClient.User = response.User;
                APIClient.Token = response.Token;

                if (response.User.CompanyId != null)
                {
                    APIClient.Company = await APIClient.GetRequestAsync<CompanyViewModel>(
                        $"api/company/profile?id={response.User.CompanyId}");
                }

                //SendEmail(login);

                return Json(new { success = true, redirectUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GoogleAuth()
        {
            if (APIClient.User == null)
            {
                return RedirectToAction("Enter");
            }

            var clientId = _configuration["Google:ClientId"];
            var redirectUri = Url.Action("GoogleCallback", "Home", null, Request.Scheme);
            var state = $"{Guid.NewGuid()}|{DateTime.UtcNow:o}";

            Response.Cookies.Append(
                "GoogleOAuthState",
                state,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(5)
                });

            var authUrl = $"https://accounts.google.com/o/oauth2/v2/auth?" +
                          $"response_type=code&" +
                          $"client_id={Uri.EscapeDataString(clientId)}&" +
                          $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
                          $"scope={Uri.EscapeDataString("https://www.googleapis.com/auth/calendar.events")}&" +
                          $"state={Uri.EscapeDataString(state)}&" +
                          $"access_type=offline&" +
                          $"prompt=consent";

            return Redirect(authUrl);
        }


        [HttpGet]
        public async Task<IActionResult> GoogleCallback(string code, string state)
        {
            var savedState = Request.Cookies["GoogleOAuthState"];
            if (savedState == null)
            {
                TempData["GoogleAuthError"] = "Ошибка безопасности при авторизации";
                return RedirectToAction("Enter");
            }

            Response.Cookies.Delete("GoogleOAuthState");

            var stateCreationTime = DateTime.ParseExact(
                savedState.Split('|')[1],
                "o",
                CultureInfo.InvariantCulture);

            if (DateTime.UtcNow - stateCreationTime > TimeSpan.FromMinutes(5))
            {
                TempData["GoogleAuthError"] = "Время авторизации истекло";
                return RedirectToAction("Enter");
            }

            try
            {
                var tokenRequest = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("client_id", _configuration["Google:ClientId"]),
                    new KeyValuePair<string, string>("client_secret", _configuration["Google:ClientSecret"]),
                    new KeyValuePair<string, string>("redirect_uri", Url.Action("GoogleCallback", "Home", null, Request.Scheme)),
                    new KeyValuePair<string, string>("grant_type", "authorization_code")
                });

                var client = _httpClientFactory.CreateClient();
                var response = await client.PostAsync("https://oauth2.googleapis.com/token", tokenRequest);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Ошибка Google OAuth: {error}");
                }

                var tokenResponse = await response.Content.ReadFromJsonAsync<GoogleTokenResponse>();

                if (APIClient.User == null)
                {
                    TempData["GoogleAuthError"] = "Пользователь не авторизован";
                    return RedirectToAction("Enter");
                }

                APIClient.User.GoogleToken = tokenResponse.access_token;
                APIClient.User.GoogleRefreshToken = tokenResponse.refresh_token;
                APIClient.User.GoogleTokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.expires_in);

                APIClient.PostRequest("api/user/update", APIClient.User);

                TempData["GoogleAuthSuccess"] = "Google Календарь успешно подключен!";
                return RedirectToAction("Meetings", "Meeting");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка авторизации через Google");
                TempData["GoogleAuthError"] = $"Ошибка при подключении: {ex.Message}";
                return RedirectToAction("Enter");
            }
        }
    }
}