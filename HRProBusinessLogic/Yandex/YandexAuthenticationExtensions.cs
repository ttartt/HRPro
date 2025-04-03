using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace HRProBusinessLogic.Yandex
{
    public static class YandexAuthenticationExtensions
    {
        public static AuthenticationBuilder AddYandex(this AuthenticationBuilder builder,
            Action<YandexOAuthOptions> configureOptions)
        {
            return builder.AddOAuth<YandexOAuthOptions, YandexOAuthHandler>(
                "Yandex", configureOptions);
        }
    }
}
