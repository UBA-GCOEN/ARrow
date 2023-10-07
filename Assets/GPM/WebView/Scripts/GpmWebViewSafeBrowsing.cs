namespace Gpm.WebView
{
    using System.Collections.Generic;
    using Gpm.WebView.Internal;

    public static class GpmWebViewSafeBrowsing
    {
        /// <summary>
        /// Open the Chrome for Android or Safari for iOS and loads web content referenced by the specified url.
        /// </summary>
        /// <param name="url">The URL of the resource to load.</param>
        /// <param name="configuration">The configuration of Custom tabs. Refer to <see cref="GpmWebViewRequest.ConfigurationSafeBrowsing"/></param>
        /// <param name="callback">Notifies users events.</param>
        public static void ShowSafeBrowsing(
            string url,
            GpmWebViewRequest.ConfigurationSafeBrowsing configuration = null,
            GpmWebViewCallback.GpmWebViewDelegate callback = null)
        {
            WebViewImplementation.Instance.ShowSafeBrowsing(url, configuration, callback);
        }
    }
}
