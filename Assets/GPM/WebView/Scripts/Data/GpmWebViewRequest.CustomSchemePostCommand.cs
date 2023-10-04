namespace Gpm.WebView
{
    using System.Collections.Generic;

    public static partial class GpmWebViewRequest
    {
        public class CustomSchemePostCommand
        {
            public List<string> commandList = new List<string>();

            public void Close(string customScheme)
            {
                commandList.Add(string.Join(GpmWebViewCustomSchemeCommand.SEPARATOR, customScheme, GpmWebViewCustomSchemeCommand.COMMAND_CLOSE));
            }

            public void LoadUrl(string customScheme, string url)
            {
                commandList.Add(string.Join(GpmWebViewCustomSchemeCommand.SEPARATOR, customScheme, GpmWebViewCustomSchemeCommand.COMMAND_LOAD_URL, url));
            }

            public void ExecuteJavascript(string customScheme, string script)
            {
                commandList.Add(string.Join(GpmWebViewCustomSchemeCommand.SEPARATOR, customScheme, GpmWebViewCustomSchemeCommand.COMMAND_EXECUTE_JAVASCRIPT, script));
            }
        }
    }
}
