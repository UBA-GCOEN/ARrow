using Gpm.Common.Indicator;
using Gpm.Common.Multilanguage;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Globalization;

namespace Gpm.Manager.Internal
{
    using Constant;
    using Util;

    internal static class GpmManagerIndicator
    {
        private const string KEY_ACTION = "ACTION";
        private const string KEY_GUID = "GUID";
        private const string KEY_ACTION_DETAIL_1 = "ACTION_DETAIL_1";
        private const string KEY_ACTION_DETAIL_2 = "ACTION_DETAIL_2";
        private const string KEY_ACTION_DETAIL_3 = "ACTION_DETAIL_3";

        private const string ACTION_AD = "Ad";
        private const string ACTION_INSTALL = "Install";
        private const string ACTION_UPDATE = "Update";
        private const string ACTION_REMOVE  = "Remove";
        private const string ACTION_LINK = "Link";
        private const string ACTION_ACTIVATION = "Activation";
        private const string ACTION_RUNTIME = "Runtime";
        
        private static string guid;

        internal static string GetGuid()
        {
            if (string.IsNullOrEmpty(guid) == true)
            {
                if (EditorPrefs.HasKey(ManagerInfos.DEVICE_GUID_KEY) == true)
                {
                    GUID value;
                    if (GUID.TryParse((EditorPrefs.GetString(ManagerInfos.DEVICE_GUID_KEY)), out value) == true)
                    {
                        guid = value.ToString();
                    }
                }
            }

            if (string.IsNullOrEmpty(guid) == true)
            {
                guid = GUID.Generate().ToString();
                EditorPrefs.SetString(ManagerInfos.DEVICE_GUID_KEY, guid);
            }

            return guid;

        }
        
        internal static string GetCurtureCode()
        {
#if UNITY_EDITOR_WIN
            string curture = CultureInfo.CurrentCulture.Name;
            if (string.IsNullOrEmpty(curture) == true)
            {
                curture = GpmMultilanguage.GetSystemLanguage(false);
            }
#else
            string curture = GpmMultilanguage.GetSystemLanguage(false);
#endif
            if (string.IsNullOrEmpty(curture) == true)
            {
                curture = Application.systemLanguage.ToString();
            }

            return curture;
        }
        
        public static void SendAd(string name, string linkUrl)
        {
            Send(new Dictionary<string, string>() 
            {
                { KEY_ACTION,             ACTION_AD },
                { KEY_GUID,               GetGuid() },
                { KEY_ACTION_DETAIL_1,    name },
                { KEY_ACTION_DETAIL_2,    linkUrl },
            });
        }

        public static void SendLink(string serviceName, string linkName, string linkUrl)
        {
            Send(new Dictionary<string, string>()
            {
                { KEY_ACTION,             ACTION_LINK },
                { KEY_GUID,               GetGuid() },
                { KEY_ACTION_DETAIL_1,    serviceName },
                { KEY_ACTION_DETAIL_2,    linkName },
                { KEY_ACTION_DETAIL_3,    linkUrl },
            });
        }

        public static void SendInstall(string serviceName, string serviceVersion, string installName)
        {
            Send(new Dictionary<string, string>()
            {
                { KEY_ACTION,             ACTION_INSTALL },
                { KEY_GUID,               GetGuid() },
                { KEY_ACTION_DETAIL_1,    serviceName },
                { KEY_ACTION_DETAIL_2,    serviceVersion },
                { KEY_ACTION_DETAIL_3,    installName },
            });
        }

        public static void SendUpdate(string serviceName, string serviceVersion, string beforeVersion)
        {
            Send(new Dictionary<string, string>()
            {
                { KEY_ACTION,             ACTION_UPDATE },
                { KEY_GUID,               GetGuid() },
                { KEY_ACTION_DETAIL_1,    serviceName },
                { KEY_ACTION_DETAIL_2,    serviceVersion},
                { KEY_ACTION_DETAIL_3,    beforeVersion  },
            });
        }

        public static void SendRemove(string serviceName, string serviceVersion)
        {
            Send(new Dictionary<string, string>()
            {
                { KEY_ACTION,             ACTION_REMOVE },
                { KEY_GUID,               GetGuid() },
                { KEY_ACTION_DETAIL_1,    serviceName },
                { KEY_ACTION_DETAIL_2,    serviceVersion },
            });
        }

        public static void SendActivation()
        {
            Send(new Dictionary<string, string>()
            {
                { KEY_ACTION,             ACTION_ACTIVATION },
                { KEY_GUID,               GetGuid() },
                { KEY_ACTION_DETAIL_1,    ManagerTime.ToLogString(ManagerTime.Now) },
                { KEY_ACTION_DETAIL_2,    GetCurtureCode() },
            });
        }

        public static void SendRunTime(string startTime, string endTime, string totalTime)
        {
            Send(new Dictionary<string, string>()
            {
                { KEY_ACTION,             ACTION_RUNTIME },
                { KEY_GUID,               GetGuid() },
                { KEY_ACTION_DETAIL_1,    startTime },
                { KEY_ACTION_DETAIL_2,    endTime },
                { KEY_ACTION_DETAIL_3,    totalTime },
            });
        }

        private static void Send(Dictionary<string, string> data)
        {
            GpmIndicator.Send(ManagerInfos.SERVICE_NAME, GpmManagerVersion.VERSION, data);
        }
    }
}
