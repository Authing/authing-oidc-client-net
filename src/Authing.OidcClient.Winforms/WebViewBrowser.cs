using IdentityModel.OidcClient.Browser;
using Microsoft.Toolkit.Forms.UI.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Authing.OidcClient
{
    /// <summary>
    /// Winforms 的认证窗口实现
    /// </summary>
    public class WebViewBrowser: IBrowser
    {
        private readonly Func<Form> _formFactory;

        /// <summary>
        /// 实例化认证窗口
        /// </summary>
        /// <param name="formFactory">生成窗口的方法</param>
        public WebViewBrowser(Func<Form> formFactory)
        {
            _formFactory = formFactory;
        }

        /// <summary>
        /// 实例化认证窗口
        /// </summary>
        /// <param name="title">窗口标题</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public WebViewBrowser(string title = "认证中...", int width = 1024, int height = 768)
            : this(() => new Form
            {
                Name = "WebAuthentication",
                Text = title,
                Width = width,
                Height = height
            })
        {
        }

        /// <inheritdoc />
        public Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<BrowserResult>();

            var window = _formFactory();
            var webView = new WebViewCompatible { Dock = DockStyle.Fill };
            window.Controls.Add(webView);

            webView.NavigationStarting += (sender, e) =>
            {
                if (e.Uri.AbsoluteUri.StartsWith(options.EndUrl))
                {
                    tcs.SetResult(new BrowserResult { ResultType = BrowserResultType.Success, Response = e.Uri.ToString() });
                    window.Close();
                }
            };

            window.Closing += (sender, e) =>
            {
                if (!tcs.Task.IsCompleted)
                    tcs.SetResult(new BrowserResult { ResultType = BrowserResultType.UserCancel });
            };


            if (options.DisplayMode == DisplayMode.Hidden)
            {
                window.WindowState = FormWindowState.Minimized;
            }
            window.Show();
            webView.Navigate(options.StartUrl);

            return tcs.Task;
        }
    }
}
