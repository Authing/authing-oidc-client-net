using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class AuthingMessageHandler : DelegatingHandler
    {
        public AuthingMessageHandler()
        {
            InnerHandler = new HttpClientHandler();
        }

        protected async override Task<HttpResponseMessage> SendAsync(
          HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Console.WriteLine("hello");
            Console.WriteLine(request.RequestUri.AbsoluteUri);
            if (request.RequestUri.AbsoluteUri.StartsWith("https://users.authing.cn/oauth/oidc/user/userinfo"))
            {
                Console.WriteLine(request.Headers.Authorization);
                var code = request.Headers.Authorization.Parameter;
                request.RequestUri = new Uri($"https://users.authing.cn/oauth/oidc/user/userinfo?access_token={code}");
            }
            var response = await base.SendAsync(request, cancellationToken);
            Console.WriteLine("bye");
            return response;
        }
    }
}
