using Microsoft.AspNetCore.Authentication.OAuth;

namespace HRProBusinessLogic.Yandex
{
    public class YandexOAuthOptions : OAuthOptions
    {
        public YandexOAuthOptions()
        {
            AuthorizationEndpoint = "https://oauth.yandex.ru/authorize";
            TokenEndpoint = "https://oauth.yandex.ru/token";
            UserInformationEndpoint = "https://login.yandex.ru/info";
            Scope.Add("login:email");
        }
    }
}
