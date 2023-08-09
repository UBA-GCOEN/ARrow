using System;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.ARKit
{
    struct NSMutableData : IDisposable, INSObject, IEquatable<NSMutableData>
    {
        IntPtr m_Self;

        NSData AsNSData() => new NSData(m_Self);

        public static implicit operator IntPtr(NSMutableData data) => data.m_Self;

        public IntPtr AsIntPtr() => m_Self;

        public unsafe NSMutableData(void* bytes, int length) => m_Self = CreateWithBytes(bytes, length);

        public Class staticClass => GetClass();

        public bool created => m_Self != IntPtr.Zero;

        public unsafe void* bytes => AsNSData().bytes;

        public int length => AsNSData().length;

        public unsafe void AppendBytes(void* bytes, int length)
        {
            if (!created)
                throw new InvalidOperationException("The NSMutableArray has not been created.");

            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            AppendBytes(this, bytes, length);
        }

        public static implicit operator NSData(NSMutableData data) => data.AsNSData();

        public void Dispose() => NSObject.Dispose(ref m_Self);

        public override int GetHashCode() => m_Self.GetHashCode();
        public override bool Equals(object obj) => obj is NSMutableData data && Equals(data);
        public bool Equals(NSMutableData other) => m_Self == other.m_Self;
        public static bool operator ==(NSMutableData lhs, NSMutableData rhs) => lhs.Equals(rhs);
        public static bool operator !=(NSMutableData lhs, NSMutableData rhs) => !lhs.Equals(rhs);
        public static bool operator ==(NSMutableData? lhs, NSMutableData? rhs) => NSObject.ArePointersEqual(lhs, rhs);
        public static bool operator !=(NSMutableData? lhs, NSMutableData? rhs) => !(lhs == rhs);
        public void SetUnderlyingNativePtr(IntPtr ptr) => m_Self = ptr;

#if UNITY_XR_ARKIT_LOADER_ENABLED
        [DllImport("__Internal", EntryPoint = "NSMutableData_appendBytes_length_")]
        static extern unsafe void AppendBytes(NSMutableData self, void* bytes, int length);

        [DllImport("__Internal", EntryPoint = "NSMutableData_createWithBytes_length_")]
        static extern unsafe IntPtr CreateWithBytes(void* bytes, int length);

        [DllImport("__Internal", EntryPoint = "NSMutableData_class")]
        static extern Class GetClass();
#else
        static readonly string k_ExceptionMsg = "Apple ARKit XR Plug-in Provider not enabled in project settings.";

        static unsafe void AppendBytes(NSMutableData self, void* bytes, int length) =>
            throw new System.NotImplementedException(k_ExceptionMsg);

        static unsafe IntPtr CreateWithBytes(void* bytes, int length) =>
            throw new System.NotImplementedException(k_ExceptionMsg);

        static Class GetClass() => default;
#endif
    }
}
