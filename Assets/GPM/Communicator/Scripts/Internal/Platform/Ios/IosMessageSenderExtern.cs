#if UNITY_EDITOR || UNITY_IOS
namespace Gpm.Communicator.Internal.Ios
{
    using System;
    using System.Runtime.InteropServices;

    public class IosMessageSenderExtern
    {
        [DllImport("__Internal")]
        private static extern void initializeUnityObject(string gameObjectName, string methodName);
        [DllImport("__Internal")]
        private static extern void initializeClass(string className);
        [DllImport("__Internal")]
        private static extern IntPtr onRequestSync(string domain, string data, string extra);
        [DllImport("__Internal")]
        private static extern void onRequestAsync(string domain, string data, string extra);

        public void Initialize(string gameObjectName, string methodName)
        {
            initializeUnityObject(gameObjectName, methodName);
        }

        public void InitializeClass(string className)
        {
            initializeClass(className);
        }

        public string CallSync(string domain, string data, string extra)
        {
            string retValue = string.Empty;
            IntPtr result = onRequestSync(domain, data, extra);
            if (IntPtr.Zero != result)
            {
                retValue = Marshal.PtrToStringAnsi(result);
            }

            return retValue;
        }

        public void CallAsync(string domain, string data, string extra)
        {
            onRequestAsync(domain, data, extra);
        }
    }
}
#endif
