using IdentityModel.OidcClient.Browser;
using System.Net.Http;

namespace Authing.OidcClient
{
    /// <summary>
    /// <see cref="AuthingClientBase"/> 的配置类
    /// </summary>
    public class AuthingClientOptions
    {
        /// <summary>
        /// 实现 <see cref="IBrowser"/> 接口的类的实例，用于调用各平台的登录窗口。<br/>
        /// 默认已经提供了各平台的实现。
        /// </summary>
        public IBrowser Browser { get; set; }

        /// <summary>
        /// OIDC 应用 ID
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// OIDC 应用的域名，不包括协议部分。<br/>
        /// 例如：sign.authing.cn
        /// </summary>
        public string AppDomain { get; set; }

        /// <summary>
        /// OIDC 应用登录成功后的回调地址，需要使用在控制台中填写的。<br/>
        /// 例如：https://authing.cn/guide/oidc/callback
        /// </summary>
        public string RedirectUri { get; set; }

        /// <summary>
        /// OIDC 应用注销成功后的回调地址，需要使用在控制台中填写的。<br/>
        /// 例如：https://authing.cn/guide/oidc/callback
        /// </summary>
        public string PostLogoutRedirectUri { get; set; }

        /// <summary>
        /// 是否在登录成功后获取用户信息，默认为 true。
        /// </summary>
        public bool LoadProfile { get; set; } = true;

        /// <summary>
        /// OIDC 应用申请的权限范围，必须包含 openid。<br/>
        /// 默认为 "openid profile email phone"
        /// </summary>
        public string Scope { get; set; } = "openid profile email phone";

        /// <summary>
        /// OIDC 应用响应类型，默认为 "code"
        /// </summary>
        public string ResponseType { get; set; } = "code";

        /// <summary>
        /// 可以覆盖默认的 RefreshTokenMessageHandler
        /// </summary>
        /// <example>
        /// var handler = new HttpClientHandler();
        /// var options = new AuthingClientOptions
        /// {
        ///    RefreshTokenMessageHandler = handler
        /// };
        /// </example>
        public HttpMessageHandler RefreshTokenMessageHandler { get; set; }

        /// <summary>
        /// 可以覆盖默认的 BackchannelHandler.
        /// </summary>
        /// <example>
        /// <code>
        /// var handler = new HttpClientHandler();
        /// var options = new AuthingClientOptions
        /// {
        ///    BackchannelHandler = handler
        /// };
        /// </code>
        /// </example>
        public HttpMessageHandler BackchannelHandler { get; set; }

        /// <summary>
        /// 实例化 <see cref="AuthingClientBase"/> 的配置
        /// </summary>
        public AuthingClientOptions()
        {
        }
    }
}
