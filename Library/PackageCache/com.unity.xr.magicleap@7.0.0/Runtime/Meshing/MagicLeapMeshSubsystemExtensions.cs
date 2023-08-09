using System;
using System.Runtime.InteropServices;
using AOT;

using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.MagicLeap.Meshing;

namespace UnityEngine.XR.MagicLeap
{
    /// TODO (05/25/2020): Move shared MLSpatialMapper functionality to the mesh subsystem extensions
    /// TODO (05/25/2020): Move much of this logic to native provider.  This will simplify ARFoundation integration significantly
    /// 1. Move update timing to Native provider so ARFoundation doesn't query updates every frame as per ML instruction.
    static class MagicLeapMeshSubsystemExtensions
    {
        [MonoPInvokeCallback(typeof(Action<Feature>))]
        static void OnMeshSubsystemStart(Feature feature)
        {
            MagicLeapFeatures.SetFeatureRequested(Feature.Meshing | Feature.PointCloud, false);
            MagicLeapFeatures.SetFeatureRequested(feature, true);
        }

        [MonoPInvokeCallback(typeof(Action))]
        static void OnMeshSubsystemStop()
        {
            MagicLeapFeatures.SetFeatureRequested(Feature.Meshing | Feature.PointCloud, false);
        }

        static Action<Feature> s_OnMeshSubsystemStart = OnMeshSubsystemStart;
        static Action s_OnMeshSubsystemStop = OnMeshSubsystemStop;

        internal static bool RegisterNativeSubsystemCallbacks(this XRMeshSubsystem meshSubsystem)
            => NativeApi.RegisterMeshProviderFeatureCallbacks(s_OnMeshSubsystemStart, s_OnMeshSubsystemStop);


        public static void SetMeshingFeature(this XRMeshSubsystem meshSubsystem, Feature feature)
        {
            // TODO (05/25/2020): update this to maintain the flags for the explicit magic leap subsystem
            // TODO (05/25/2020): move get default meshing settings to a shared place (MeshingSettings?)
            var settings = MLSpatialMapper.GetDefaultMeshingSettings();

            if (feature.HasFlag(Feature.Meshing | Feature.PointCloud))
            {
                if (feature.HasFlag(Feature.Meshing) && feature.HasFlag(Feature.PointCloud))
                    throw new InvalidOperationException("Magic Leap does not support surfacing point cloud data while also surfacing meshing.");

                if (feature.HasFlag(Feature.Meshing))
                    settings.flags ^= MLMeshingFlags.PointCloud;

                MeshingSettings.meshingSettings = settings;
            }
            else
            {
                throw new ArgumentException($"Attempted to set invalid feature {feature} on Magic Leap Mesh Subsystem.");
            }
        }

        static class NativeApi
        {
            [DllImport("UnityMagicLeap", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UnityMagicLeap_Meshing_RegisterSubsystemLifecycleEventCallbackHandlers")]
            public static extern bool RegisterMeshProviderFeatureCallbacks(Action<Feature> onStartHandlerFuncPtr, Action onStopHandlerFuncPtr);
        }
    }
}
