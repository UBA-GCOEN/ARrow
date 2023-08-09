using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine.Rendering;
#if MODULE_URP_ENABLED
using UnityEngine.Rendering.Universal;
#endif // MODULE_URP_ENABLED
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARKit
{
    /// <summary>
    /// The camera subsystem implementation for ARKit.
    /// </summary>
    [Preserve]
    public sealed class ARKitCameraSubsystem : XRCameraSubsystem
    {
        /// <summary>
        /// The identifying name for the camera-providing implementation.
        /// </summary>
        /// <value>The identifying name for the camera-providing implementation.</value>
        const string k_SubsystemId = "ARKit-Camera";

        /// <summary>
        /// The name of the shader for rendering the camera texture before opaques render.
        /// </summary>
        /// <value>The name of the shader for rendering the camera texture.</value>
        const string k_BeforeOpaquesBackgroundShaderName = "Unlit/ARKitBackground";

        /// <summary>
        /// The name of the shader for rendering the camera texture after opaques have rendered.
        /// </summary>
        /// <value>The name of the shader for rendering the camera texture.</value>
        const string k_AfterOpaquesBackgroundShaderName = "Unlit/ARKitBackground/AfterOpaques";

        /// <summary>
        /// The shader keyword for enabling URP rendering.
        /// </summary>
        /// <value>The shader keyword for enabling URP rendering.</value>
        const string k_BackgroundShaderKeywordURP = "ARKIT_BACKGROUND_URP";

        /// <summary>
        /// The list of shader keywords to avoid during compilation.
        /// </summary>
        /// <value>The list of shader keywords to avoid during compilation.</value>
        static readonly List<string> k_BackgroundShaderKeywordsToNotCompile = new()
        {
#if !MODULE_URP_ENABLED
            k_BackgroundShaderKeywordURP,
#endif // !MODULE_URP_ENABLED
        };

        /// <summary>
        /// The names for the background shaders based on the current render pipeline.
        /// </summary>
        /// <value>The names for the background shaders based on the current render pipeline.</value>
        /// <remarks>
        /// <para>There are two shaders in the Apple ARKit Provider Package. One is used for rendering
        /// before opaques and one is used for rendering after opaques.</para>
        ///<para>In order:
        /// <list type="number">
        /// <item>Before Opaques Shader Name</item>
        /// <item>After Opaques Shader Name</item>
        /// </list>
        /// </para>
        /// </remarks>
        public static readonly string[] backgroundShaderNames = new[]
        {
            k_BeforeOpaquesBackgroundShaderName,
            k_AfterOpaquesBackgroundShaderName
        };

        /// <summary>
        /// The name for the background shader.
        /// </summary>
        /// <value>The name for the background shader.</value>
        [Obsolete("'backgroundShaderName' is obsolete, use backgroundShaderNames instead. (2022/04/04)")]
        public static string backgroundShaderName => k_BeforeOpaquesBackgroundShaderName;

        /// <summary>
        /// The list of shader keywords to avoid during compilation.
        /// </summary>
        /// <value>The list of shader keywords to avoid during compilation.</value>
        internal static List<string> backgroundShaderKeywordsToNotCompile => k_BackgroundShaderKeywordsToNotCompile;

        /// <summary>
        /// Resulting values from setting the camera configuration.
        /// </summary>
        enum CameraConfigurationResult
        {
            /// <summary>
            /// Setting the camera configuration was successful.
            /// </summary>
            Success = 0,

            /// <summary>
            /// Setting camera configuration was not supported by the provider.
            /// </summary>
            Unsupported = 1,

            /// <summary>
            /// The given camera configuration was not valid to be set by the provider.
            /// </summary>
            InvalidCameraConfiguration = 2,

            /// <summary>
            /// The provider session was invalid.
            /// </summary>
            InvalidSession = 3,
        }

        /// <summary>
        /// Provides the camera functionality for the ARKit implementation.
        /// </summary>
        class ARKitProvider : Provider
        {
            /// <summary>
            /// The shader property name for the luminance component of the camera video frame.
            /// </summary>
            /// <value>The shader property name for the luminance component of the camera video frame.</value>
            const string k_TextureYPropertyName = "_textureY";

            /// <summary>
            /// The shader property name for the chrominance components of the camera video frame.
            /// </summary>
            /// <value>The shader property name for the chrominance components of the camera video frame.</value>
            const string k_TextureCbCrPropertyName = "_textureCbCr";

            /// <summary>
            /// The shader property name identifier for the luminance component of the camera video frame.
            /// </summary>
            /// <value>The shader property name identifier for the luminance component of the camera video frame.</value>
            static readonly int k_TextureYPropertyNameId = Shader.PropertyToID(k_TextureYPropertyName);

            /// <summary>
            /// The shader property name identifier for the chrominance components of the camera video frame.
            /// </summary>
            /// <value>The shader property name identifier for the chrominance components of the camera video frame.</value>
            static readonly int k_TextureCbCrPropertyNameId = Shader.PropertyToID(k_TextureCbCrPropertyName);

            /// <summary>
            /// The shader keywords to enable when the Legacy RP is enabled.
            /// </summary>
            /// <value>The shader keywords to enable when the Legacy RP is enabled.</value>
            static readonly List<string> k_LegacyRPEnabledMaterialKeywords = null;

            /// <summary>
            /// The shader keywords to disable when the Legacy RP is enabled.
            /// </summary>
            /// <value>The shader keywords to disable when the Legacy RP is enabled.</value>
            static readonly List<string> k_LegacyRPDisabledMaterialKeywords = new() { k_BackgroundShaderKeywordURP };

            /// <summary>
            /// The current <see cref="RenderingThreadingMode"/> use by Unity's rendering pipeline.
            /// </summary>
            /// <value>Current <see cref="RenderingThreadingMode"/> use by Unity's rendering pipeline.</value>
            static readonly RenderingThreadingMode k_RenderingThreadingMode = SystemInfo.renderingThreadingMode;

            /// <summary>
            /// Indicates whether multithreaded rendering is enabled.
            /// </summary>
            /// <value><see langword="true"/> if multithreaded rendering is enabled. Otherwise, <see langword="false"/>.</value>
            static readonly bool k_MultithreadedRenderingEnabled =
                k_RenderingThreadingMode == RenderingThreadingMode.MultiThreaded ||
                k_RenderingThreadingMode == RenderingThreadingMode.NativeGraphicsJobs;

#if MODULE_URP_ENABLED
            /// <summary>
            /// The shader keywords to enable when URP is enabled.
            /// </summary>
            /// <value>The shader keywords to enable when URP is enabled.</value>
            static readonly List<string> k_URPEnabledMaterialKeywords = new List<string>() {k_BackgroundShaderKeywordURP};

            /// <summary>
            /// The shader keywords to disable when URP is enabled.
            /// </summary>
            /// <value>The shader keywords to disable when URP is enabled.</value>
            static readonly List<string> k_URPDisabledMaterialKeywords = null;
#endif // MODULE_URP_ENABLED

            Material m_BeforeOpaqueCameraMaterial;
            Material m_AfterOpaqueCameraMaterial;

            /// <summary>
            /// Get the Material used by <c>XRCameraSubsystem</c> to render the camera texture.
            /// </summary>
            /// <returns>The Material to render the camera texture.</returns>
            /// <remarks>
            /// This subsystem will lazily create the camera materials depending on the <see cref="currentBackgroundRenderingMode"/>.
            /// Once created, the materials exist for the lifespan of the subsystem.
            /// </remarks>
            public override Material cameraMaterial
            {
                get
                {
                    switch (currentBackgroundRenderingMode)
                    {
                        case XRCameraBackgroundRenderingMode.BeforeOpaques:
                            return m_BeforeOpaqueCameraMaterial ??= CreateCameraMaterial(k_BeforeOpaquesBackgroundShaderName);

                        case XRCameraBackgroundRenderingMode.AfterOpaques:
                            return m_AfterOpaqueCameraMaterial ??= CreateCameraMaterial(k_AfterOpaquesBackgroundShaderName);

                        default:
                            Debug.LogError($"Unable to create material for unknown background rendering mode {currentBackgroundRenderingMode}.");
                            return null;
                    }
                }
            }

            /// <summary>
            /// Whether camera permission has been granted.
            /// </summary>
            /// <value><see langword="true"/> if camera permission has been granted for this app. Otherwise, <see langword="false"/>.</value>
            public override bool permissionGranted => NativeApi.UnityARKit_Camera_IsCameraPermissionGranted();

            /// <summary>
            /// Constructs the ARKit camera functionality provider.
            /// </summary>
            public ARKitProvider()
            {
                NativeApi.UnityARKit_Camera_Construct(k_TextureYPropertyNameId, k_TextureCbCrPropertyNameId,
                    k_MultithreadedRenderingEnabled);
            }

            public override Feature currentCamera => NativeApi.UnityARKit_Camera_GetCurrentCamera();

            /// <summary>
            /// Get the currently active camera or set the requested camera.
            /// </summary>
            public override Feature requestedCamera
            {
                get => Api.GetRequestedFeatures();
                set
                {
                    Api.SetFeatureRequested(Feature.AnyCamera, false);
                    Api.SetFeatureRequested(value, true);
                }
            }

            /// <summary>
            /// Describes the subsystem's current <see cref="XRCameraBackgroundRenderingMode"/>.
            /// </summary>
            public override XRCameraBackgroundRenderingMode currentBackgroundRenderingMode
            {
                get
                {
                    switch (m_RequestedCameraRenderingMode)
                    {
                        case XRSupportedCameraBackgroundRenderingMode.Any:
                        case XRSupportedCameraBackgroundRenderingMode.BeforeOpaques:
                            return XRCameraBackgroundRenderingMode.BeforeOpaques;

                        case XRSupportedCameraBackgroundRenderingMode.AfterOpaques:
                            return XRCameraBackgroundRenderingMode.AfterOpaques;

                        case XRSupportedCameraBackgroundRenderingMode.None:
                        default:
                            return XRCameraBackgroundRenderingMode.None;
                    }
                }
            }

            /// <inheritdoc />
            public override XRSupportedCameraBackgroundRenderingMode requestedBackgroundRenderingMode
            {
                get => m_RequestedCameraRenderingMode;
                set => m_RequestedCameraRenderingMode = value;
            }

            XRSupportedCameraBackgroundRenderingMode m_RequestedCameraRenderingMode = XRSupportedCameraBackgroundRenderingMode.Any;

            /// <inheritdoc />
            public override XRSupportedCameraBackgroundRenderingMode supportedBackgroundRenderingMode
                => XRSupportedCameraBackgroundRenderingMode.Any;

            /// <summary>
            /// Start the camera functionality.
            /// </summary>
            public override void Start() => NativeApi.UnityARKit_Camera_Start();

            /// <summary>
            /// Stop the camera functionality.
            /// </summary>
            public override void Stop() => NativeApi.UnityARKit_Camera_Stop();

            /// <summary>
            /// Destroy any resources required for the camera functionality.
            /// </summary>
            public override void Destroy() => NativeApi.UnityARKit_Camera_Destruct();

            /// <summary>
            /// Get the current camera frame for the subsystem.
            /// </summary>
            /// <param name="cameraParams">The current Unity <c>Camera</c> parameters.</param>
            /// <param name="cameraFrame">The current camera frame returned by the method.</param>
            /// <returns><see langword="true"/> if the method successfully got a frame. Otherwise, <see langword="false"/>.</returns>
            public override bool TryGetFrame(XRCameraParams cameraParams, out XRCameraFrame cameraFrame)
            {
                return NativeApi.UnityARKit_Camera_TryGetFrame(cameraParams, out cameraFrame);
            }

            public override bool autoFocusEnabled => NativeApi.UnityARKit_Camera_GetAutoFocusEnabled();

            /// <summary>
            /// Get or set the requested focus mode for the camera.
            /// </summary>
            public override bool autoFocusRequested
            {
                get => Api.GetRequestedFeatures().All(Feature.AutoFocus);
                set => Api.SetFeatureRequested(Feature.AutoFocus, value);
            }

            /// <summary>
            /// Gets the current light estimation mode as reported by the
            /// [ARSession's configuration](https://developer.apple.com/documentation/arkit/arconfiguration/2923546-lightestimationenabled).
            /// </summary>
            public override Feature currentLightEstimation => NativeApi.GetCurrentLightEstimation();

            /// <summary>
            /// Get or set the requested light estimation mode.
            /// </summary>
            public override Feature requestedLightEstimation
            {
                get => Api.GetRequestedFeatures();
                set
                {
                    Api.SetFeatureRequested(Feature.AnyLightEstimation, false);
                    Api.SetFeatureRequested(value.Intersection(Feature.AnyLightEstimation), true);
                }
            }

            /// <summary>
            /// Get the camera intrinsics information.
            /// </summary>
            /// <param name="cameraIntrinsics">The camera intrinsics information returned from the method.</param>
            /// <returns><see langword="true"/> if the method successfully gets the camera intrinsics information.
            /// Otherwise, <see langword="false"/>.</returns>
            public override bool TryGetIntrinsics(out XRCameraIntrinsics cameraIntrinsics)
            {
                return NativeApi.UnityARKit_Camera_TryGetIntrinsics(out cameraIntrinsics);
            }

            /// <summary>
            /// Queries the supported camera configurations.
            /// </summary>
            /// <param name="defaultCameraConfiguration">A default value used to fill the returned array before copying in
            /// real values. This ensures future additions to this struct are backwards compatible.</param>
            /// <param name="allocator">The allocation strategy to use for the returned data.</param>
            /// <returns>The supported camera configurations.</returns>
            public override NativeArray<XRCameraConfiguration> GetConfigurations(
                XRCameraConfiguration defaultCameraConfiguration,
                Allocator allocator)
            {
                IntPtr configurations = NativeApi.UnityARKit_Camera_AcquireConfigurations(
                    out int configurationsCount,
                    out int configurationSize);

                try
                {
                    unsafe
                    {
                        return NativeCopyUtility.PtrToNativeArrayWithDefault(
                            defaultCameraConfiguration,
                            (void*)configurations,
                            configurationSize, configurationsCount,
                            allocator);
                    }
                }
                finally
                {
                    NativeApi.UnityARKit_Camera_ReleaseConfigurations(configurations);
                }
            }

            /// <summary>
            /// The current camera configuration.
            /// </summary>
            /// <value>The current camera configuration if it exists. Otherwise, <see langword="null"/>.</value>
            /// <exception cref="System.ArgumentException">Thrown when setting the current configuration if the given
            /// configuration is not a valid, supported camera configuration.</exception>
            /// <exception cref="System.InvalidOperationException">Thrown when setting the current configuration if the
            /// implementation is unable to set the current camera configuration for various reasons such as:
            /// <list type="bullet">
            /// <item><description>Version of iOS does not support camera configurations.</description></item>
            /// <item><description>ARKit session is invalid.</description></item>
            /// </list>
            /// </exception>
            public override XRCameraConfiguration? currentConfiguration
            {
                get
                {
                    if (NativeApi.UnityARKit_Camera_TryGetCurrentConfiguration(out XRCameraConfiguration cameraConfiguration))
                    {
                        return cameraConfiguration;
                    }

                    return null;
                }
                set
                {
                    // Assert that the camera configuration is not null.
                    // The XRCameraSubsystem should have already checked this.
                    Debug.Assert(value != null, "Cannot set the current camera configuration to null");

                    switch (NativeApi.UnityARKit_Camera_TrySetCurrentConfiguration((XRCameraConfiguration)value))
                    {
                        case CameraConfigurationResult.Success:
                            break;
                        case CameraConfigurationResult.Unsupported:
                            throw new InvalidOperationException(
                                "cannot set camera configuration because ARKit version does not support camera configurations");
                        case CameraConfigurationResult.InvalidCameraConfiguration:
                            throw new ArgumentException(
                                "camera configuration does not exist in the available configurations", nameof(value));
                        case CameraConfigurationResult.InvalidSession:
                            throw new InvalidOperationException(
                                "cannot set camera configuration because the ARKit session is not valid");
                        default:
                            throw new InvalidOperationException("cannot set camera configuration for ARKit");
                    }
                }
            }

            /// <summary>
            /// Gets the texture descriptors associated with the current camera frame.
            /// </summary>
            /// <param name="defaultDescriptor">Default descriptor.</param>
            /// <param name="allocator">Allocator.</param>
            /// <returns>The texture descriptors.</returns>
            public override unsafe NativeArray<XRTextureDescriptor> GetTextureDescriptors(
                XRTextureDescriptor defaultDescriptor,
                Allocator allocator)
            {
                var textureDescriptors = NativeApi.UnityARKit_Camera_AcquireTextureDescriptors(
                    out int length, out int elementSize);

                try
                {
                    return NativeCopyUtility.PtrToNativeArrayWithDefault(
                        defaultDescriptor,
                        textureDescriptors, elementSize, length, allocator);
                }
                finally
                {
                    NativeApi.UnityARKit_Camera_ReleaseTextureDescriptors(textureDescriptors);
                }
            }

            /// <summary>
            /// Get the enabled and disabled shader keywords for the material.
            /// </summary>
            /// <param name="enabledKeywords">The keywords to enable for the material.</param>
            /// <param name="disabledKeywords">The keywords to disable for the material.</param>
            public override void GetMaterialKeywords(out List<string> enabledKeywords, out List<string> disabledKeywords)
            {
                if (GraphicsSettings.currentRenderPipeline == null)
                {
                    enabledKeywords = k_LegacyRPEnabledMaterialKeywords;
                    disabledKeywords = k_LegacyRPDisabledMaterialKeywords;
                }
#if MODULE_URP_ENABLED
                else if (GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset)
                {
                    enabledKeywords = k_URPEnabledMaterialKeywords;
                    disabledKeywords = k_URPDisabledMaterialKeywords;
                }
#endif // MODULE_URP_ENABLED
                else
                {
                    enabledKeywords = null;
                    disabledKeywords = null;
                }
            }

            /// <summary>
            /// An instance of the <see cref="XRCpuImage.Api"/> used to operate on <see cref="XRCpuImage"/> objects.
            /// </summary>
            public override XRCpuImage.Api cpuImageApi => ARKitCpuImageApi.instance;

            /// <summary>
            /// Query for the latest native camera image.
            /// </summary>
            /// <param name="cameraImageCinfo">The metadata required to construct a <see cref="XRCpuImage"/></param>
            /// <returns><see langword="true"/> if the camera image is acquired. Otherwise, <see langword="false"/>.</returns>
            public override bool TryAcquireLatestCpuImage(out XRCpuImage.Cinfo cameraImageCinfo)
                => ARKitCpuImageApi.TryAcquireLatestImage(ARKitCpuImageApi.ImageType.Camera, out cameraImageCinfo);
            
            /// <summary>
            /// Called on the render thread by background rendering code immediately before the background
            /// is rendered.
            /// For ARKit, this is required in order to free the metal textures retained on the main thread.
            /// </summary>
            /// <param name="id">Platform-specific data.</param>
            public override void OnBeforeBackgroundRender(int id)
            {
                // callback to schedule the release of the metal texture buffers after rendering is complete
                NativeApi.UnityARKit_Camera_ScheduleReleaseTextureBuffers();
            }
        }

        /// <summary>
        /// Create and register the camera subsystem descriptor to advertise a providing implementation for camera
        /// functionality.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Register()
        {
            if (!Api.AtLeast11_0())
                return;

            var cameraSubsystemCinfo = new XRCameraSubsystemCinfo
            {
                id = k_SubsystemId,
                providerType = typeof(ARKitProvider),
                subsystemTypeOverride = typeof(ARKitCameraSubsystem),
                supportsAverageBrightness = false,
                supportsAverageColorTemperature = true,
                supportsColorCorrection = false,
                supportsDisplayMatrix = true,
                supportsProjectionMatrix = true,
                supportsTimestamp = true,
                supportsCameraConfigurations = true,
                supportsCameraImage = true,
                supportsAverageIntensityInLumens = true,
                supportsFocusModes = true,
                supportsFaceTrackingAmbientIntensityLightEstimation = true,
                supportsFaceTrackingHDRLightEstimation = true,
                supportsWorldTrackingAmbientIntensityLightEstimation = true,
                supportsWorldTrackingHDRLightEstimation = false,
                supportsCameraGrain = Api.AtLeast13_0(),
            };

            if (!XRCameraSubsystem.Register(cameraSubsystemCinfo))
            {
                Debug.LogError($"Cannot register the {k_SubsystemId} subsystem");
            }
        }

        /// <summary>
        /// Container to wrap the native ARKit camera APIs.
        /// </summary>
        static class NativeApi
        {
#if UNITY_XR_ARKIT_LOADER_ENABLED
            [DllImport("__Internal", EntryPoint="UnityARKit_Camera_GetCurrentLightEstimation")]
            public static extern Feature GetCurrentLightEstimation();

            [DllImport("__Internal")]
            public static extern void UnityARKit_Camera_Construct(
                int textureYPropertyNameId,
                int textureCbCrPropertyNameId,
                bool mtRenderingEnabled);

            [DllImport("__Internal")]
            public static extern void UnityARKit_Camera_Destruct();

            [DllImport("__Internal")]
            public static extern void UnityARKit_Camera_Start();

            [DllImport("__Internal")]
            public static extern void UnityARKit_Camera_Stop();

            [DllImport("__Internal")]
            public static extern bool UnityARKit_Camera_TryGetFrame(
                XRCameraParams cameraParams,
                out XRCameraFrame cameraFrame);

            [DllImport("__Internal")]
            public static extern bool UnityARKit_Camera_TryGetIntrinsics(out XRCameraIntrinsics cameraIntrinsics);

            [DllImport("__Internal")]
            public static extern bool UnityARKit_Camera_IsCameraPermissionGranted();

            [DllImport("__Internal")]
            public static extern IntPtr UnityARKit_Camera_AcquireConfigurations(
                out int configurationsCount,
                out int configurationSize);

            [DllImport("__Internal")]
            public static extern void UnityARKit_Camera_ReleaseConfigurations(IntPtr configurations);

            [DllImport("__Internal")]
            public static extern bool UnityARKit_Camera_TryGetCurrentConfiguration(
                out XRCameraConfiguration cameraConfiguration);

            [DllImport("__Internal")]
            public static extern CameraConfigurationResult UnityARKit_Camera_TrySetCurrentConfiguration(
                XRCameraConfiguration cameraConfiguration);

            [DllImport("__Internal")]
            public static extern unsafe void* UnityARKit_Camera_AcquireTextureDescriptors(
                out int length, out int elementSize);

            [DllImport("__Internal")]
            public static extern unsafe void UnityARKit_Camera_ReleaseTextureDescriptors(
                void* descriptors);

            [DllImport("__Internal")]
            public static extern Feature UnityARKit_Camera_GetCurrentCamera();

            [DllImport("__Internal")]
            public static extern bool UnityARKit_Camera_GetAutoFocusEnabled();

            [DllImport("__Internal")]
            public static extern void UnityARKit_Camera_ScheduleReleaseTextureBuffers();
#else
            static readonly string k_ExceptionMsg = "Apple ARKit XR Plug-in Provider not enabled in project settings.";

            public static Feature GetCurrentLightEstimation() => Feature.None;


            public static void UnityARKit_Camera_Construct(
                int textureYPropertyNameId,
                int textureCbCrPropertyNameId,
                bool mtRenderingEnabled)
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }

            public static void UnityARKit_Camera_Destruct()
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }

            public static void UnityARKit_Camera_Start()
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }

            public static void UnityARKit_Camera_Stop()
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }

            public static bool UnityARKit_Camera_TryGetFrame(XRCameraParams cameraParams, out XRCameraFrame cameraFrame)
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }

            public static bool UnityARKit_Camera_TryGetIntrinsics(out XRCameraIntrinsics cameraIntrinsics)
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }

            public static bool UnityARKit_Camera_IsCameraPermissionGranted() => false;

            public static IntPtr UnityARKit_Camera_AcquireConfigurations(
                out int configurationsCount,
                out int configurationSize)
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }

            public static void UnityARKit_Camera_ReleaseConfigurations(IntPtr configurations)
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }

            public static bool UnityARKit_Camera_TryGetCurrentConfiguration(out XRCameraConfiguration cameraConfiguration)
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }

            public static CameraConfigurationResult UnityARKit_Camera_TrySetCurrentConfiguration(XRCameraConfiguration cameraConfiguration)
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }

            public static unsafe void* UnityARKit_Camera_AcquireTextureDescriptors(
                out int length,
                out int elementSize)
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }

            public static unsafe void UnityARKit_Camera_ReleaseTextureDescriptors(void* descriptors)
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }

            public static void UnityARKit_Camera_ScheduleReleaseTextureBuffers()
            {
                throw new System.NotImplementedException(k_ExceptionMsg);
            }

            public static Feature UnityARKit_Camera_GetCurrentCamera() => Feature.None;

            public static bool UnityARKit_Camera_GetAutoFocusEnabled() => false;
#endif
        }
    }
}
