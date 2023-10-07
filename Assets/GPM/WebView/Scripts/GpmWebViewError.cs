using Gpm.Common.ThirdParty.LitJson;

namespace Gpm.WebView
{
    public class GpmWebViewError
    {
        public string domain;
        public int code;
        public string message;

        public GpmWebViewError error;

        public override string ToString()
        {
            return JsonMapper.ToJson(this);
        }
    }
}