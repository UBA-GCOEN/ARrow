using System;
#if UNITY_ANDROID
using System.Runtime.InteropServices;
#endif // UNITY_ANDROID

namespace UnityEngine.XR.MagicLeap.Internal
{
    internal struct PerceptionHandle : IDisposable
    {
        static class Native
        {
#if UNITY_ANDROID
            const string k_Library = "UnityMagicLeap";

            [DllImport(k_Library, EntryPoint = "UnityMagicLeap_ReleasePerceptionStack")]
            internal static extern void Release(IntPtr ptr);

            [DllImport(k_Library, EntryPoint = "UnityMagicLeap_RetainPerceptionStack")]
            internal static extern IntPtr Retain();
#else
            internal static void Release(IntPtr ptr) {}
            internal static IntPtr Retain() => IntPtr.Zero;
#endif // UNITY_ANDROID
        }

        IntPtr m_Handle;

        public bool active
        {
            get { return m_Handle != IntPtr.Zero; }
        }

        internal static PerceptionHandle Acquire()
        {
            return new PerceptionHandle
            {
                m_Handle = Native.Retain()
            };
        }

        public void Dispose()
        {
            if (!active)
                throw new ObjectDisposedException("Handle has already been disposed");

            Native.Release(m_Handle);
            m_Handle = IntPtr.Zero;
        }
    }
}
