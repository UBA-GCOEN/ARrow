using System;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.MagicLeap
{
    internal static class RcoApi
    {
        [DllImport("UnityMagicLeap", EntryPoint="UnityMagicLeap_rco_retain")]
        public static extern int Retain(IntPtr ptr);

        [DllImport("UnityMagicLeap", EntryPoint="UnityMagicLeap_rco_release")]
        public static extern int Release(IntPtr ptr);

        [DllImport("UnityMagicLeap", EntryPoint="UnityMagicLeap_rco_retain_count")]
        public static extern int RetainCount(IntPtr ptr);
    }
}
