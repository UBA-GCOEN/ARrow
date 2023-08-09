using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARKit
{
    struct ARReferenceObject : INSObject, IDisposable, IEquatable<ARReferenceObject>
    {
        IntPtr m_Self;

        public Class staticClass => GetClass();

        public IntPtr AsIntPtr() => m_Self;

        public void Dispose() => NSObject.Dispose(ref m_Self);

        public override string ToString() => NSObject.ToString(this);

        public ARReferenceObject(NativeSlice<byte> bytes)
        {
            unsafe
            {
                this = InitWithBytes(bytes.GetUnsafeReadOnlyPtr(), bytes.Length);
            }
        }

        public ARReferenceObject(byte[] bytes)
        {
            if (bytes == null)
            {
                this = default;
            }
            else
            {
                unsafe
                {
                    fixed (void* ptr = bytes)
                    {
                        this = InitWithBytes(ptr, bytes.Length);
                    }
                }
            }
        }

        public static ARReferenceObject FromURL(NSURL url, out NSError error) => InitWithArchiveURL(url, out error);

        public NSString name
        {
            get => GetName(this);
            set => SetName(this, value);
        }

        public void SetName(XRReferenceObject referenceObject)
        {
            using var uuid = referenceObject.guid.ToNSString();
            using var name = new NSMutableString(NSObject.Initialization.Default)
            {
                referenceObject.name,
                NSString.underscore,
                uuid
            };

            this.name = name;
        }

        public static NSSet<ARReferenceObject> GetReferenceObjectsInGroupNamed(NSString name, NSBundle bundle) =>
            new NSSet<ARReferenceObject>(ReferenceObjectsInGroupNamed(name, bundle));

        public bool Equals(ARReferenceObject other) => NSObject.IsEqual(this, other);

        public override bool Equals(object obj) => obj is ARReferenceObject other && Equals(other);

        public override int GetHashCode() => NSObject.GetHashCode(this);

        public static bool operator ==(ARReferenceObject lhs, ARReferenceObject rhs) => lhs.m_Self == rhs.m_Self;

        public static bool operator ==(ARReferenceObject? lhs, ARReferenceObject? rhs) => NSObject.ArePointersEqual(lhs, rhs);

        public static bool operator !=(ARReferenceObject? lhs, ARReferenceObject? rhs) => !(lhs == rhs);

        public static bool operator !=(ARReferenceObject lhs, ARReferenceObject rhs) => lhs.m_Self != rhs.m_Self;

        void INSObject.SetUnderlyingNativePtr(IntPtr ptr) => m_Self = ptr;

#if UNITY_EDITOR || !UNITY_XR_ARKIT_LOADER_ENABLED
        static NSString GetName(ARReferenceObject self) => default;

        static void SetName(ARReferenceObject self, NSString name) { }

        static IntPtr ReferenceObjectsInGroupNamed(NSString name, NSBundle bundle) => default;

        static ARReferenceObject InitWithArchiveURL(NSURL url, out NSError error)
        {
            error = default;
            return default;
        }

        static Class GetClass() => default;

        static unsafe ARReferenceObject InitWithBytes(void* bytes, int byteCount) => default;
#else
        [DllImport("__Internal", EntryPoint = "ARReferenceObject_get_name")]
        static extern NSString GetName(ARReferenceObject self);

        [DllImport("__Internal", EntryPoint = "ARReferenceObject_set_name")]
        static extern void SetName(ARReferenceObject self, NSString name);

        [DllImport("__Internal", EntryPoint = "ARReferenceObject_referenceObjectsInGroupNamed_bundle_")]
        static extern IntPtr ReferenceObjectsInGroupNamed(NSString name, NSBundle bundle);

        [DllImport("__Internal", EntryPoint = "ARReferenceObject_initWithArchiveURL_error_")]
        static extern ARReferenceObject InitWithArchiveURL(NSURL url, out NSError error);

        [DllImport("__Internal", EntryPoint = "ARReferenceObject_class")]
        static extern Class GetClass();

        [DllImport("__Internal", EntryPoint = "ARReferenceObject_initWithBytes_length_")]
        static extern unsafe ARReferenceObject InitWithBytes(void* bytes, int byteCount);
#endif
    }

    static class ARReferenceObjectExtensions
    {
        public static NSString GetARResourceGroupName(this XRReferenceObjectLibrary library)
        {
            // Creates $"{library.name}_{library.guid}
            using var uuid = library.guid.ToNSString();
            return new NSMutableString(NSObject.Initialization.Default)
            {
                library.name,
                NSString.underscore,
                uuid
            };
        }
    }
}
