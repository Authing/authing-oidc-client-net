using IdentityModel.OidcClient.Browser;
using Microsoft.Toolkit.Forms.UI.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Authing.OidcClient
{
    public class WebViewBrowser: IBrowser
    {
        private readonly Func<Form> _formFactory;

        public WebViewBrowser(Func<Form> formFactory)
        {
            _formFactory = formFactory;
        }

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


            window.Controls.Add(webView);
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
