using System;

namespace Authing.OidcClient
{
    /// <summary>
    /// 基于 <see cref="IdentityModel.OidcClient.OidcClient"/> 实现 Authing OIDC 认证功能的主类
    /// </summary>
    public class AuthingClient : AuthingClientBase
    {
        /// <summary>
        /// 实例化 <see cref="AuthingClient"/>
        /// </summary>
        /// <param name="options"><see cref="AuthingClientOptions"/> 实例</param>
        public AuthingClient(AuthingClientOptions options)
            : base(options)
        {
            options.Browser = options.Browser ?? new WebViewBrowser();
        }
    }
}
