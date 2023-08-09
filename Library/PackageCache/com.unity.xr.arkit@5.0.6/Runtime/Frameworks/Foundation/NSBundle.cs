using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARKit
{
    struct NSBundle : INSObject, IDisposable, IEquatable<NSBundle>
    {
        IntPtr m_Self;
        public IntPtr AsIntPtr() => m_Self;
        public void Dispose() => NSObject.Dispose(ref m_Self);
        public override string ToString() => NSObject.ToString(this);
        public Class staticClass => GetClass();

        public static NSBundle mainBundle => GetMainBundle();

        public NSString bundleIdentifier => GetBundleIdentifier(this);

        public NSBundle(NSString bundleIdentifier, NativeView car) =>
            this = CreateFromCompiledAssetCatalog(bundleIdentifier, car);

        public NSString bundlePath => GetBundlePath(this);

        public bool Equals(NSBundle other) => NSObject.IsEqual(this, other);
        public override bool Equals(object obj) => obj is NSBundle other && Equals(other);
        public override int GetHashCode() => NSObject.GetHashCode(this);
        public static bool operator ==(NSBundle lhs, NSBundle rhs) => lhs.m_Self == rhs.m_Self;
        public static bool operator !=(NSBundle lhs, NSBundle rhs) => lhs.m_Self != rhs.m_Self;
        public static bool operator ==(NSBundle? lhs, NSBundle? rhs) => NSObject.ArePointersEqual(lhs, rhs);
        public static bool operator !=(NSBundle? lhs, NSBundle? rhs) => !(lhs == rhs);
        void INSObject.SetUnderlyingNativePtr(IntPtr ptr) => m_Self = ptr;

#if UNITY_EDITOR || !UNITY_XR_ARKIT_LOADER_ENABLED
        static NSBundle CreateFromCompiledAssetCatalog(NSString bundleIdentifier, NativeView car) => default;
        static NSBundle GetMainBundle() => default;
        static NSString GetBundlePath(NSBundle self) => default;
        static NSString GetBundleIdentifier(NSBundle self) => default;
        static Class GetClass() => default;
#else
        [DllImport("__Internal", EntryPoint = "UTempBundle_CreateFromCompiledAssetCatalog")]
        static extern NSBundle CreateFromCompiledAssetCatalog(NSString bundleIdentifier, NativeView car);

        [DllImport("__Internal", EntryPoint = "NSBundle_get_mainBundle")]
        static extern NSBundle GetMainBundle();

        [DllImport("__Internal", EntryPoint = "NSBundle_get_bundlePath")]
        static extern NSString GetBundlePath(NSBundle self);

        [DllImport("__Internal", EntryPoint = "NSBundle_get_bundleIdentifier")]
        static extern NSString GetBundleIdentifier(NSBundle self);

        [DllImport("__Internal", EntryPoint = "NSBundle_class")]
        static extern Class GetClass();
#endif
    }

    static class NSBundleExtensions
    {
        public static unsafe NSBundle GetNSBundle(this XRReferenceImageLibrary library)
        {
            // If no data exists in the XRReferenceImageLibrary, then try to look it up in the main bundle
            if (!library.dataStore.TryGetValue(ARKitPackageInfo.identifier, out var assetCatalogData))
            {
                var mainBundle = NSBundle.mainBundle;
                NSObject.Retain(mainBundle);
                return mainBundle;
            }

            fixed (void* data = assetCatalogData)
            {
                using var mainBundleIdentifier = NSBundle.mainBundle.bundleIdentifier;
                using var bundleIdentifier = new NSMutableString(mainBundleIdentifier);
                using var uuid = library.guid.ToNSString();

                bundleIdentifier.Append(uuid);

                return new NSBundle(bundleIdentifier, new NativeView
                {
                    data = data,
                    count = assetCatalogData.Length
                });
            }
        }
    }
}
