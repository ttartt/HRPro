using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Newtonsoft.Json.Linq;

namespace HRProBusinessLogic.Yandex
{
    public class YandexOAuthHandler : OAuthHandler<YandexOAuthOptions>
    {
        public YandexOAuthHandler(
            IOptionsMonitor<YandexOAuthOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {}

        protected override async Task<AuthenticationTicket> CreateTicketAsync(
            ClaimsIdentity identity,
            AuthenticationProperties properties,
            OAuthTokenResponse tokens)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Options.UserInformationEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

            var response = await Backchannel.SendAsync(request, Context.RequestAborted);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var user = JObject.Parse(content);

            var context = new OAuthCreatingTicketContext(
                new ClaimsPrincipal(identity),
                properties,
                Context,
                Scheme,
                Options,
                Backchannel,
                tokens,
                user);

            context.RunClaimActions();
            await Events.CreatingTicket(context);

            return new AuthenticationTicket(
                context.Principal,
                context.Properties,
                Scheme.Name);
        }
    }
}
