using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.ARKit
{
    struct NSSet<T>
        : INSObject
        , IDisposable
        , IEquatable<NSSet<T>>
        , IReadOnlyCollection<T>
        where T : struct, INSObject
    {
        IntPtr m_Self;
        public NSSet(IntPtr ptr) => m_Self = ptr;
        public IntPtr AsIntPtr() => m_Self;
        public void Dispose() => NSObject.Dispose(ref m_Self);
        public override string ToString() => NSObject.ToString(this);

        public ulong uLongCount => NSSet.GetCount(m_Self);

        public int Count => (int)uLongCount;

        public bool Contains(T obj) => NSSet.ContainsObject(m_Self, obj.AsIntPtr());

        public Class staticClass => NSSet.GetClass();
        public bool Equals(NSSet<T> other) => NSObject.IsEqual(this, other);
        public override bool Equals(object obj) => obj is NSSet<T> other && Equals(other);
        public override int GetHashCode() => NSObject.GetHashCode(this);
        public static bool operator ==(NSSet<T> lhs, NSSet<T> rhs) => lhs.m_Self == rhs.m_Self;
        public static bool operator !=(NSSet<T> lhs, NSSet<T> rhs) => lhs.m_Self != rhs.m_Self;
        public static bool operator ==(NSSet<T>? lhs, NSSet<T>? rhs) => NSObject.ArePointersEqual(lhs, rhs);
        public static bool operator !=(NSSet<T>? lhs, NSSet<T>? rhs) => !(lhs == rhs);
        void INSObject.SetUnderlyingNativePtr(IntPtr ptr) => m_Self = ptr;

        public NSEnumerator<T> GetEnumerator() => new NSEnumerator<T>(NSSet.GetEnumerator(m_Self));
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    static class NSSet
    {
#if UNITY_EDITOR || !UNITY_XR_ARKIT_LOADER_ENABLED
        public static ulong GetCount(IntPtr self) => default;
        public static IntPtr GetEnumerator(IntPtr self) => default;
        public static bool ContainsObject(IntPtr self, IntPtr obj) => default;
        public static Class GetClass() => default;
#else
        [DllImport("__Internal", EntryPoint = "NSSet_get_count")]
        public static extern ulong GetCount(IntPtr self);

        [DllImport("__Internal", EntryPoint = "NSSet_get_objectEnumerator")]
        public static extern IntPtr GetEnumerator(IntPtr self);

        [DllImport("__Internal", EntryPoint = "NSSet_containsObject_")]
        public static extern bool ContainsObject(IntPtr self, IntPtr obj);

        [DllImport("__Internal", EntryPoint = "NSSet_class")]
        public static extern Class GetClass();
#endif
    }
}
