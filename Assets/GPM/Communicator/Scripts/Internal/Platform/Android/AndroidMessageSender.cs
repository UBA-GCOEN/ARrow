#if UNITY_EDITOR || UNITY_ANDROID
namespace Gpm.Communicator.Internal.Android
{
    using UnityEngine;

    public sealed class AndroidMessageSender : INativeMessageSender
    {
        private static readonly AndroidMessageSender instance = new AndroidMessageSender();
        private const string GAMEBASE_ANDROID_PLUGIN_CLASS = "com.gpm.communicator.internal.MessageReceiver";
        private AndroidJavaClass jc = null;

        public static AndroidMessageSender Instance
        {
            get { return instance; }
        }

        private AndroidMessageSender()
        {
            if (jc == null)
            {
                jc = new AndroidJavaClass(GAMEBASE_ANDROID_PLUGIN_CLASS);
            }            
        }
        
        public void Initialize(string gameObjectName, string methodName)
        {
            jc.CallStatic("initializeUnityObject", gameObjectName, methodName);
        }

        public void InitializeClass(string className)
        {
            jc.CallStatic("initializeClass", className);
        }

        public string CallSync(string domain, string data, string extra)
        {
            string retValue = jc.CallStatic<string>("onRequestSync", domain, data, extra);

            return retValue;
        }

        public void CallAsync(string domain, string data, string extra)
        {
            jc.CallStatic("onRequestAsync", domain, data, extra);
        }
    }
}
#endif
