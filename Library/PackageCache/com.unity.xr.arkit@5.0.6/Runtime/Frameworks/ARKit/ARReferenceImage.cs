using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARKit
{
    struct ARReferenceImage : INSObject, IDisposable, IEquatable<ARReferenceImage>
    {
        IntPtr m_Self;
        public IntPtr AsIntPtr() => m_Self;
        public void Dispose() => NSObject.Dispose(ref m_Self);
        public override string ToString() => NSObject.ToString(this);
        public Class staticClass => GetClass();

        public static NSSet<ARReferenceImage> GetReferenceImagesInGroupNamed(string groupName, NSBundle bundle)
        {
            using (var name = new NSString(groupName))
            {
                return GetReferenceImagesInGroupNamed(name, bundle);
            }
        }

        public static NSSet<ARReferenceImage> GetReferenceImagesInGroupNamed(NSString groupName, NSBundle bundle) =>
            new NSSet<ARReferenceImage>(ReferenceImagesInGroupNamed(groupName, bundle));

        public bool Equals(ARReferenceImage other) => NSObject.IsEqual(this, other);
        public override bool Equals(object obj) => obj is ARReferenceImage other && Equals(other);
        public override int GetHashCode() => NSObject.GetHashCode(this);
        public static bool operator ==(ARReferenceImage lhs, ARReferenceImage rhs) => lhs.m_Self == rhs.m_Self;
        public static bool operator !=(ARReferenceImage lhs, ARReferenceImage rhs) => lhs.m_Self != rhs.m_Self;
        public static bool operator ==(ARReferenceImage? lhs, ARReferenceImage? rhs) => NSObject.ArePointersEqual(lhs, rhs);
        public static bool operator !=(ARReferenceImage? lhs, ARReferenceImage? rhs) => !(lhs == rhs);
        void INSObject.SetUnderlyingNativePtr(IntPtr ptr) => m_Self = ptr;

#if UNITY_EDITOR || !UNITY_XR_ARKIT_LOADER_ENABLED
        static IntPtr ReferenceImagesInGroupNamed(NSString groupName, NSBundle bundle) => default;
        static Class GetClass() => default;
#else
        [DllImport("__Internal", EntryPoint = "ARReferenceImage_referenceImagesInGroupNamed_bundle_")]
        static extern IntPtr ReferenceImagesInGroupNamed(NSString groupName, NSBundle bundle);

        [DllImport("__Internal", EntryPoint = "ARReferenceImage_class")]
        static extern Class GetClass();
#endif
    }

    static class ARReferenceImageExtensions
    {
        public static NSString GetARResourceGroupName(this XRReferenceImageLibrary library)
        {
            if (library.dataStore.TryGetValue(ARKitPackageInfo.identifier, out var data))
                return new NSString(ARKitImageDatabase.resourceGroupName);

            // Otherwise, construct the name based on the library's name + guid
            var name = new NSMutableString(library.name);
            name.Append(NSString.underscore);
            using (var uuid = library.guid.ToNSString())
            {
                name.Append(uuid);
            }

            return name;
        }
    }
}
