namespace Gpm.WebView.Internal
{
    public class AndroidWebView : NativeWebView
    {
        private const string ANDROID_CLASS_NAME = "com.gpm.webviewplugin.GpmWebViewPlugin";

        override protected void Initialize()
        {
            CLASS_NAME = ANDROID_CLASS_NAME;
            base.Initialize();
        }
    }
}
