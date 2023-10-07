using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gpm.Common.Indicator.Internal
{
    public class IndicatorImplementation
    {
        private static readonly IndicatorImplementation instance = new IndicatorImplementation();

        public static IndicatorImplementation Instance
        {
            get { return instance; }
        }

        private BaseIndicator Indicator
        {
            get { return GetIndicatorInstance(); }
        }

        private InAppIndicator inAppInstance;
        private EditorIndicator editorInstance;

        public IndicatorImplementation()
        {
        }

        public void Send(string serviceName, string serviceVersion, Dictionary<string, string> customData, bool ignoreActivation = false)
        {
            if (string.IsNullOrEmpty(serviceName) == true
                || string.IsNullOrEmpty(serviceVersion) == true
                || customData == null)
            {
                return;
            }

            string body = string.Format("[GPM] {0} v{1}", serviceName, serviceVersion);
            Indicator.Send(serviceName, serviceVersion, body, customData, ignoreActivation);
        }

        private BaseIndicator GetIndicatorInstance()
        {
            var isInApp = true;

#if UNITY_EDITOR
            if (EditorApplication.isPlaying == false)
            {
                isInApp = false;
            }
#endif
            if (isInApp == true)
            {
                if (inAppInstance == null)
                {
                    inAppInstance = new InAppIndicator();
                }

                return inAppInstance;
            }
            else
            {
                if (editorInstance == null)
                {
                    editorInstance = new EditorIndicator();
                }

                return editorInstance;
            }
        }
    }
}
