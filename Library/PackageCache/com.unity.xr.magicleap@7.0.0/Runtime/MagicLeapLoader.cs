using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.InteractionSubsystems;
using UnityEngine.XR.MagicLeap.Rendering;
using UnityEngine.XR.Management;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.Management;
#endif //UNITY_EDITOR

#if UNITY_2020_1_OR_NEWER
using XRTextureLayout = UnityEngine.XR.XRDisplaySubsystem.TextureLayout;
#endif // UNITY_2020_1_OR_NEWER

namespace UnityEngine.XR.MagicLeap
{
#if UNITY_EDITOR && XR_MANAGEMENT_3_2_0_OR_NEWER
    [XRSupportedBuildTarget(BuildTargetGroup.Android)]
    [XRSupportedBuildTarget(BuildTargetGroup.Standalone, new BuildTarget[]{BuildTarget.StandaloneOSX, BuildTarget.StandaloneWindows, BuildTarget.StandaloneWindows64})]
#endif // UNITY_EDITOR && XR_MANAGEMENT_3_2_0_OR_NEWER
    /// <summary>
    /// Magic Leap XR Loader.
    /// Part of the XR Management system for loading the Magic Leap Provider
    /// </summary>
    public sealed class MagicLeapLoader : XRLoaderHelper
#if UNITY_EDITOR
        , IXRLoaderPreInit
#endif
    {
 #if UNITY_EDITOR
        public string GetPreInitLibraryName(BuildTarget buildTarget, BuildTargetGroup buildTargetGroup)
        {
            return "UnityMagicLeap";
        }
#endif

        enum Privileges : uint
        {
            ControllerPose = 263,
            GesturesConfig = 269,
            GesturesSubscribe = 268,
            LowLatencyLightwear = 59,
            WorldReconstruction = 33
        }
        const string kLogTag = "MagicLeapLoader";
        // Integrated Subsystems
        static List<XRDisplaySubsystemDescriptor> s_DisplaySubsystemDescriptors = new List<XRDisplaySubsystemDescriptor>();
        static List<XRInputSubsystemDescriptor> s_InputSubsystemDescriptors = new List<XRInputSubsystemDescriptor>();
        static List<XRMeshSubsystemDescriptor> s_MeshSubsystemDescriptor = new List<XRMeshSubsystemDescriptor>();
        static List<XRGestureSubsystemDescriptor> s_GestureSubsystemDescriptors = new List<XRGestureSubsystemDescriptor>();

        /// <summary>
        /// Display Subsystem property.
        /// Use this to determine the loaded Display Subsystem.
        /// </summary>
        public XRDisplaySubsystem displaySubsystem => GetLoadedSubsystem<XRDisplaySubsystem>();
        /// <summary>
        /// XR Input Subsystem property.
        /// Use this to determine the loaded XR Input Subsystem.
        /// </summary>
        public XRInputSubsystem inputSubsystem => GetLoadedSubsystem<XRInputSubsystem>();
        /// <summary>
        /// XR Meshing Subsystem property.
        /// Use this to determine the loaded XR Meshing Subsystem.
        /// </summary>
        public XRMeshSubsystem meshSubsystem => GetLoadedSubsystem<XRMeshSubsystem>();
        /// <summary>
        /// XR Gesture Subsystem property.
        /// Use this to determine the loaded XR Gesture Subsystem.
        /// </summary>
        public XRGestureSubsystem gestureSubsystem => GetLoadedSubsystem<XRGestureSubsystem>();

        // Expose subsystem lifecycle events and methods internally so that
        // the MagicLeap UnitySDK can drive subsystems that the XR Package doesn't have supported.
        // Subscribe to any of the events you need and then use the below functions to
        // trigger the lifecycle change of a subsystem.
        /* Example (using the anchor subsystem):
                    MagicLeapLoader.OnSubsystemsCreate += OnSubsystemsCreate;
                    private static void OnSubsystemsCreate(MagicLeapLoader loader) =>  loader.ForwardCreateSubsystem<XRAnchorSubsystemDescriptor, XRAnchorSubsystem>(new List<XRAnchorSubsystemDescriptor>(), MagicLeapSettings.Subsystems.GetSubsystemOverrideOrDefault<XRAnchorSubsystem>(MagicLeapConstants.kAnchorSubsystemId));
        */
        internal static event Action<MagicLeapLoader> OnSubsystemsCreate;
        internal static event Action<MagicLeapLoader> OnSubsystemsStart;
        internal static event Action<MagicLeapLoader> OnSubsystemsStop;
        internal static event Action<MagicLeapLoader> OnSubsystemsDestroy;

        // These calls used in conjunction with the above events to create/start/stop/destroy the subsystems
        // that are not supported by the XR Package alone. After subscribing to one of the above lifecycle events
        // these calls are used to actually trigger the lifecycle change.
        internal void ForwardCreateSubsystem<TDescriptor, TSubsystem>(List<TDescriptor> descriptors, string id)
    where TDescriptor : ISubsystemDescriptor
    where TSubsystem : ISubsystem
    => this.CreateSubsystem<TDescriptor, TSubsystem>(descriptors, id);

        internal void ForwardStartSubsystem<T>() where T : class, ISubsystem => this.StartSubsystem<T>();

        internal void ForwardStopSubsystem<T>() where T : class, ISubsystem => this.StopSubsystem<T>();

        internal void ForwardDestroySubsystem<T>() where T : class, ISubsystem => this.DestroySubsystem<T>();


        // ARSubsystems
        static List<XRSessionSubsystemDescriptor> s_SessionSubsystemDescriptors = new List<XRSessionSubsystemDescriptor>();
        static List<XRPlaneSubsystemDescriptor> s_PlaneSubsystemDescriptors = new List<XRPlaneSubsystemDescriptor>();
        static List<XRAnchorSubsystemDescriptor> s_AnchorSubsystemDescriptors = new List<XRAnchorSubsystemDescriptor>();
        static List<XRRaycastSubsystemDescriptor> s_RaycastSubsystemDescriptors = new List<XRRaycastSubsystemDescriptor>();

        /// <summary>
        /// XR Session Subsystem property.
        /// Use this to determine the loaded XR Session Subsystem.
        /// </summary>
        public XRSessionSubsystem sessionSubsystem => GetLoadedSubsystem<XRSessionSubsystem>();
        /// <summary>
        /// XR Plane Subsystem property.
        /// Use this to determine the loaded XR Plane Subsystem.
        /// </summary>
        public XRPlaneSubsystem planeSubsystem => GetLoadedSubsystem<XRPlaneSubsystem>();
        /// <summary>
        /// XR Raycast Subsystem property.
        /// Use this to determine the loaded XR Raycast Subsystem.
        /// </summary>
        public XRRaycastSubsystem raycastSubsystem => GetLoadedSubsystem<XRRaycastSubsystem>();

#if UNITY_EDITOR
        /// <summary>
        /// Location of the Magic Leap Loader asset.
        /// </summary>
        public static MagicLeapLoader assetInstance => (MagicLeapLoader)AssetDatabase.LoadAssetAtPath("Packages/com.unity.xr.magicleap/XR/Loaders/Magic Leap Loader.asset", typeof(MagicLeapLoader));
#endif // UNITY_EDITOR

        private bool m_DisplaySubsystemRunning = false;
        private int m_MeshSubsystemRefcount = 0;

        internal bool DisableValidationChecksOnEnteringPlaymode = false;

        /// <summary>
        /// Initialize all the Magic Leap subsystems.
        /// </summary>
        /// <returns>true if successfully created all subsystems.</returns>
        public override bool Initialize()
        {
#if UNITY_EDITOR
            if (!DisableValidationChecksOnEnteringPlaymode)
            {
                // This will only work when "Magic Leap App Simulator" is selected as the Editor's XR Plugin
                if (MagicLeapProjectValidation.LogPlaymodeValidationIssues())
                    return false;
            }
#endif
            Application.onBeforeRender -= CameraEnforcement.EnforceCameraProperties;
            Application.onBeforeRender += CameraEnforcement.EnforceCameraProperties;
            ApplySettings();

            // Display Subsystem depends on Input Subsystem, so initialize that first.
            CreateSubsystem<XRInputSubsystemDescriptor, XRInputSubsystem>(s_InputSubsystemDescriptors, MagicLeapSettings.Subsystems.GetSubsystemOverrideOrDefault<XRInputSubsystem>(MagicLeapConstants.kInputSubsystemId));
            CreateSubsystem<XRDisplaySubsystemDescriptor, XRDisplaySubsystem>(s_DisplaySubsystemDescriptors, MagicLeapSettings.Subsystems.GetSubsystemOverrideOrDefault<XRDisplaySubsystem>(MagicLeapConstants.kDisplaySubsytemId));
            CreateSubsystem<XRGestureSubsystemDescriptor, XRGestureSubsystem>(s_GestureSubsystemDescriptors, MagicLeapSettings.Subsystems.GetSubsystemOverrideOrDefault<XRGestureSubsystem>(MagicLeapConstants.kGestureSubsystemId));
            CreateSubsystem<XRMeshSubsystemDescriptor, XRMeshSubsystem>(s_MeshSubsystemDescriptor, MagicLeapSettings.Subsystems.GetSubsystemOverrideOrDefault<XRMeshSubsystem>(MagicLeapConstants.kMeshSubsystemId));

            // Now that subsystem creation is strictly handled by the loaders we must create the following subsystems
            // that live in ARSubsystems
            CreateSubsystem<XRSessionSubsystemDescriptor, XRSessionSubsystem>(s_SessionSubsystemDescriptors, MagicLeapSettings.Subsystems.GetSubsystemOverrideOrDefault<XRSessionSubsystem>(MagicLeapConstants.kSessionSubsystemId));
            CreateSubsystem<XRPlaneSubsystemDescriptor, XRPlaneSubsystem>(s_PlaneSubsystemDescriptors, MagicLeapSettings.Subsystems.GetSubsystemOverrideOrDefault<XRPlaneSubsystem>(MagicLeapConstants.kPlanesSubsystemId));
            CreateSubsystem<XRAnchorSubsystemDescriptor, XRAnchorSubsystem>(s_AnchorSubsystemDescriptors, MagicLeapSettings.Subsystems.GetSubsystemOverrideOrDefault<XRAnchorSubsystem>(MagicLeapConstants.kAnchorSubsystemId));
            CreateSubsystem<XRRaycastSubsystemDescriptor, XRRaycastSubsystem>(s_RaycastSubsystemDescriptors, MagicLeapSettings.Subsystems.GetSubsystemOverrideOrDefault<XRRaycastSubsystem>(MagicLeapConstants.kRaycastSubsystemId));

            OnSubsystemsCreate?.Invoke(this);

            return true;
        }

        /// <summary>
        /// Start all subsystems.
        /// </summary>
        /// <returns>true if all subsystems have successfully started.</returns>
        public override bool Start()
        {
            StartSubsystem<XRInputSubsystem>();
            StartSubsystem<XRGestureSubsystem>();

            if (!isLegacyDeviceActive)
            {
                var settings = MagicLeapSettings.currentSettings;
#if UNITY_2020_1_OR_NEWER
                if (settings != null && settings.forceMultipass)
                {
                    displaySubsystem.textureLayout = XRTextureLayout.SeparateTexture2Ds;
                    RenderingSettings.UnityMagicLeap_RenderingSetParameter("SinglePassEnabled", 0.0f);
                }
                else
                {
                    displaySubsystem.textureLayout = XRTextureLayout.Texture2DArray;
                    RenderingSettings.UnityMagicLeap_RenderingSetParameter("SinglePassEnabled", 1.0f);
                }
#else
                if (settings != null && settings.forceMultipass)
                    displaySubsystem.singlePassRenderingDisabled = true;
                else
                    displaySubsystem.singlePassRenderingDisabled = false;
#endif // UNITY_2020_1_OR_NEWER
                StartSubsystem<XRDisplaySubsystem>();
                m_DisplaySubsystemRunning = true;
            }


            OnSubsystemsStart?.Invoke(this);

            return true;
        }

        /// <summary>
        /// Stop all Magic Leap Subsystems.
        /// </summary>
        /// <returns>always returns true.</returns>
        public override bool Stop()
        {
            if (m_DisplaySubsystemRunning)
            {
                StopSubsystem<XRDisplaySubsystem>();
                m_DisplaySubsystemRunning = false;
            }
            if (m_MeshSubsystemRefcount > 0)
            {
                m_MeshSubsystemRefcount = 0;
                StopSubsystem<XRMeshSubsystem>();
            }
            StopSubsystem<XRPlaneSubsystem>();
            StopSubsystem<XRGestureSubsystem>();
            StopSubsystem<XRAnchorSubsystem>();
            StopSubsystem<XRRaycastSubsystem>();
            StopSubsystem<XRImageTrackingSubsystem>();
            StopSubsystem<XRInputSubsystem>();
            StopSubsystem<XRSessionSubsystem>();

            OnSubsystemsStop?.Invoke(this);
            return true;
        }

        /// <summary>
        /// Teardown (Deinitialize) all subsystems.
        /// </summary>
        /// <returns>Always returns true</returns>
        public override bool Deinitialize()
        {
            DestroySubsystem<XRDisplaySubsystem>();
            DestroySubsystem<XRMeshSubsystem>();
            DestroySubsystem<XRPlaneSubsystem>();
            DestroySubsystem<XRGestureSubsystem>();
            DestroySubsystem<XRImageTrackingSubsystem>();
            DestroySubsystem<XRRaycastSubsystem>();
            DestroySubsystem<XRAnchorSubsystem>();
            DestroySubsystem<XRInputSubsystem>();
            DestroySubsystem<XRSessionSubsystem>();

            OnSubsystemsDestroy?.Invoke(this);

            Application.onBeforeRender -= CameraEnforcement.EnforceCameraProperties;

            return true;
        }

        internal static bool isLegacyDeviceActive
        {
            get { return XRSettings.enabled && (XRSettings.loadedDeviceName == "Android"); }
        }

        internal void StartMeshSubsystem()
        {
            m_MeshSubsystemRefcount += 1;

            if (m_MeshSubsystemRefcount == 1)
            {
                StartSubsystem<XRMeshSubsystem>();
            }
        }

        internal void StopMeshSubsystem()
        {
            if (m_MeshSubsystemRefcount == 0)
                return;

            m_MeshSubsystemRefcount -= 1;

            if (m_MeshSubsystemRefcount == 0)
            {
                StopSubsystem<XRMeshSubsystem>();
            }
        }

        private void ApplySettings()
        {
            var settings = MagicLeapSettings.currentSettings;
            if (settings != null)
            {
                // set depth buffer precision
                Rendering.RenderingSettings.depthPrecision = settings.depthPrecision;
                Rendering.RenderingSettings.headlocked = settings.headlockGraphics;
            }
        }
    }
#if UNITY_EDITOR
    internal static class XRMangementEditorExtensions
    {
        internal static bool IsEnabledForPlatform(this XRLoader loader, BuildTargetGroup group)
        {
            var settings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(group);
            return settings?.Manager?.activeLoaders?.Contains(loader) ?? false;
        }

        internal static bool IsEnabledForPlatform(this XRLoader loader, BuildTarget target)
        {
            return loader.IsEnabledForPlatform(BuildPipeline.GetBuildTargetGroup(target));
        }
    }
#endif // UNITY_EDITOR
}
