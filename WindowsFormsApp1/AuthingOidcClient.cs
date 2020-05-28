using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using IdentityModel.OidcClient.Results;
using static IdentityModel.OidcClient.OidcClientOptions;

namespace WindowsFormsApp1
{
    public class AuthingOidcClient : OidcClient
    {
        public AuthingOidcClient(OidcClientOptions options) : base(options)
        {
        }
    }
}
