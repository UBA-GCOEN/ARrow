using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.XR.CoreUtils;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    class CameraTextureProvider : MonoBehaviour
    {
        internal static event Action<Camera> preRenderCamera;
        internal static event Action<Camera> postRenderCamera;
        internal event Action<CameraTextureFrameEventArgs> cameraFrameReceived;

        Camera m_XrCamera;
        Camera m_SimulationRenderCamera;
        RenderTexture m_SimulationRenderTexture;
        Texture2D m_SimulationProviderTexture;
        IntPtr m_TexturePtr;
        CameraTextureFrameEventArgs? m_CameraFrameEventArgs;
        SimulationXRayManager m_XRayManager;

        readonly List<Texture2D> m_CameraImagePlanes = new List<Texture2D>();

        internal CameraTextureFrameEventArgs? CameraFrameEventArgs => m_CameraFrameEventArgs;

        bool m_Initialized;

        internal static CameraTextureProvider AddTextureProviderToCamera(Camera simulationCamera, Camera xrCamera)
        {
            simulationCamera.depth = xrCamera.depth - 1;
            simulationCamera.cullingMask = 1 << XRSimulationRuntimeSettings.Instance.environmentLayer;
            simulationCamera.clearFlags = CameraClearFlags.Color;
            simulationCamera.backgroundColor = Color.clear;

            var cameraTextureProvider = simulationCamera.gameObject.AddComponent<CameraTextureProvider>();
            cameraTextureProvider.InitializeProvider(xrCamera, simulationCamera);

            return cameraTextureProvider;
        }

        void OnDestroy()
        {
            BaseSimulationSceneManager.environmentSetupFinished -= OnEnvironmentSetupFinished;

            if (m_SimulationRenderCamera != null)
                m_SimulationRenderCamera.targetTexture = null;

            if (m_SimulationRenderTexture != null)
                m_SimulationRenderTexture.Release();

            if (m_SimulationProviderTexture != null)
                UnityObjectUtils.Destroy(m_SimulationProviderTexture);
        }

        void Update()
        {
            if (!m_Initialized)
                return;

            // Currently assuming the main camera is being set to the correct settings for rendering to the target device
            m_XrCamera.ResetProjectionMatrix();
            CopyLimitedSettingsToCamera(m_XrCamera, m_SimulationRenderCamera);
            DoCameraRender(m_SimulationRenderCamera);

            if (!m_SimulationRenderTexture.IsCreated() && !m_SimulationRenderTexture.Create())
                return;

            if (m_SimulationProviderTexture.width != m_SimulationRenderTexture.width
                || m_SimulationProviderTexture.height != m_SimulationRenderTexture.height)
            {
                if (!m_SimulationProviderTexture.Reinitialize(m_SimulationRenderTexture.width, m_SimulationRenderTexture.height))
                    return;

                m_TexturePtr = m_SimulationProviderTexture.GetNativeTexturePtr();
            }

            Graphics.CopyTexture(m_SimulationRenderTexture, m_SimulationProviderTexture);

            m_CameraImagePlanes.Clear();
            m_CameraImagePlanes.Add(m_SimulationProviderTexture);

            var frameEventArgs = new CameraTextureFrameEventArgs
            {
                timestampNs = (long)(Time.time * 1e9),
                projectionMatrix = m_XrCamera.projectionMatrix,
                textures = m_CameraImagePlanes,
            };

            cameraFrameReceived?.Invoke(frameEventArgs);
        }

        void InitializeProvider(Camera xrCamera, Camera simulationCamera)
        {
            m_XRayManager = new SimulationXRayManager();

            m_XrCamera = xrCamera;
            m_SimulationRenderCamera = simulationCamera;
            CopyLimitedSettingsToCamera(m_XrCamera, m_SimulationRenderCamera);

            var descriptor = new RenderTextureDescriptor(m_XrCamera.scaledPixelWidth, m_XrCamera.scaledPixelHeight);

            // Need to make sure we set the graphics format to our valid format
            // or we will get an out of range value for the render texture format
            // when we try creating the render texture
            descriptor.graphicsFormat = SystemInfo.GetGraphicsFormat(DefaultFormat.LDR);
            // Need to enable depth buffer if the target camera did not already have it.
            if (descriptor.depthBufferBits < 24)
                descriptor.depthBufferBits = 24;

            m_SimulationRenderTexture = new RenderTexture(descriptor)
            {
                name = "XR Render Camera",
                hideFlags = HideFlags.HideAndDontSave,
            };

            if (m_SimulationRenderTexture.Create())
                m_SimulationRenderCamera.targetTexture = m_SimulationRenderTexture;

            if (m_SimulationProviderTexture == null)
            {
                m_SimulationProviderTexture = new Texture2D(descriptor.width, descriptor.height, descriptor.graphicsFormat, 1, TextureCreationFlags.None)
                {
                    name = "Simulated Native Camera Texture",
                    hideFlags = HideFlags.HideAndDontSave
                };
                m_TexturePtr = m_SimulationProviderTexture.GetNativeTexturePtr();
            }

            BaseSimulationSceneManager.environmentSetupFinished += OnEnvironmentSetupFinished;

            m_Initialized = true;
        }

        void OnEnvironmentSetupFinished()
        {
            var simulationSceneManager = SimulationSessionSubsystem.simulationSceneManager;
            m_SimulationRenderCamera.scene = simulationSceneManager.environmentScene;
        }

        static void CopyLimitedSettingsToCamera(Camera source, Camera destination)
        {
            var scene = destination.scene;
            var cullingMask = destination.cullingMask;
            var clearFlags = destination.clearFlags;
            var backgroundColor = destination.backgroundColor;
            var depth = destination.depth;
            var targetTexture = destination.targetTexture;
            var localPosition = destination.transform.localPosition;
            var localRotation = destination.transform.localRotation;
            var localScale = destination.transform.localScale;

            destination.CopyFrom(source);
            destination.projectionMatrix = source.projectionMatrix;

            destination.scene = scene;
            destination.cullingMask = cullingMask;
            destination.clearFlags = clearFlags;
            destination.backgroundColor = backgroundColor;
            destination.depth = depth;
            destination.targetTexture = targetTexture;
            destination.transform.localPosition = localPosition;
            destination.transform.localRotation = localRotation;
            destination.transform.localScale = localScale;
        }

        void DoCameraRender(Camera renderCamera)
        {
            m_XRayManager.UpdateXRayShader(false, null);

            preRenderCamera?.Invoke(renderCamera);
            renderCamera.Render();
            postRenderCamera?.Invoke(renderCamera);
        }

        internal void TryGetTextureDescriptors(out NativeArray<XRTextureDescriptor> planeDescriptors,
            Allocator allocator)
        {
            Shader.SetGlobalTexture(SimulationCameraSubsystem.textureSinglePropertyNameId, m_SimulationProviderTexture);

            var isValid = TryGetLatestImagePtr(out var nativePtr);

            var descriptors = new XRTextureDescriptor[1];
            if (isValid)
            {
                descriptors[0] = new XRTextureDescriptor(nativePtr, m_SimulationProviderTexture.width, m_SimulationProviderTexture.height,
                m_SimulationProviderTexture.mipmapCount, m_SimulationProviderTexture.format, SimulationCameraSubsystem.textureSinglePropertyNameId, 0,
                TextureDimension.Tex2D);
            }
            else
                descriptors[0] = default;

            planeDescriptors = new NativeArray<XRTextureDescriptor>(descriptors, allocator);
        }

        internal bool TryGetLatestImagePtr(out IntPtr nativePtr)
        {
            if (m_CameraImagePlanes != null && m_CameraImagePlanes.Count > 0 && m_SimulationProviderTexture != null
                && m_SimulationProviderTexture.isReadable)
            {
                nativePtr =  m_TexturePtr;
                return true;
            }

            nativePtr = IntPtr.Zero;
            return false;
        }
    }
}
