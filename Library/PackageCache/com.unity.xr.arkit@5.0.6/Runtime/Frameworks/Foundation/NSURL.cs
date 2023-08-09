using System;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.ARKit
{
    struct NSURL : INSObject, IDisposable, IEquatable<NSURL>
    {
        IntPtr m_Self;

        public static NSURL FromFilePath(NSString path) => FileURLWithPath(path);

        public static NSURL FromFilePath(string path)
        {
            using var nsstring = new NSString(path);
            return FileURLWithPath(nsstring);
        }

        public IntPtr AsIntPtr() => m_Self;
        public void Dispose() => NSObject.Dispose(ref m_Self);
        public override string ToString() => NSObject.ToString(this);

        public Class staticClass => GetClass();
        public bool Equals(NSURL other) => NSObject.IsEqual(this, other);
        public override bool Equals(object obj) => obj is NSURL other && Equals(other);
        public override int GetHashCode() => NSObject.GetHashCode(this);
        public static bool operator ==(NSURL lhs, NSURL rhs) => lhs.m_Self == rhs.m_Self;
        public static bool operator !=(NSURL lhs, NSURL rhs) => lhs.m_Self != rhs.m_Self;
        public static bool operator ==(NSURL? lhs, NSURL? rhs) => NSObject.ArePointersEqual(lhs, rhs);
        public static bool operator !=(NSURL? lhs, NSURL? rhs) => !(lhs == rhs);
        void INSObject.SetUnderlyingNativePtr(IntPtr ptr) => m_Self = ptr;

#if UNITY_EDITOR || !UNITY_XR_ARKIT_LOADER_ENABLED
        static NSURL URLWithString(NSString str) => default;
        static Class GetClass() => default;
        static NSURL FileURLWithPath(NSString path) => default;
#else
        [DllImport("__Internal", EntryPoint = "NSURL_fileURLWithPath_")]
        static extern NSURL FileURLWithPath(NSString path);

        [DllImport("__Internal", EntryPoint = "NSURL_URLWithString_")]
        static extern NSURL URLWithString(NSString str);

        [DllImport("__Internal", EntryPoint = "NSURL_class")]
        static extern Class GetClass();
#endif
    }
}
