using IdentityModel.OidcClient.Browser;
using System;
using System.Net.Http;

namespace Authing.OidcClient
{
    public class AuthingClientOptions
    {
        public IBrowser Browser { get; set; }

        public string AppId { get; private set; }

        public string AppDomain { get; private set; }

        public string RedirectUri { get; set; }

        public string PostLogoutRedirectUri { get; set; }

        public bool LoadProfile { get; set; } = true;

        public string Scope { get; set; } = "openid profile email phone";

        public string ResponseType { get; set; } = "code";

        /// <summary>
        /// Allow overriding the RetryMessageHandler.
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
        /// Allow overriding the BackchannelHandler.
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

        public AuthingClientOptions()
        {
        }
    }
}
