using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Authing.OidcClient;

namespace Winforms
{
    public partial class MainForm : Form
    {
        private Action<string> print;
        private Action clear;
        private AuthingClient _client;

        public MainForm()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
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
        }
    }
}
