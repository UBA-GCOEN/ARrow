namespace Gpm.WebView
{
    public static class GpmWebViewCustomSchemeCommand
    {
        /// <summary>
        /// Close currently displayed WebView.
        /// </summary>
        public const string COMMAND_CLOSE = "close";

        /// <summary>
        /// Load url from webview.
        /// </summary>
        public const string COMMAND_LOAD_URL = "loadUrl";

        /// <summary>
        /// Execute the specified JavaScript string.
        /// </summary>
        public const string COMMAND_EXECUTE_JAVASCRIPT = "executeJavascript";

        /// <summary>
        /// Distinguish instructions and parameters.
        /// </summary>
        public const string SEPARATOR = "|";
    }
}
