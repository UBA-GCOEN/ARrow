using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.ARKit
{
    struct NSMutableString : INSObject, IDisposable, IEnumerable, IEquatable<NSMutableString>
    {
        IntPtr m_Self;
        public IntPtr AsIntPtr() => m_Self;
        public void Dispose() => NSObject.Dispose(ref m_Self);
        NSString AsNSString() => new NSString(m_Self);
        public NSMutableString(string str) => this = InitWithBytes(str, str?.Length ?? 0, NSStringEncoding.NSUTF16LittleEndianStringEncoding);
        public NSMutableString(NSString str) => this = InitWithString(str);

        public NSMutableString(NSObject.Initialization initialization)
        {
            if (initialization != NSObject.Initialization.Default)
                throw new ArgumentException($"Initialization {initialization} is not supported.", nameof(initialization));

            this = Init();
        }

        public string GetStringAndDispose()
        {
            using (this)
            {
                return ToString();
            }
        }

        public override string ToString() => AsNSString().ToString();

        public void Add(string str)
        {
            using var nsstring = new NSString(str);
            Append(nsstring);
        }

        public void Add(NSString str) => Append(str);

        // Required for collection initialization
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

        public ulong ulongLength => AsNSString().ulongLength;

        public int length => (int)ulongLength;

        public void Append(NSString str) => AppendString(this, str);

        public static implicit operator NSString(NSMutableString str) => str.AsNSString();

        public Class staticClass => GetClass();
        public bool Equals(NSMutableString other) => NSObject.IsEqual(this, other);
        public override bool Equals(object obj) => obj is NSMutableString other && Equals(other);
        public override int GetHashCode() => NSObject.GetHashCode(this);
        public static bool operator ==(NSMutableString lhs, NSMutableString rhs) => lhs.m_Self == rhs.m_Self;
        public static bool operator !=(NSMutableString lhs, NSMutableString rhs) => lhs.m_Self != rhs.m_Self;
        public static bool operator ==(NSMutableString? lhs, NSMutableString? rhs) => NSObject.ArePointersEqual(lhs, rhs);
        public static bool operator !=(NSMutableString? lhs, NSMutableString? rhs) => !(lhs == rhs);
        void INSObject.SetUnderlyingNativePtr(IntPtr ptr) => m_Self = ptr;

#if UNITY_EDITOR || !UNITY_XR_ARKIT_LOADER_ENABLED
        static void AppendString(NSMutableString self, NSString stringToAppend) { }
        static NSMutableString InitWithBytes(string str, int length, NSStringEncoding encoding) => default;
        static NSMutableString InitWithString(NSString str) => default;
        static NSMutableString Init() => default;
        static Class GetClass() => default;
#else
        [DllImport("__Internal", EntryPoint = "NSMutableString_appendString_")]
        static extern void AppendString(NSMutableString self, NSString stringToAppend);

        [DllImport("__Internal", EntryPoint = "NSMutableString_initWithBytes_length_encoding_")]
        static extern NSMutableString InitWithBytes([MarshalAs(UnmanagedType.LPWStr)] string str, int length, NSStringEncoding encoding);

        [DllImport("__Internal", EntryPoint = "NSMutableString_initWithString_")]
        static extern NSMutableString InitWithString(NSString str);

        [DllImport("__Internal", EntryPoint = "NSMutableString_init")]
        static extern NSMutableString Init();

        [DllImport("__Internal", EntryPoint = "NSMutableString_class")]
        static extern Class GetClass();
#endif
    }
}
