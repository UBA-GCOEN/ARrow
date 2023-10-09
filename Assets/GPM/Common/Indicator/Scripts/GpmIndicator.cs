using Gpm.Common.Indicator.Internal;
using System.Collections.Generic;

namespace Gpm.Common.Indicator
{
    public sealed class GpmIndicator
    {
        public const string SERVICE_NAME = "Indicator";

        public static void Send(string serviceName, string serviceVersion, Dictionary<string, string> customData, bool ignoreActivation = false)
        {
            IndicatorImplementation.Instance.Send(serviceName, serviceVersion, customData, ignoreActivation);
        }
    }
}