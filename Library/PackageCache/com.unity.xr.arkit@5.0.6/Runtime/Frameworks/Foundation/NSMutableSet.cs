using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.ARKit
{
    struct NSMutableSet<T>
        : INSObject
        , IDisposable
        , IEquatable<NSMutableSet<T>>
        , IReadOnlyCollection<T>
        where T : struct, INSObject
    {
        IntPtr m_Self;

        public IntPtr AsIntPtr() => m_Self;

        public void Dispose() => NSObject.Dispose(ref m_Self);

        public override string ToString() => NSObject.ToString(this);

        public static implicit operator NSSet<T>(NSMutableSet<T> set) => new NSSet<T>(set.m_Self);

        public NSMutableSet(NSObject.Initialization initialization)
        {
            if (initialization != NSObject.Initialization.Default)
                throw new ArgumentException($"Initialization {initialization} is not supported.", nameof(initialization));

            m_Self = NSMutableSet.New();
        }

        public NSMutableSet(NSSet<T> other) => m_Self = NSMutableSet.SetWithSet(other.AsIntPtr());

        public void Add(T obj) => NSMutableSet.AddObject(m_Self, obj.AsIntPtr());

        public void Add(NSSet<T> set) => NSMutableSet.UnionSet(m_Self, set.AsIntPtr());

        public void Remove(T obj) => NSMutableSet.RemoveObject(m_Self, obj.AsIntPtr());

        public void Clear() => NSMutableSet.RemoveAllObjects(m_Self);

        public ulong uLongCount => NSSet.GetCount(m_Self);

        public int Count => (int)uLongCount;

        public bool Contains(T obj) => NSSet.ContainsObject(m_Self, obj.AsIntPtr());

        public Class staticClass => NSMutableSet.GetClass();
        public bool Equals(NSMutableSet<T> other) => NSObject.IsEqual(this, other);
        public override bool Equals(object obj) => obj is NSMutableSet<T> other && Equals(other);
        public override int GetHashCode() => NSObject.GetHashCode(this);
        public static bool operator ==(NSMutableSet<T> lhs, NSMutableSet<T> rhs) => lhs.m_Self == rhs.m_Self;
        public static bool operator !=(NSMutableSet<T> lhs, NSMutableSet<T> rhs) => lhs.m_Self != rhs.m_Self;
        public static bool operator ==(NSMutableSet<T>? lhs, NSMutableSet<T>? rhs) => NSObject.ArePointersEqual(lhs, rhs);
        public static bool operator !=(NSMutableSet<T>? lhs, NSMutableSet<T>? rhs) => !(lhs == rhs);
        void INSObject.SetUnderlyingNativePtr(IntPtr ptr) => m_Self = ptr;

        public NSEnumerator<T> GetEnumerator() => new NSEnumerator<T>(NSSet.GetEnumerator(m_Self));
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    static class NSMutableSet
    {
#if UNITY_EDITOR || !UNITY_XR_ARKIT_LOADER_ENABLED
        public static IntPtr SetWithSet(IntPtr other) => default;
        public static IntPtr New() => default;
        public static void AddObject(IntPtr self, IntPtr obj) { }
        public static void RemoveObject(IntPtr self, IntPtr obj) { }
        public static void RemoveAllObjects(IntPtr self) { }
        public static void UnionSet(IntPtr self, IntPtr set) { }
        public static Class GetClass() => default;
#else
        [DllImport("__Internal", EntryPoint = "NSMutableSet_new")]
        public static extern IntPtr New();

        [DllImport("__Internal", EntryPoint = "NSMutableSet_setWithSet_")]
        public static extern IntPtr SetWithSet(IntPtr other);

        [DllImport("__Internal", EntryPoint = "NSMutableSet_addObject_")]
        public static extern void AddObject(IntPtr self, IntPtr obj);

        [DllImport("__Internal", EntryPoint = "NSMutableSet_removeObject_")]
        public static extern void RemoveObject(IntPtr self, IntPtr obj);

        [DllImport("__Internal", EntryPoint = "NSMutableSet_removeAllObjects")]
        public static extern void RemoveAllObjects(IntPtr self);

        [DllImport("__Internal", EntryPoint = "NSMutableSet_unionSet_")]
        public static extern void UnionSet(IntPtr self, IntPtr set);

        [DllImport("__Internal", EntryPoint = "NSMutableSet_class")]
        public static extern Class GetClass();
#endif
    }
}
