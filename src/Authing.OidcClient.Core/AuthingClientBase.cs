using Authing.OidcClient.Authing.OidcClient;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using static IdentityModel.OidcClient.OidcClientOptions;

namespace Authing.OidcClient
{
    public abstract class AuthingClientBase
    {
        private readonly AuthingClientOptions _options;
        private IdentityModel.OidcClient.OidcClient _oidcClient;
        private IdentityModel.OidcClient.OidcClient OidcClient
        {
            get
            {
                return _oidcClient ?? (_oidcClient = new IdentityModel.OidcClient.OidcClient(CreateOidcClientOptions(_options)));
            }
        }

        public AuthingClientBase(AuthingClientOptions options)
        {
            _options = options;
        }

        public async Task<LoginResult> LoginAsync(object extraParameters = null, CancellationToken cancellationToken = default)
        {
            var frontChannelExtraParameters = ObjectToDictionary(extraParameters);
            var loginRequest = new LoginRequest {
                FrontChannelExtraParameters = frontChannelExtraParameters
            };
            return await OidcClient.LoginAsync(loginRequest, cancellationToken);
        }

        public async Task<BrowserResultType> LogoutAsync(CancellationToken cancellationToken = default)
        {
            var endSessionUrl = $"https://${_options.AppDomain}/login/profile/logout?app_id=${_options.AppId}&redirect_uri=${_options.PostLogoutRedirectUri}";

            var browserOptions = new BrowserOptions(endSessionUrl, _options.PostLogoutRedirectUri ?? string.Empty);
            var result = await _options.Browser.InvokeAsync(browserOptions);
            return result.ResultType;
        }

        private OidcClientOptions CreateOidcClientOptions(AuthingClientOptions options)
        {
            var scopes = options.Scope.Split(' ').ToList();
            if (!scopes.Contains("openid"))
                scopes.Insert(0, "openid");

            var oidcClientOptions = new OidcClientOptions
            {
                Authority = $"https://{options.AppDomain}/oauth/oidc",
                ClientId = options.AppId,
                Scope = String.Join(" ", scopes),
                LoadProfile = options.LoadProfile,
                Browser = options.Browser,
                Flow = AuthenticationFlow.AuthorizationCode,
                ResponseMode = AuthorizeResponseMode.Redirect,
                RedirectUri = options.RedirectUri,
                PostLogoutRedirectUri = options.PostLogoutRedirectUri,

                Policy = {
                    RequireAuthorizationCodeHash = false,
                    RequireAccessTokenHash = false
                }
            };

            if (options.RefreshTokenMessageHandler != null)
                oidcClientOptions.RefreshTokenInnerHttpHandler = options.RefreshTokenMessageHandler;

            if (options.BackchannelHandler != null)
                oidcClientOptions.BackchannelHandler = options.BackchannelHandler;
            else
                oidcClientOptions.BackchannelHandler = new DefaultBackchannelHandler(oidcClientOptions);

            return oidcClientOptions;
        }

        private Dictionary<string, string> ObjectToDictionary(object values)
        {
            if (values is Dictionary<string, string> dictionary)
                return dictionary;

            dictionary = new Dictionary<string, string>();
            if (values != null)
                foreach (var prop in values.GetType().GetRuntimeProperties())
                {
                    var value = prop.GetValue(values) as string;
                    if (!string.IsNullOrEmpty(value))
                        dictionary.Add(prop.Name, value);
                }

            return dictionary;
        }
    }
}
