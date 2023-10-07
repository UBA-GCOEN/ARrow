using System.Collections.Generic;
using System.Text;
using Gpm.Common.Util;
using UnityEngine;

namespace Gpm.Common.Indicator.Internal
{
    public static class IndicatorField
    {
        private const string KEY_REQUIRED_PROJECT_NAME = "projectName";
        private const string KEY_REQUIRED_PROJECT_VERSION = "projectVersion";
        private const string KEY_REQUIRED_LOG_VERSION = "logVersion";

        private const string KEY_CUSTOM_UNITY_VERSION = "unityVersion";
        private const string KEY_CUSTOM_PLATFORM = "platform";
        private const string KEY_CUSTOM_BRAND_NAME = "brandName";
        private const string KEY_CUSTOM_SERVICE_NAME = "serviceName";
        private const string KEY_CUSTOM_SERVICE_VERSION = "serviceVersion";
        private const string KEY_CUSTOM_BODY = "body";

        public static byte[] CreateSendData(
            string appKey, 
            string logVersion,
            string serviceName, 
            string serviceVersion,
            string body,
            Dictionary<string, string> customData)
        {
            var sendData = new Dictionary<string, string>();

            //----------------------------------------
            //  required field
            //----------------------------------------
            sendData.Add(KEY_REQUIRED_PROJECT_NAME, appKey);
            sendData.Add(KEY_REQUIRED_PROJECT_VERSION, GpmCommon.VERSION);
            sendData.Add(KEY_REQUIRED_LOG_VERSION, logVersion);

            //----------------------------------------
            //  custom field
            //----------------------------------------
            sendData.Add(KEY_CUSTOM_UNITY_VERSION, Application.unityVersion);
            sendData.Add(KEY_CUSTOM_PLATFORM, Application.platform.ToString());
            sendData.Add(KEY_CUSTOM_SERVICE_NAME, serviceName);
            sendData.Add(KEY_CUSTOM_SERVICE_VERSION, serviceVersion);
            sendData.Add(KEY_CUSTOM_BODY, body);
            
            if (customData == null)
            {
                return Encoding(sendData);
            }

            foreach (KeyValuePair<string, string> kvp in customData)
            {
                sendData.Add(kvp.Key, kvp.Value);
            }

            return Encoding(sendData);
        }

        public static string CreateJson(
            string appKey,
            string logVersion,
            string serviceName,
            string serviceVersion,
            string body,
            Dictionary<string, string> customData)
        {
            var sendData = new Dictionary<string, string>();

            //----------------------------------------
            //  required field
            //----------------------------------------
            sendData.Add(KEY_REQUIRED_PROJECT_NAME, appKey);
            sendData.Add(KEY_REQUIRED_PROJECT_VERSION, GpmCommon.VERSION);
            sendData.Add(KEY_REQUIRED_LOG_VERSION, logVersion);

            //----------------------------------------
            //  custom field
            //----------------------------------------
            sendData.Add(KEY_CUSTOM_UNITY_VERSION, Application.unityVersion);
            sendData.Add(KEY_CUSTOM_PLATFORM, Application.platform.ToString());
            sendData.Add(KEY_CUSTOM_SERVICE_NAME, serviceName);
            sendData.Add(KEY_CUSTOM_SERVICE_VERSION, serviceVersion);
            sendData.Add(KEY_CUSTOM_BODY, body);

            if (customData == null)
            {
                return GpmJsonMapper.ToJson(sendData);
            }

            foreach (KeyValuePair<string, string> kvp in customData)
            {
                sendData.Add(kvp.Key, kvp.Value);
            }

            return GpmJsonMapper.ToJson(sendData);
        }

        private static byte[] Encoding(Dictionary<string, string> dictionary)
        {
            var jsonString = GpmJsonMapper.ToJson(dictionary);
            return new UTF8Encoding().GetBytes(jsonString);
        }
    }
}