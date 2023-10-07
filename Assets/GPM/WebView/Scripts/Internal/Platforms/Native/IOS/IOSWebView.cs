namespace Gpm.WebView.Internal
{
    public class IOSWebView : NativeWebView
    {
        private const string IOS_CLASS_NAME = "GPMWebViewPlugin";

        override protected void Initialize()
        {
            CLASS_NAME = IOS_CLASS_NAME;
            base.Initialize();
        }
    }
}
