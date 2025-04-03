using HRProClientApp;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
namespace HRProClientApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(
            ILogger<HomeController> logger)
        {
            _logger = logger;
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

        [HttpPost]
        public async Task<IActionResult> Enter(string login, string password)
        {
            string redirectUrl = "/Home/Index";
            try
            {
                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                {
                    throw new Exception("������� ����� � ������");
                }

                APIClient.User = await APIClient.GetRequestAsync<UserViewModel>(
                    $"api/user/login?login={login}&password={password}");

                if (APIClient.User == null)
                {
                    throw new Exception("�������� �����/������");
                }

                if (APIClient.User?.CompanyId != null)
                {
                    APIClient.Company = await APIClient.GetRequestAsync<CompanyViewModel>(
                        $"api/company/profile?id={APIClient.User?.CompanyId}");
                }

                return Json(new { success = true, redirectUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /*[HttpGet]
        public IActionResult YandexAuth()
        {
            if (APIClient.User == null)
            {
                TempData["YandexAuthError"] = "������� ������� � �������";
                return RedirectToAction("Enter");
            }

            var clientId = _configuration["Yandex:ClientId"];
            var redirectUri = Url.Action("YandexCallback", "Home", null, Request.Scheme);
            var state = $"{Guid.NewGuid()}|{DateTime.UtcNow.ToString("o")}";

            // ��������� state � cookies ��������
            Response.Cookies.Append(
                "YandexOAuthState",
                state,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.Now.AddMinutes(5)
                });

            var authUrl = $"https://oauth.yandex.ru/authorize?" +
                $"response_type=code&" +
                $"client_id={clientId}&" +
                $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
                $"state={state}";

            return Redirect(authUrl);
        }


        [HttpGet]
        public async Task<IActionResult> YandexCallback(string code, string error, string state)
        {
            // �������� state �� cookies
            var savedState = Request.Cookies["YandexOAuthState"];

            var stateCreationTime = DateTime.ParseExact(
    savedState.Split('|')[1],
    "o",
    CultureInfo.InvariantCulture);

            if (DateTime.UtcNow - stateCreationTime > TimeSpan.FromMinutes(5))
            {
                TempData["YandexAuthError"] = "����� ����������� �������";
                return RedirectToAction("Enter");
            }

            // ������� cookie ����� ����� �������������
            Response.Cookies.Delete("YandexOAuthState");

            _logger.LogInformation($"YandexAuth: Generated state = {state}");
            _logger.LogInformation($"YandexCallback: Received state = {state}, savedState = {savedState}");

            if (state != savedState)
            {
                _logger.LogError($"�������������� state ���������. ���������: {savedState}, ��������: {state}");
                TempData["YandexAuthError"] = "������ ������������ ��� �����������";
                return RedirectToAction("Enter");
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var requestContent = new FormUrlEncodedContent(
                [
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("client_id", _configuration["Yandex:ClientId"]),
                    new KeyValuePair<string, string>("client_secret", _configuration["Yandex:ClientSecret"])
                ]);

                var response = await client.PostAsync("https://oauth.yandex.ru/token", requestContent);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"������ ������.OAuth: {errorContent}");
                }

                var tokenResponse = await response.Content.ReadFromJsonAsync<YandexTokenResponse>();

                if (APIClient.User == null)
                {
                    TempData["YandexAuthError"] = "������������ �� �����������";
                    return RedirectToAction("Enter");
                }

                // ��������� �����
                APIClient.User.YandexToken = tokenResponse.access_token;
                APIClient.User.YandexTokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.expires_in);

                // ��������� ������������
                APIClient.PostRequest("api/user/update", APIClient.User);

                TempData["YandexAuthSuccess"] = "������.��������� ������� ���������!";
                return RedirectToAction("Index", "Home"); // �������������� ���� �����
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "������ ����������� ����� ������");
                TempData["YandexAuthError"] = $"������ ��� �����������: {ex.Message}";
                return RedirectToAction("Enter");
            }
        }

        public class YandexTokenResponse
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
        }*/
    }
}