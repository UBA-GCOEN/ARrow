using System;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.ARKit
{
    struct NSUUID : INSObject, IDisposable, IEquatable<NSUUID>
    {
        IntPtr m_Self;
        public IntPtr AsIntPtr() => m_Self;
        public void Dispose() => NSObject.Dispose(ref m_Self);
        public override string ToString() => NSObject.ToString(this);

        public NSUUID(Guid guid) => this = Init(guid);
        public static NSUUID New() => UUID();
        public NSString ToNSString() => UUIDString(this);

        public Class staticClass => GetClass();
        public bool Equals(NSUUID other) => NSObject.IsEqual(this, other);
        public override bool Equals(object obj) => obj is NSUUID other && Equals(other);
        public override int GetHashCode() => NSObject.GetHashCode(this);
        public static bool operator ==(NSUUID lhs, NSUUID rhs) => lhs.m_Self == rhs.m_Self;
        public static bool operator !=(NSUUID lhs, NSUUID rhs) => lhs.m_Self != rhs.m_Self;
        public static bool operator ==(NSUUID? lhs, NSUUID? rhs) => NSObject.ArePointersEqual(lhs, rhs);
        public static bool operator !=(NSUUID? lhs, NSUUID? rhs) => !(lhs == rhs);
        void INSObject.SetUnderlyingNativePtr(IntPtr ptr) => m_Self = ptr;

#if UNITY_EDITOR || !UNITY_XR_ARKIT_LOADER_ENABLED
        static NSUUID Init(Guid guid) => default;
        static NSUUID UUID() => default;
        static NSString UUIDString(NSUUID self) => default;
        static Class GetClass() => default;
#else
        [DllImport("__Internal", EntryPoint = "NSUUID_UUID")]
        static extern NSUUID UUID();

        [DllImport("__Internal", EntryPoint = "NSUUID_initWithUUIDBytes_")]
        static extern NSUUID Init(Guid guid);

        [DllImport("__Internal", EntryPoint = "NSUUID_UUIDString")]
        static extern NSString UUIDString(NSUUID self);

        [DllImport("__Internal", EntryPoint = "NSUUID_class")]
        static extern Class GetClass();
#endif
    }

    static class NSUUIDExtensions
    {
        public static NSString ToNSString(this Guid guid)
        {
            using (var uuid = new NSUUID(guid))
            {
                return uuid.ToNSString();
            }
        }
    }
}
