using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.ARKit
{
    struct NSEnumerator<T>
        : INSObject
        , IEquatable<NSEnumerator<T>>
        , IEnumerator<T>
        where T : struct, INSObject
    {
        IntPtr m_Self;
        public NSEnumerator(IntPtr ptr) => (m_Self, Current) = (ptr, default);
        public IntPtr AsIntPtr() => m_Self;
        public void Dispose() => NSObject.Dispose(ref m_Self);
        public override string ToString() => NSObject.ToString(this);
        public Class staticClass => NSEnumerator.GetClass();

        public bool Equals(NSEnumerator<T> other) => NSObject.IsEqual(this, other);
        public override bool Equals(object obj) => obj is NSEnumerator<T> other && Equals(other);
        public override int GetHashCode() => NSObject.GetHashCode(this);
        public static bool operator ==(NSEnumerator<T> lhs, NSEnumerator<T> rhs) => lhs.m_Self == rhs.m_Self;
        public static bool operator !=(NSEnumerator<T> lhs, NSEnumerator<T> rhs) => lhs.m_Self != rhs.m_Self;
        public static bool operator ==(NSEnumerator<T>? lhs, NSEnumerator<T>? rhs) => NSObject.ArePointersEqual(lhs, rhs);
        public static bool operator !=(NSEnumerator<T>? lhs, NSEnumerator<T>? rhs) => !(lhs == rhs);
        void INSObject.SetUnderlyingNativePtr(IntPtr ptr) => m_Self = ptr;

        public bool MoveNext()
        {
            var nextObject = NSEnumerator.GetNextObject(m_Self);
            Current.SetUnderlyingNativePtr(nextObject);
            return nextObject != IntPtr.Zero;
        }

        void IEnumerator.Reset() => throw new NotSupportedException();

        public T Current { get; }

        object IEnumerator.Current => Current;
    }

    static class NSEnumerator
    {
#if UNITY_EDITOR || !UNITY_XR_ARKIT_LOADER_ENABLED
        public static IntPtr GetNextObject(IntPtr self) => default;
        public static Class GetClass() => default;
#else
        [DllImport("__Internal", EntryPoint = "NSEnumerator_get_nextObject")]
        public static extern IntPtr GetNextObject(IntPtr self);

        [DllImport("__Internal", EntryPoint = "NSData_class")]
        public static extern Class GetClass();
#endif
    }
}
