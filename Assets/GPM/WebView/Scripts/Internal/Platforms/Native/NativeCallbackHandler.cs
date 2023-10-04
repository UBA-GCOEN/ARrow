using System.Collections.Generic;

namespace Gpm.WebView.Internal
{
    public static class NativeCallbackHandler
    {
        private static int handle = 0;
        private static Dictionary<int, object> callbackDic = new Dictionary<int, object>();

        public static int RegisterCallback(object callback)
        {
            if (callback == null)
            {
                return -1;
            }   

            callbackDic.Add(handle, callback);
            return handle++;
        }

        public static T GetCallback<T>(int handle) where T : class
        {
            if (callbackDic.ContainsKey(handle) == true)
            {
                return (T)callbackDic[handle];
            }

            return null;
        }

        public static void UnregisterCallback(int handle)
        {
            if (callbackDic.ContainsKey(handle) == true)
            {
                callbackDic.Remove(handle);
            }
        }
    }
}