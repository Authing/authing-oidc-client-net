using System;
using System.Collections.Generic;
using System.Text;

namespace Authing.OidcClient
{
    using IdentityModel.OidcClient;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    namespace Authing.OidcClient
    {
        public class DefaultBackchannelHandler : DelegatingHandler
        {
            private OidcClientOptions _options;

            public DefaultBackchannelHandler(OidcClientOptions options)
            {
                _options = options;
                InnerHandler = new HttpClientHandler();
            }

            protected async override Task<HttpResponseMessage> SendAsync(
              HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (_options?.ProviderInformation?.UserInfoEndpoint != null)
                {
                    var userInfoEndpoint = _options.ProviderInformation.UserInfoEndpoint;

                    if (request.RequestUri.AbsoluteUri.StartsWith(userInfoEndpoint))
                    {
                        var code = request.Headers.Authorization.Parameter;
                        request.RequestUri = new Uri($"{userInfoEndpoint}?access_token={code}");
                    }
                }
                return await base.SendAsync(request, cancellationToken);
            }
        }
    }

}
