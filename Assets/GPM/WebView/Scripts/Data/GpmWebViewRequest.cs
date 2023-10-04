namespace Gpm.WebView
{
    public static partial class GpmWebViewRequest
    {
        /// <summary>
        /// Position of popup style webView
        /// </summary>
        public struct Position
        {
            public bool hasValue;
            public int x;
            public int y;
        }

        /// <summary>
        /// Size of popup style webView
        /// </summary>
        public struct Size
        {
            public bool hasValue;
            public int width;
            public int height;
        }

        /// <summary>
        /// Margins of popup style webView
        /// </summary>
        public struct Margins
        {
            public bool hasValue;
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        public class Configuration
        {
            /// <summary>
            /// These constants indicate the type of launch style such as popup, fullscreen.
            /// Refer to <see cref="GpmWebViewStyle"/>
            /// </summary>
            public int style;

            /// <summary>
            /// These constants indicate the type of screen orientation.
            /// /// Refer to <see cref="GpmWebViewOrientation"/>
            /// </summary>
            public int orientation;

            /// <summary>
            /// Clear cookies.
            /// </summary>
            public bool isClearCookie;

            /// <summary>
            /// Clear cache.
            /// </summary>
            public bool isClearCache;

            /// <summary>
            /// Set the background color of the WebView.
            /// </summary>
            public string backgroundColor = "#FFFFFF";

            /// <summary>
            /// Sets the visibility of the navigation bar.
            /// Close button visibility of iOS popup style
            /// </summary>
            public bool isNavigationBarVisible;

            /// <summary>
            /// Sets the color of the navigation bar.
            /// (e.g. #000000 ~ #FFFFFF)
            /// Default: #4B96E6
            /// </summary>
            public string navigationBarColor = "#4B96E6";

            /// <summary>
            /// The page title.
            /// </summary>
            public string title;

            /// <summary>
            /// Sets the visibility of the back button.
            /// </summary>
            public bool isBackButtonVisible;

            /// <summary>
            /// Sets the visibility of the forward button.
            /// </summary>
            public bool isForwardButtonVisible;

            /// <summary>
            /// Sets the visibility of the close button.
            /// </summary>
            public bool isCloseButtonVisible;

            /// <summary>
            /// Sets whether the WebView whether supports multiple windows.
            /// </summary>
            public bool supportMultipleWindows;

            /// <summary>
            /// custom userAgent string
            /// </summary>
            public string userAgentString = "";

            /// <summary>
            /// Add javascript to webView(Javascript injection)
            /// </summary>
            public string addJavascript = "";

            /// <summary>
            /// Native command executed after receiving custom scheme.
            /// </summary>
            public CustomSchemePostCommand customSchemePostCommand;

            /// <summary>
            /// Only used in style of Popup.
            /// Sets the webView position.
            /// </summary>
            public Position position;

            /// <summary>
            /// Only used in style of Popup.
            /// Sets the webView size.
            /// </summary>
            public Size size;

            /// <summary>
            /// Only used in style of Popup.
            /// Sets the webView margins.
            /// </summary>
            public Margins margins;

            /// <summary>
            /// Android only.
            /// The webview does not close and notified by a callback.
            /// </summary>
            public bool isBackButtonCloseCallbackUsed;

            /// <summary>
            /// iOS only.
            /// The content mode for the web view to use when it loads and renders a webpage.
            /// Refer to <see cref="GpmWebViewContentMode"/>
            /// </summary>
            public int contentMode;

            /// <summary>
            /// iOS only.
            /// Only used in style of Popup.
            /// Sets the visibility of the mask view.
            /// </summary>
            public bool isMaskViewVisible;

            /// <summary>
            /// iOS only.
            /// Sets the auto orientation of the web view.
            /// </summary>
            public bool isAutoRotation;
        }

        public class ConfigurationSafeBrowsing
        {
            /// <summary>
            /// Sets the color of the navigation bar.
            /// (e.g. #000000 ~ #FFFFFF)
            /// Default: #4B96E6
            /// </summary>
            public string navigationBarColor = "#4B96E6";

            /// <summary>
            /// iOS only.
            /// Sets the color of the navigation text.
            /// </summary>
            public string navigationTextColor = "#FFFFFF";
        }
    }
}
