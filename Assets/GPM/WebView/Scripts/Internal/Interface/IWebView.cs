namespace Gpm.WebView.Internal
{
    using System.Collections.Generic;

    public interface IWebView
    {
        void ShowUrl(
            string url,
            GpmWebViewRequest.Configuration configuration,
            GpmWebViewCallback.GpmWebViewDelegate callback,
            List<string> schemeList);

        void ShowHtmlFile(
            string filePath,
            GpmWebViewRequest.Configuration configuration,
            GpmWebViewCallback.GpmWebViewDelegate callback,
            List<string> schemeList);

        void ShowHtmlString(
            string htmlString,
            GpmWebViewRequest.Configuration configuration,
            GpmWebViewCallback.GpmWebViewDelegate callback,
            List<string> schemeList);

        void ShowSafeBrowsing(
            string url,
            GpmWebViewRequest.ConfigurationSafeBrowsing configuration = null,
            GpmWebViewCallback.GpmWebViewDelegate callback = null);

        void Close();
        bool IsActive();
        void ExecuteJavaScript(string script);
        void SetFileDownloadPath(string path);

        bool CanGoBack { get; }
        bool CanGoForward { get; }
        void GoBack();
        void GoForward();

        void SetPosition(int x, int y);
        void SetSize(int x, int y);
        void SetMargins(int left, int top, int right, int bottom);

        int GetX();
        int GetY();
        int GetWidth();
        int GetHeight();

        void ShowWebBrowser(string url);
    }
}
