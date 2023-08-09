#if URP_14_0_0_OR_NEWER
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.Management;

namespace URP.SegmentedDimmer
{
    [DisallowMultipleRendererFeature]
    [Tooltip("Segmented Dimmer will render tagged objects to a render target that can then be used to configure the Magic Leap Segmented Dimmer feature.")]
    public class SegmentedDimmer : ScriptableRendererFeature
    {
        [Flags]
        private enum DimmerTechnique
        {
            Undefined                       = 0,
            RenderTargetAndManualAlphaBlit  = 0b0001,
            RenderTargetAndYFlipBlit        = 0b0010,
            DirectRender                    = 0b0100,
            DirectRenderToXRTarget          = 0b1000
        };
        private const DimmerTechnique TechniqueNeedRenderTargetTexture = DimmerTechnique.RenderTargetAndManualAlphaBlit 
                                                                | DimmerTechnique.RenderTargetAndYFlipBlit;


        private const string AlphaBlitShaderName = "Hidden/MagicLeap/Universal Render Pipeline/SegmentedDimmer/AlphaBlitShader";
        private const string AlphaClearShaderName = "Hidden/MagicLeap/Universal Render Pipeline/SegmentedDimmer/AlphaClearShader";
        private const string SegmentedDimmerShaderName = "Hidden/MagicLeap/Universal Render Pipeline/SegmentedDimmer/MaskShader";
        private const RenderPassEvent RenderPassEventBeforeScene = RenderPassEvent.BeforeRenderingPrePasses;
        private const RenderPassEvent RenderPassEventAfterEverything = RenderPassEvent.AfterRendering + 100;

        [Serializable]
        public class SegmentedDimmerSettings
        {
            [SerializeField] public bool isEnabled = true;

            [Header("General")]
            [SerializeField] public LayerMask layerMask;
            [Tooltip("Clear value for the Segmented Dimmer texture, values can be between [0..1]")]
            [SerializeField, Range(0.0f, 1.0f)] public float clearValue = 0.0f;

            // overrideMaterial is now deprecated, we are keeping it around since we could need it later.
            // Allow users to re-add it by adding MAGIC_LEAP_SEGMENTED_DIMMER_EXPOSE_OVERRIDE_MATERIAL_OPTION define to the project
            [Tooltip("DEPRECATED - When this option is true, URP will override all the mesh materials with a fully opaque shader")]
        #if !MAGIC_LEAP_SEGMENTED_DIMMER_EXPOSE_OVERRIDE_MATERIAL_OPTION
            [HideInInspector] 
        #endif
            [SerializeField] public bool overrideMaterial = false;

            [Header("Resolution")]
            [Tooltip("Using the full resolution will try to render the mask directly in the alpha channel of the color target, if possible")]
            [SerializeField] public bool useFullResolution = true;
            [Tooltip("Only used if useFullResolution is disabled, size of the render target to use for the dimmer")]
            [SerializeField] public Vector2Int renderTextureSize = new Vector2Int(256, 256);

            // Shaders will be auto-populated by the editor, this will make sure the shaders are actually built
            [SerializeField, HideInInspector] public Shader alphaBlitShader = null;
            [SerializeField, HideInInspector] public Shader alphaClearShader = null;
            [SerializeField, HideInInspector] public Shader segmentedDimmerShader = null;
        }
        [SerializeField] public SegmentedDimmerSettings settings = new SegmentedDimmerSettings();

        private DimmerTechnique m_DimmerTechnique = DimmerTechnique.Undefined;
        private bool needRenderTargetTexture
        {
            get { return (m_DimmerTechnique & TechniqueNeedRenderTargetTexture) != 0; }
        }

        private DimmerTechnique GetFallbackDimmerTechnique()
        {
            return NeedSegmentedDimmerScriptableRenderPassBlit() || m_XRMultipass
                ? DimmerTechnique.RenderTargetAndManualAlphaBlit
                : DimmerTechnique.RenderTargetAndYFlipBlit;
        }

        private RTHandle m_RenderTexture;
        private RTHandle[] m_XRRenderTexture = new RTHandle[2] {null, null};

        private SegmentedDimmerPass[] m_ScriptablePass = new SegmentedDimmerPass[2] {null, null};
        private SegmentedDimmerAlphaBlit[] m_AlphaBlitPass = new SegmentedDimmerAlphaBlit[2] {null, null};

        private Material m_AlphaClearMaterial;
        private Material m_SegmentedDimmerMaterial;
        private Material m_AlphaBlitMaterial;

        private bool m_WasActive = false;
        private bool[] m_XRPassFormatChecked = new bool[2] {false,false};
        private bool m_XRMultipass = false;
        private bool m_InitDone = false;

        public override void Create()
        {
            Init();
        }

        private bool Init()
        {
        #if !MAGIC_LEAP_SEGMENTED_DIMMER_EXPOSE_OVERRIDE_MATERIAL_OPTION
            // Deprecating "overrideMaterial", but we will keep it around until we are sure we won't use it
            if (settings.overrideMaterial)
                settings.overrideMaterial = false;
        #endif

            if (m_InitDone)
                return true;

            if (!LoadMaterial())
            {
                m_InitDone = false;
                return false;
            }

            // If an app has enabled the SegmentedDimmer feature but doesn't
            // enable 'Magic Leap' in the XR Plugin Management settings, then
            // we should not proceed with this function, otherwise the
            // UnityMagicLeap native library functions fails because it doesn't
            // get packaged in the apk.
            // One case of this happening is when a project uses OpenXR instead
            // of this XR plugin.
            bool isMagicLeapLoaderActive = XRSettings.enabled &&
                                           XRGeneralSettings.Instance != null &&
                                           XRGeneralSettings.Instance.Manager != null &&
                                           XRGeneralSettings.Instance.Manager.ActiveLoaderAs<MagicLeapLoader>() != null;
            
            if (!isMagicLeapLoaderActive)
            {
                m_InitDone = false;
                return false;
            }

            var mlSettings = MagicLeapSettings.currentSettings;
            bool singlePassEnabled = (mlSettings == null || !mlSettings.forceMultipass);
            m_XRMultipass = !singlePassEnabled;
            m_XRPassFormatChecked = new[] {false,false};
            m_DimmerTechnique = DimmerTechnique.Undefined;

            // Select the technique to use for writing the Alpha
            if (settings.useFullResolution)
            {
                m_DimmerTechnique = DimmerTechnique.DirectRender;
            }
            else
            {
                if (NeedSegmentedDimmerScriptableRenderPassBlit())
                {
                    if (m_AlphaBlitMaterial == null)
                    {
                        m_DimmerTechnique = DimmerTechnique.Undefined;
                        
                        m_InitDone = true;
                        return true;
                    }
                    m_DimmerTechnique = DimmerTechnique.RenderTargetAndManualAlphaBlit;
                }
                else
                {
                    m_DimmerTechnique = m_XRMultipass ? DimmerTechnique.RenderTargetAndManualAlphaBlit : DimmerTechnique.RenderTargetAndYFlipBlit;
                }
            }

            if (!isActive)
            {
                m_InitDone = true;
                return true;
            }

            SetupRenderPass(null, 0);
            if (m_XRMultipass)
                SetupRenderPass(null, 1);
            
            m_InitDone = true;
            return true;
        }

        private bool SetupRenderPass(RenderTextureDescriptor? cameraRenderTextureDesc, int passId)
        {
            RenderPassEvent renderPassEvent = 0;
            ColorWriteMask writeMask = ColorWriteMask.Red;
            RTHandle renderTexture = null;
            if (m_DimmerTechnique == DimmerTechnique.DirectRender)
            {
                if (m_AlphaClearMaterial == null)
                {
                    m_DimmerTechnique = DimmerTechnique.Undefined;
                    return false;
                }

                renderPassEvent = RenderPassEventAfterEverything;

                if (m_RenderTexture != null)
                {
                    RTHandles.Release(m_RenderTexture);
                    m_RenderTexture = null;
                }

                writeMask = ColorWriteMask.Alpha;

                renderTexture = null;
            }
            else if (m_DimmerTechnique == DimmerTechnique.DirectRenderToXRTarget)
            {
                if (m_AlphaClearMaterial == null)
                {
                    m_DimmerTechnique = DimmerTechnique.Undefined;
                    return false;
                }

                renderPassEvent = RenderPassEventAfterEverything;

                if (m_RenderTexture != null)
                {
                    RTHandles.Release(m_RenderTexture);
                    m_RenderTexture = null;
                }

                writeMask = ColorWriteMask.Alpha;

                renderTexture = m_XRRenderTexture[passId];
            }
            else if (needRenderTargetTexture)
            {
                int textureWidth = settings.renderTextureSize.x;
                int textureHeight = settings.renderTextureSize.y;
                if (settings.useFullResolution)
                {
                    // If we don't currently have camera information, we can deffer the creation of the texture/renderpass 
                    if (!cameraRenderTextureDesc.HasValue)
                        return false;

                    textureWidth = cameraRenderTextureDesc.Value.width;
                    textureHeight = cameraRenderTextureDesc.Value.height;
                }

                renderPassEvent = RenderPassEventBeforeScene;

                // Make sure the render texture is correctly configured for Single-Pass rendering
                int sliceCount = m_XRMultipass ? 1 : 2;
                TextureDimension texDimension = m_XRMultipass ? TextureDimension.Tex2D : TextureDimension.Tex2DArray;
                VRTextureUsage vrUsage = m_XRMultipass ? VRTextureUsage.None : VRTextureUsage.TwoEyes;
                m_RenderTexture = RTHandles.Alloc(textureWidth, textureHeight, sliceCount, 
                    DepthBits.None, GraphicsFormat.R8_UNorm, FilterMode.Point, TextureWrapMode.Clamp,
                    texDimension, false, false, false, false,
                    1, 0, MSAASamples.None, false, false,
                    RenderTextureMemoryless.None, vrUsage, "SegmentedDimmer");

                if (m_RenderTexture == null)
                {
                    m_DimmerTechnique = DimmerTechnique.Undefined;
                    return false;
                }

                renderTexture = m_RenderTexture;
            }

            // The render pass that will generate the Segmented dimmer mask 
            m_ScriptablePass[passId] = new SegmentedDimmerPass("SegmentedDimmerRenderPass",
                renderPassEvent, renderTexture, writeMask, settings.layerMask.value, settings.clearValue);
            m_AlphaBlitPass[passId] = null;

            switch (m_DimmerTechnique)
            {
                case DimmerTechnique.DirectRender:
                case DimmerTechnique.DirectRenderToXRTarget:
                {
                    m_ScriptablePass[passId].clearAlphaMaterial = m_AlphaClearMaterial;
                    m_ScriptablePass[passId].alphaClearMaterialPassIndex = 0;
                    SetSegmentedDimmerKeepAlpha(true);
                    break;
                }
                case DimmerTechnique.RenderTargetAndYFlipBlit:
                {
                    // Use the yflip render pass that is controlled by the c++ code to blit the mask in the alpha channel
                    Texture texture = m_RenderTexture;
                    SetSegmentedDimmerTexture(texture.GetNativeTexturePtr());
                    break;
                }
                case DimmerTechnique.RenderTargetAndManualAlphaBlit:
                {
                    // If we can't pass the task of writing the mask to the underlying c++ code, add a blit pass to do it
                    m_AlphaBlitPass[passId] = 
                        new SegmentedDimmerAlphaBlit("SegmentedDimmerAlphaBlit",
                                                     RenderPassEventAfterEverything,
                                                     m_RenderTexture,
                                                     m_AlphaBlitMaterial);
                    SetSegmentedDimmerKeepAlpha(true);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

#if !MAGIC_LEAP_SEGMENTED_DIMMER_EXPOSE_OVERRIDE_MATERIAL_OPTION
            if (settings.overrideMaterial && m_SegmentedDimmerMaterial != null)
            {
                m_ScriptablePass[passId].overrideMaterial = m_SegmentedDimmerMaterial;
                m_ScriptablePass[passId].overrideMaterialPassIndex = 0;
            }
#endif

            // Make sure Segmented dimmer feature is active from now on
            SegmentedDimmerEnable(true);

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (!m_InitDone)
                return;

            // Make sure Segmented dimmer feature is deactivated from now on
            SetSegmentedDimmerKeepAlpha(false);
            SetSegmentedDimmerTexture(IntPtr.Zero);
            SegmentedDimmerEnable(false);

            if (m_RenderTexture != null)
                RTHandles.Release(m_RenderTexture);

            if (m_XRRenderTexture[0] != null)
                RTHandles.Release(m_XRRenderTexture[0]);
            if (m_XRRenderTexture[1] != null)
                RTHandles.Release(m_XRRenderTexture[1]);

            m_InitDone = false;
        }

        // Try to retrieve XRPass info from URP, if the xr data is missing or the internal API changed, we will return null
        private XRPass GetXRPass(ref RenderingData renderingData)
        {
            CameraData cameraData = renderingData.cameraData;
            Type type = cameraData.GetType();
            FieldInfo xrField = type.GetField("xr", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (xrField == null) return null;

            return (XRPass)xrField.GetValue(cameraData);
        }

        // Try to retrieve the pass id from XRPass, if we are in multi-pass (single-pass will always return id 0)
        private int GetXRPassId(ref RenderingData renderingData)
        {
            if (!m_XRMultipass)
                return 0;

            XRPass xrPass = GetXRPass(ref renderingData);
            return xrPass.multipassId;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (!m_InitDone && !Init())
                return;

            SegmentedDimmerEnable(settings.isEnabled);

            // Only add the renderpass if we are in XR more and enabled
            if (!settings.isEnabled || !renderingData.cameraData.xrRendering || m_DimmerTechnique == DimmerTechnique.Undefined)
                return;

            // Never add the pass in 'Preview' mode
            if (renderingData.cameraData.camera.cameraType == CameraType.Preview)
                return;

            int passId = GetXRPassId(ref renderingData);

            // Validate that we have the best render strategy based on if the camera or XR target have an alpha channel
            if (!ValidateRenderTargetAlpha(ref renderingData, passId)) 
                return;

            renderer.EnqueuePass(m_ScriptablePass[passId]);

            if (m_AlphaBlitPass[passId] != null)
                renderer.EnqueuePass(m_AlphaBlitPass[passId]);
        }

        private bool ValidateRenderTargetAlpha(ref RenderingData renderingData, int passId)
        {
            // First check if we can use the XR final target alpha channel to do a direct render
            RenderTextureDescriptor cameraRenderTextureDesc = renderingData.cameraData.cameraTargetDescriptor;
            if ((m_DimmerTechnique == DimmerTechnique.DirectRender
                    && (!GraphicsFormatUtility.HasAlphaChannel(cameraRenderTextureDesc.graphicsFormat) 
                        || (cameraRenderTextureDesc.flags & RenderTextureCreationFlags.EyeTexture) == 0))
                || (m_DimmerTechnique == DimmerTechnique.DirectRenderToXRTarget && !m_XRPassFormatChecked[passId]))
            {
                if (!m_XRPassFormatChecked[passId])
                {
                    // XRPass is not publicly exposed, so if it is possible to get the data, we'll use it, otherwise,
                    // we will fallback to using a separate alpha texture.
                    XRPass xr = GetXRPass(ref renderingData);
                    if (xr != null && GraphicsFormatUtility.HasAlphaChannel(xr.renderTargetDesc.graphicsFormat))
                    {
                        m_XRRenderTexture[passId] = RTHandles.Alloc(xr.renderTarget);
                        m_DimmerTechnique = DimmerTechnique.DirectRenderToXRTarget;
                    }
                    else
                    {
                        m_DimmerTechnique = GetFallbackDimmerTechnique();
                    }

                    m_XRPassFormatChecked[passId] = true;
                }
                else if (m_XRRenderTexture[passId] != null)
                {
                    m_DimmerTechnique = DimmerTechnique.DirectRenderToXRTarget;
                }

                if (!SetupRenderPass(null, passId))
                    return false;
            }
            // Next we check if we can use the camera's alpha channel to do a direct render
            else if (m_DimmerTechnique == DimmerTechnique.DirectRender
                && !GraphicsFormatUtility.HasAlphaChannel(cameraRenderTextureDesc.graphicsFormat))
            {
                m_DimmerTechnique = GetFallbackDimmerTechnique();
                if (!SetupRenderPass(cameraRenderTextureDesc, passId))
                    return false;
            }
            // Make sure the render texture is created if we need one
            else if (needRenderTargetTexture && m_RenderTexture == null)
            {
                if (!SetupRenderPass(cameraRenderTextureDesc, passId))
                    return false;
            }

            return true;
        }

        private bool LoadMaterial()
        {
            if (m_AlphaBlitMaterial != null && m_SegmentedDimmerMaterial != null && m_AlphaClearMaterial!= null)
            {
                return true;
            }

            Material CreateMaterialFromShader(ref Shader shaderOut, string shaderName)
            {
                if (shaderOut == null)
                {
                    shaderOut = Shader.Find(shaderName);
                    if (shaderOut == null)
                        return null;
                }

                return CoreUtils.CreateEngineMaterial(shaderOut);
            }

            m_AlphaBlitMaterial = CreateMaterialFromShader(ref settings.alphaBlitShader, AlphaBlitShaderName);
            m_SegmentedDimmerMaterial = CreateMaterialFromShader(ref settings.segmentedDimmerShader, SegmentedDimmerShaderName);
            m_AlphaClearMaterial = CreateMaterialFromShader(ref settings.alphaClearShader, AlphaClearShaderName);

            return m_AlphaBlitMaterial != null && m_SegmentedDimmerMaterial != null && m_AlphaClearMaterial!= null;
        }

        void SegmentedDimmerEnable(bool status)
        {
            if (status != m_WasActive)
            {
                if (status)
                {
                    // TODO : activate segmented dimmer via native API
                }
                else
                {
                    // TODO : deactivate segmented dimmer via native API
                }

                m_WasActive = status;
            }
        }

        // Relish/Android allows us to override the alpha during the yflip, other platforms will need to blit it in a renderpass
        [DllImport("UnityMagicLeap", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UnityMagicLeap_SegmentedDimmer_SetTexture")]
        private static extern void SetSegmentedDimmerTexture(IntPtr texture);

        [DllImport("UnityMagicLeap", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UnityMagicLeap_SegmentedDimmer_NeedScriptableRenderPassBlit")]
        private static extern bool NeedSegmentedDimmerScriptableRenderPassBlit();

        [DllImport("UnityMagicLeap", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UnityMagicLeap_SegmentedDimmer_KeepAlpha")]
        private static extern void SetSegmentedDimmerKeepAlpha(bool status);
    }
}
#endif
