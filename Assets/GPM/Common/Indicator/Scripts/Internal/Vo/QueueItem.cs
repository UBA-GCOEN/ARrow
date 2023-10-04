using System.Collections.Generic;

namespace Gpm.Common.Indicator.Internal
{
    public class QueueItem
    {
        public string serviceName;
        public string serviceVersion;
        public string body;
        public Dictionary<string, string> customData;
        public bool ignoreActivation;
        public bool isRunning;

        public QueueItem(string serviceName, string serviceVersion, string body, Dictionary<string, string> customData, bool ignoreActivation)
        {
            this.serviceName = serviceName;
            this.serviceVersion = serviceVersion;
            this.body = body;
            this.customData = customData;
            this.ignoreActivation = ignoreActivation;
        }
    }
}