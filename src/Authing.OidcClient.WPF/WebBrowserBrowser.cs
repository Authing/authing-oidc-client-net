using IdentityModel.OidcClient.Browser;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Authing.OidcClient
{
    /// <summary>
    /// 通过已有的浏览器进行认证
    /// </summary>
    public class WebBrowserBrowser : IBrowser
    {
        private readonly WebBrowser webBrowser;

        /// <summary>
        /// 实例化认证浏览器
        /// </summary>
        /// <param name="webBrowser">浏览器控件</param>
        public WebBrowserBrowser(WebBrowser webBrowser)
        {
            this.webBrowser = webBrowser;
        }

        /// <summary>
        /// 异步执行
        /// </summary>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<BrowserResult>();

            NavigatingCancelEventHandler handler = (sender, e) =>
            {
                if (e.Uri.AbsoluteUri.StartsWith(options.EndUrl))
                {
                    tcs.SetResult(new BrowserResult { ResultType = BrowserResultType.Success, Response = e.Uri.ToString() });
                }
            };

            webBrowser.Navigating += handler;
            webBrowser.Navigate(options.StartUrl);

            return tcs.Task.ContinueWith((result) =>
            {
                webBrowser.Navigating -= handler;
                return result.Result;
            });
        }
    }
}
