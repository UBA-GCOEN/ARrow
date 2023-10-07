namespace Gpm.WebView
{
    using System.Collections.Generic;
    using Gpm.WebView.Internal;

    public static class GpmWebView
    {
        public const string VERSION = "2.0.0";

        /// <summary>
        /// Create the webview and loads the web content referenced by the specified URL.
        /// </summary>
        /// <param name="url">The URL of the resource to load.</param>
        /// <param name="configuration">The configuration of GPM WebWiew. Refer to <see cref="GpmWebViewRequest.Configuration"/></param>
        /// <param name="callback">Notifies users events.</param>
        /// <param name="schemeList">Specifies the list of customized schemes a user wants.</param>
        public static void ShowUrl(
            string url,
            GpmWebViewRequest.Configuration configuration,
            GpmWebViewCallback.GpmWebViewDelegate callback,
            List<string> schemeList)
        {
            WebViewImplementation.Instance.ShowUrl(url, configuration, callback, schemeList);
        }

        /// <summary>
        /// Create the webview and loads the web content from the specified file.
        /// </summary>
        /// <param name="filePath">The URL of a file that contains web content. This URL must be a file-based URL.</param>
        /// <param name="configuration">The configuration of GPM WebWiew. Refer to <see cref="GpmWebViewRequest.Configuration"/></param>
        /// <param name="callback">Notifies users events.</param>
        /// <param name="schemeList">Specifies the list of customized schemes a user wants.</param>
        public static void ShowHtmlFile(
            string filePath,
            GpmWebViewRequest.Configuration configuration,
            GpmWebViewCallback.GpmWebViewDelegate callback,
            List<string> schemeList)
        {
            WebViewImplementation.Instance.ShowHtmlFile(filePath, configuration, callback, schemeList);
        }

        /// <summary>
        /// Create the webview and loads the contents of the specified HTML string.
        /// </summary>
        /// <param name="htmlString">The string to use as the contents of the webpage.</param>
        /// <param name="configuration">The configuration of GPM WebWiew. Refer to <see cref="GpmWebViewRequest.Configuration"/></param>
        /// <param name="callback">Notifies users events.</param>
        /// <param name="schemeList">Specifies the list of customized schemes a user wants.</param>
        public static void ShowHtmlString(
            string htmlString,
            GpmWebViewRequest.Configuration configuration,
            GpmWebViewCallback.GpmWebViewDelegate callback,
            List<string> schemeList)
        {
            WebViewImplementation.Instance.ShowHtmlString(htmlString, configuration, callback, schemeList);
        }

        /// <summary>
        /// Execute the specified JavaScript string.
        /// </summary>
        /// <param name="script">The JavaScript string to execute.</param>
        public static void ExecuteJavaScript(string script)
        {
            WebViewImplementation.Instance.ExecuteJavaScript(script);
        }

        /// <summary>
        /// Close currently displayed WebView.
        /// </summary>
        public static void Close()
        {
            WebViewImplementation.Instance.Close();
        }

        public static bool IsActive()
        {
            return WebViewImplementation.Instance.IsActive();
        }

        public static bool CanGoBack()
        {
            return WebViewImplementation.Instance.CanGoBack();
        }

        public static bool CanGoForward()
        {
            return WebViewImplementation.Instance.CanGoForward();
        }

        public static void GoBack()
        {
            WebViewImplementation.Instance.GoBack();
        }

        public static void GoForward()
        {
            WebViewImplementation.Instance.GoForward();
        }

        public static void SetPosition(int x, int y)
        {
            WebViewImplementation.Instance.SetPosition(x, y);
        }

        public static void SetSize(int width, int height)
        {
            WebViewImplementation.Instance.SetSize(width, height);
        }

        public static void SetMargins(int left, int top, int right, int bottom)
        {
            WebViewImplementation.Instance.SetMargins(left, top, right, bottom);
        }

        public static int GetX()
        {
            return WebViewImplementation.Instance.GetX();
        }

        public static int GetY()
        {
            return WebViewImplementation.Instance.GetY();
        }

        public static int GetWidth()
        {
            return WebViewImplementation.Instance.GetWidth();
        }

        public static int GetHeight()
        {
            return WebViewImplementation.Instance.GetHeight();
        }

        /// <summary>
        /// Open a Web Browser with the specified URL.
        /// </summary>
        /// <param name="url">The URL of the resource to load.</param>
        public static void ShowWebBrowser(string url)
        {
            WebViewImplementation.Instance.ShowWebBrowser(url);
        }
    }
}
