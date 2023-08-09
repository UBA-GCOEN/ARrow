using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.XR.ARKit
{
    struct NSData : IDisposable, INSObject, IEquatable<NSData>
    {
        IntPtr m_Self;

        public NSData(IntPtr nsData) => m_Self = nsData;

        public static implicit operator IntPtr(NSData data) => data.m_Self;

        public IntPtr AsIntPtr() => m_Self;

        public Class staticClass => GetClass();

        public bool created => m_Self != IntPtr.Zero;

        public unsafe void* bytes => GetBytes(this);

        public ulong uLongLength => GetLength(this);

        public int length => (int)uLongLength;

        public unsafe NativeSlice<byte> ToNativeSlice() =>
            NativeSliceUnsafeUtility.ConvertExistingDataToNativeSlice<byte>(bytes, 1, length);

        public void Dispose() => NSObject.Dispose(ref m_Self);

        // IEquatable boilerplate
        public override int GetHashCode() => m_Self.GetHashCode();
        public override bool Equals(object obj) => obj is NSData data && Equals(data);
        public bool Equals(NSData other) => m_Self == other.m_Self;
        public static bool operator ==(NSData lhs, NSData rhs) => lhs.Equals(rhs);
        public static bool operator !=(NSData lhs, NSData rhs) => !lhs.Equals(rhs);
        public static bool operator ==(NSData? lhs, NSData? rhs) => NSObject.ArePointersEqual(lhs, rhs);
        public static bool operator !=(NSData? lhs, NSData? rhs) => !(lhs == rhs);
        public void SetUnderlyingNativePtr(IntPtr ptr) => m_Self = ptr;

#if UNITY_EDITOR || !UNITY_XR_ARKIT_LOADER_ENABLED
        static unsafe void* GetBytes(NSData self) => default;

        static ulong GetLength(NSData ptr) => default;

        public static unsafe NSData CreateWithBytes(void* bytes, int length) => default;

        public static unsafe NSData CreateWithBytesNoCopy(void* bytes, int length, bool freeWhenDone = false) => default;

        static Class GetClass() => default;
#else
        [DllImport("__Internal", EntryPoint = "NSData_get_bytes")]
        static extern unsafe void* GetBytes(NSData self);

        [DllImport("__Internal", EntryPoint = "NSData_get_length")]
        static extern ulong GetLength(NSData self);

        [DllImport("__Internal", EntryPoint = "NSData_createWithBytes_length_")]
        public static extern unsafe NSData CreateWithBytes(void* bytes, int length);

        [DllImport("__Internal", EntryPoint = "NSData_createWithBytesNoCopy_length_freeWhenDone_")]
        public static extern unsafe NSData CreateWithBytesNoCopy(void* bytes, int length, bool freeWhenDone = false);

        [DllImport("__Internal", EntryPoint = "NSData_class")]
        static extern Class GetClass();
#endif
    }

    static class NSDataExtensions
    {
        public static unsafe NSData AsNSData(this NativeSlice<byte> bytes) =>
            NSData.CreateWithBytesNoCopy(bytes.GetUnsafeReadOnlyPtr(), bytes.Length, false);

        public static unsafe NSData ToNSData(this NativeSlice<byte> bytes) =>
            NSData.CreateWithBytes(bytes.GetUnsafeReadOnlyPtr(), bytes.Length);
    }
}
