using Authing.OidcClient;
using System;
using System.Windows.Forms;

namespace Winforms
{
    public partial class MainForm : Form
    {
        private Action<string> print;
        private Action clear;
        private AuthingClient _client;
        private string accessToken;
        private string refreshToken;

        public MainForm()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            // 至少提供下面的四个参数
            _client = new AuthingClient(new AuthingClientOptions() {
                AppId = "5e72d72e3798fb03e1d57b13",
                AppDomain = "111.authing.cn",
                RedirectUri = "https://authing.cn/guide/oidc/callback",
                PostLogoutRedirectUri = "https://authing.cn/guide/oidc/callback",
            });
            print = (str) => TxtLog.AppendText(str + "\r\n");
            clear = () => TxtLog.Clear();
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            clear();
            print("开始登录...");

            var result = await _client.LoginAsync();
            
            if (result.IsError)
            {
                print($"登录时出现错误，错误原因是：{result.Error}");
                return;
            }

            accessToken = result.AccessToken;
            refreshToken = result.RefreshToken;

            print($"id_token: {result.IdentityToken}");
            print($"access_token: {result.AccessToken}");
            print($"refresh_token: {result.RefreshToken}");

            foreach (var claim in result.User.Claims)
            {
                print($"{claim.Type} = {claim.Value}");
            }
        }

        private async void BtnLogout_Click(object sender, EventArgs e)
        {
            clear();
            print("开始注销...");

            var result = await _client.LogoutAsync();
            print(result.ToString());

            if (result == IdentityModel.OidcClient.Browser.BrowserResultType.Success)
            {
                accessToken = null;
                refreshToken = null;
            }
        }

        private async void BtnCheck_Click(object sender, EventArgs e)
        {
            clear();
            if (accessToken == null)
            {
                print("请先登录");
                return;
            }

            print($"当前 access token 为 {accessToken}，正在验证...");
            var result = await _client.GetUserInfoAsync(accessToken);
            
            if (result.IsError)
            {
                print($"验证失败，原因是：{result.Error}");
                return;
            }

            print("验证成功，access token 有效");
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            clear();
            if (refreshToken == null)
            {
                print("请先登录");
                return;
            }

            print($"当前 refresh token 为 {refreshToken}，正在刷新...");
            var result = await _client.RefreshTokenAsync(refreshToken);

            if (result.IsError)
            {
                print($"刷新失败，原因是：{result.Error}");
                return;
            }

            print($"刷新成功，新的 access token 为：{result.AccessToken}");
        }
    }
}
