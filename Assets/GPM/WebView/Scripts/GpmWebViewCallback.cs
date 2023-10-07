namespace Gpm.WebView
{
    public static class GpmWebViewCallback
    {
        public enum CallbackType
        {
            Open,
            Close,
            PageLoad,
            MultiWindowOpen,
            MultiWindowClose,
            Scheme,
            GoBack,
            GoForward,
            ExecuteJavascript,
            PageStarted,

#if UNITY_ANDROID
            /// <summary>
            /// Used when isBackButtonCloseCallbackUsed is true
            /// </summary>
            BackButtonClose,
#endif
        }

        public delegate void GpmWebViewDelegate(CallbackType type, string data, GpmWebViewError error);
    }
}
