using IdentityModel.OidcClient.Browser;
using Microsoft.Toolkit.Forms.UI.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Authing.OidcClient
{
    public class WebViewBrowser : IBrowser
    {
        private readonly Func<Window> _windowFactory;
        private readonly bool _shouldCloseWindow;

        public WebViewBrowser(Func<Window> windowFactory, bool shouldCloseWindow = true)
        {
            _windowFactory = windowFactory;
            _shouldCloseWindow = shouldCloseWindow;
        }

        public WebViewBrowser(string title = "认证中...", int width = 1024, int height = 768)
            : this(() => new Window
            {
                Name = "WebAuthentication",
                Title = title,
                Width = width,
                Height = height
            })
        {
        }

        /// <inheritdoc />
        public Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<BrowserResult>();

            var window = _windowFactory();
            var webView = new WebViewCompatible();
            window.Content = webView;

            webView.NavigationStarting += (sender, e) =>
            {
                if (e.Uri.AbsoluteUri.StartsWith(options.EndUrl))
                {
                    tcs.SetResult(new BrowserResult { ResultType = BrowserResultType.Success, Response = e.Uri.ToString() });
                    if (_shouldCloseWindow)
                        window.Close();
                    else
                        window.Content = null;
                }
            };

            window.Closing += (sender, e) =>
            {
                if (!tcs.Task.IsCompleted)
                    tcs.SetResult(new BrowserResult { ResultType = BrowserResultType.UserCancel });
            };

            if (options.DisplayMode == DisplayMode.Hidden)
            {
                Console.WriteLine("123");
                window.WindowState = WindowState.Minimized;
            }
            window.Show();
            webView.Navigate(options.StartUrl);

            return tcs.Task;
        }
    }
}
