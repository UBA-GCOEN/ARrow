namespace Gpm.Communicator
{    
    using Gpm.Communicator.Internal;

    public class GpmCommunicator
    {
        public const string VERSION = "1.1.0";

        public static void InitializeClass(GpmCommunicatorVO.Configuration configuration)
        {
            CommunicatorImplementation.Instance.InitializeClass(configuration);
        }

        public static void AddReceiver(string domain, GpmCommunicatorCallback.CommunicatorCallback callback)
        {            
            CommunicatorImplementation.Instance.AddReceiver(domain, callback);
        }

        public static GpmCommunicatorVO.Message CallSync(GpmCommunicatorVO.Message message)
        {
            return CommunicatorImplementation.Instance.CallSync(message);
        }

        public static void CallAsync(GpmCommunicatorVO.Message message)
        {
            CommunicatorImplementation.Instance.CallAsync(message);
        }
    }
}
