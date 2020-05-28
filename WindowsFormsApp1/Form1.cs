using IdentityModel.Jwk;
using IdentityModel.OidcClient;
using Serilog;
using System;
using System.Net.Http;
using System.Windows.Forms;
using static IdentityModel.OidcClient.OidcClientOptions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            test();
        }

        private async void test()
        {
            var options = new OidcClientOptions
            {
                Authority = "https://111.authing.cn/oauth/oidc",
                ClientId = "5e72d72e3798fb03e1d57b13",
                // ClientSecret = "931f19ce2161e5560c072f586c706ee6",
                RedirectUri = "https://authing.cn/guide/oidc/callback",
                Scope = "openid profile email phone",
                Browser = new WebViewBrowser(),

                BackchannelHandler = new AuthingMessageHandler(),
            };

            //var keys = new JsonWebKeySet();
            //keys.Keys.Add(new JsonWebKey("{ \"kty\":\"RSA\",\"e\":\"AQAB\",\"kid\":\"893ed9df-80f0-4d40-b697-732b6e267d18\",\"n\":\"xRijj2seoesv5K0Z-ymRK7DSDPxdsM2sGQD2ZVhLjLsxZWJtXUXh7ERdUU6OT3BqYZZf7CLIhN6yyNtTOgfgpLG9HVJd7ZSKzuy2dS7mo8jD8YRtptAJmNFqw6z8tQp5MNG1ZHqp9isKqJmx_CFYkRdXBmjjj8PMVSP757pkC3jCq7fsi0drSSg4lIxrSsGzL0--Ra9Du71Qe_ODQKU0brxaI1OKILtfcVPTHTaheV-0dw4eYkSDtyaLBG3jqsQbdncNg8PCEWchNzdO6aajUq4wbOzy_Ctp399mz0SGKfuC5S8gqAFABFT3DH3UD21ZztQZwFEV2AlvF-bcGEstcw\"}"));

            //options.ProviderInformation = new ProviderInformation
            //{
            //    IssuerName = "https://111.authing.cn/oauth/oidc",
            //    AuthorizeEndpoint = "https://111.authing.cn/oauth/oidc/auth",
            //    TokenEndpoint = "https://111.authing.cn/oauth/oidc/token",
            //    UserInfoEndpoint = "https://users.authing.cn/oauth/oidc/user/userinfo",

            //    KeySet = keys
            //};

            var client = new OidcClient(options);
            var state = await client.LoginAsync(new LoginRequest() {
            });
            Console.WriteLine(state);
        }
    }
}
