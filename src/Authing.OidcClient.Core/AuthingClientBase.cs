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
    /// <summary>
    /// Authing OIDC Client 基类
    /// 基于 <see cref="IdentityModel.OidcClient.OidcClient"/> 实现了登录和注销功能
    /// </summary>
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

        /// <summary>
        /// 异步 OIDC 登录
        /// </summary>
        /// <param name="extraParameters">额外的 url query 参数，会解析成 Dictionary</param>
        /// <param name="cancellationToken">用于取消任务的 token</param>
        /// <returns>LoginResult 实例</returns>
        public virtual async Task<LoginResult> LoginAsync(object extraParameters = null, CancellationToken cancellationToken = default)
        {
            var frontChannelExtraParameters = ObjectToDictionary(extraParameters);
            var loginRequest = new LoginRequest {
                FrontChannelExtraParameters = frontChannelExtraParameters
            };
            return await OidcClient.LoginAsync(loginRequest, cancellationToken);
        }

        /// <summary>
        /// 异步 OIDC 注销
        /// </summary>
        /// <param name="extraParameters">额外的 url query 参数，会解析成 Dictionary</param>
        /// <param name="cancellationToken">用于取消任务的 token</param>
        /// <returns>BrowserResultType 实例</returns>
        public virtual async Task<BrowserResultType> LogoutAsync(object extraParameters = null, CancellationToken cancellationToken = default)
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
            var result = await _options.Browser.InvokeAsync(browserOptions, cancellationToken);
            return result.ResultType;
        }

        /// <summary>
        /// 通过 AuthingClientOptions 实例创建 OidcClientOptions 实例
        /// </summary>
        /// <param name="options">AuthingClientOptions 实例</param>
        /// <returns>OidcClientOptions 实例</returns>
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

        /// <summary>
        /// 将任意类型转为字典
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
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
