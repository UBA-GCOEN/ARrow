namespace Gpm.Communicator.Internal
{
    public class CommunicatorImplementation
    {
        private static readonly CommunicatorImplementation instance = new CommunicatorImplementation();
                
        private Communicator communicator;

        public static CommunicatorImplementation Instance
        {
            get { return instance; }
        }

        private CommunicatorImplementation()
        {
            communicator = ComponentManager.AddComponent<Communicator>(GameObjectManager.GameObjectType.CORE_TYPE);
            communicator.Initialize();
        }

        public void InitializeClass(GpmCommunicatorVO.Configuration configuration)
        {
            communicator.InitializeClass(configuration);
        }

        public void AddReceiver(string domain, GpmCommunicatorCallback.CommunicatorCallback callback)
        {
            communicator.AddReceiver(domain, callback);
        }

        public GpmCommunicatorVO.Message CallSync(GpmCommunicatorVO.Message message)
        {
            return communicator.CallSync(message);
        }

        public void CallAsync(GpmCommunicatorVO.Message message)
        {
            communicator.CallAsync(message);
        }
    }
}
