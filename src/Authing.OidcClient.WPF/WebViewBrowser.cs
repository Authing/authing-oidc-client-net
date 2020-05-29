using IdentityModel.OidcClient.Browser;
using Microsoft.Toolkit.Wpf.UI.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Authing.OidcClient
{
    /// <summary>
    /// WPF 的认证窗口实现
    /// </summary>
    public class WebViewBrowser : IBrowser
    {
        private readonly Func<Window> _windowFactory;

        /// <summary>
        /// 实例化认证窗口
        /// </summary>
        /// <param name="windowFactory">生成窗口的方法</param>
        public WebViewBrowser(Func<Window> windowFactory)
        {
            _windowFactory = windowFactory;
        }

        /// <summary>
        /// 实例化认证窗口
        /// </summary>
        /// <param name="title">窗口标题</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
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
                window.WindowState = WindowState.Minimized;
            }
            window.Show();
            webView.Navigate(options.StartUrl);

            return tcs.Task;
        }
    }
}
