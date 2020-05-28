using Authing.OidcClient;
using IdentityModel.Client;
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

        public async Task<BrowserResultType> LogoutAsync(object extraParameters = null, CancellationToken cancellationToken = default)
        {
            var logoutParameters = ObjectToDictionary(extraParameters);
            logoutParameters["client_id"] = OidcClient.Options.ClientId;
            logoutParameters["redirect_uri"] = OidcClient.Options.PostLogoutRedirectUri;

            var endSessionUrl = new RequestUrl($"https://{_options.AppDomain}/login/profile/logout").Create(logoutParameters);

            var logoutRequest = new LogoutRequest();
            var browserOptions = new BrowserOptions(endSessionUrl, _options.PostLogoutRedirectUri ?? string.Empty)
            {
                Timeout = TimeSpan.FromSeconds(logoutRequest.BrowserTimeout),
                DisplayMode = DisplayMode.Hidden
            };
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
