#if UNITY_EDITOR || UNITY_IOS
namespace Gpm.Communicator.Internal.Ios
{
    public sealed class IosMessageSender : INativeMessageSender
    {
        private static readonly IosMessageSender instance = new IosMessageSender();
        private IosMessageSenderExtern iosMessageSenderExtern = new IosMessageSenderExtern();

        public static IosMessageSender Instance
        {
            get { return instance; }
        }

        private IosMessageSender()
        {
            
        }

        public void Initialize(string gameObjectName, string methodName)
        {
            iosMessageSenderExtern.Initialize(gameObjectName, methodName);
        }

        public void InitializeClass(string className)
        {
            iosMessageSenderExtern.InitializeClass(className);
        }

        public string CallSync(string domain, string data, string extra)
        {
            return iosMessageSenderExtern.CallSync(domain, data, extra);
        }

        public void CallAsync(string domain, string data, string extra)
        {
            iosMessageSenderExtern.CallAsync(domain, data, extra);
        }
    }
}
#endif
