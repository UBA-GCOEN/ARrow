namespace Gpm.Communicator.Internal
{
    public interface INativeMessageSender
    {
        void Initialize(string gameObjectName, string methodName);
        void InitializeClass(string className);
        string CallSync(string domain, string data, string extra);
        void CallAsync(string domain, string data, string extra);
    }
}