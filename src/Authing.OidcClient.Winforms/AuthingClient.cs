using System;

namespace Authing.OidcClient
{
    public class AuthingClient : AuthingClientBase
    {
        public AuthingClient(AuthingClientOptions options)
            : base(options)
        {
            options.Browser = options.Browser ?? new WebViewBrowser();
        }
    }
}
