using UnityEngine.Networking;

namespace Gpm.Common.Indicator.Internal
{
    public class LogNCrashRequest
    {
        protected LaunchingInfo.Launching.Indicator.Zone indicatorInfo;

        public LogNCrashRequest(LaunchingInfo.Launching.Indicator.Zone indicatorInfo)
        {
            this.indicatorInfo = indicatorInfo;
        }

        public UnityWebRequest Request(QueueItem queueItem)
        {
            string json =
                        IndicatorField.CreateJson(
                            indicatorInfo.appKey,
                            indicatorInfo.logVersion,
                            queueItem.serviceName,
                            queueItem.serviceVersion,
                            queueItem.body,
                            queueItem.customData);

            return Request(json);
        }

        public UnityWebRequest Request(string json)
        {
            string url = string.Format("{0}/{1}/log", indicatorInfo.url, indicatorInfo.logVersion);

            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);

            UploadHandler uploadHandler = new UploadHandlerRaw(jsonToSend);
            DownloadHandler downloadHandler = new DownloadHandlerBuffer();

            UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST, downloadHandler, uploadHandler);
            request.SetRequestHeader("Content-Type", "application/json");

            return request;
        }
    }
}