using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.MagicLeap
{
    static class MagicLeapFeatures
    {
        const Feature kUniversallySupportedFeatures = Feature.AnyTrackingMode
            | Feature.WorldFacingCamera
            | Feature.PlaneTracking
            | Feature.ImageTracking;

        // Features rank in order of power usage typically
        static readonly ConfigurationDescriptor[] s_ConfigurationsDescriptors = {
            new ConfigurationDescriptor(IntPtr.Zero, kUniversallySupportedFeatures | Feature.Meshing, 0),
            new ConfigurationDescriptor((IntPtr)1,kUniversallySupportedFeatures | Feature.PointCloud, 1)
        };

        public static Feature requestedFeatures { get; private set; } = Feature.None;

        public static void SetFeatureRequested(Feature feature, bool value)
        {
            requestedFeatures = requestedFeatures.SetEnabled(feature, value);
        }

        public static Feature currentFeatures { get; set; } = Feature.None;

        public static void SetCurrentFeatureEnabled(Feature feature, bool value)
        {
            currentFeatures = currentFeatures.SetEnabled(feature, value);
        }

        public static unsafe NativeArray<ConfigurationDescriptor> AcquireConfigurationDescriptors(Allocator allocator)
            => new NativeArray<ConfigurationDescriptor>(s_ConfigurationsDescriptors, allocator);
    }
}
