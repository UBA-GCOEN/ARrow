#if URP_14_0_0_OR_NEWER
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

#if UNITY_2023_1
using UnityEngine.Rendering.RendererUtils;
#endif

namespace URP.SegmentedDimmer
{
    public class SegmentedDimmerPass : ScriptableRenderPass
    {
        public Material clearAlphaMaterial { get; set; }
        public int alphaClearMaterialPassIndex { get; set; }
        
#if !MAGIC_LEAP_SEGMENTED_DIMMER_EXPOSE_OVERRIDE_MATERIAL_OPTION
        public Material overrideMaterial { get; set; }
        public int overrideMaterialPassIndex { get; set; }
#endif

        private static readonly int ClearValueShaderProperty = Shader.PropertyToID("_ClearValue");
        private static readonly ShaderTagId ShaderTagId = new ShaderTagId("SRPDefaultUnlit");
        
        private readonly ProfilingSampler m_ProfilingSampler;
        private readonly ProfilingSampler m_ProfilingSamplerClearAlpha;
        private readonly Color    m_ClearColor;
        private readonly RTHandle m_RenderTexture;
#if UNITY_2023_1
        private int m_LayerMask;
#else
        private FilteringSettings m_FilteringSettings;
#endif
        private RenderStateBlock  m_RenderStateBlock;
    
        public SegmentedDimmerPass(string profilerTag, RenderPassEvent renderPassEventArg, RTHandle renderTexture, ColorWriteMask writeMask, int layerMask, float dimmerClearValue)
        {
            base.profilingSampler = new ProfilingSampler(nameof(SegmentedDimmerPass));

            m_ProfilingSampler = new ProfilingSampler(profilerTag);
            m_ProfilingSamplerClearAlpha = new ProfilingSampler("ClearAlpha");
            renderPassEvent = renderPassEventArg;
            clearAlphaMaterial = null;
            alphaClearMaterialPassIndex = 0;
#if !MAGIC_LEAP_SEGMENTED_DIMMER_EXPOSE_OVERRIDE_MATERIAL_OPTION
            overrideMaterial = null;
            overrideMaterialPassIndex = 0;
#endif
            
#if UNITY_2023_1
            m_LayerMask = layerMask;
#else
            m_FilteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask);
#endif
            
            m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Everything);
            m_RenderStateBlock.depthState = new DepthState(false, CompareFunction.Always);
            m_RenderStateBlock.stencilState = new StencilState(false);
            m_RenderStateBlock.rasterState = new RasterState(CullMode.Off, 0, 0f, false);
            
            BlendState blendState = new BlendState();
            blendState.blendState0 = new RenderTargetBlendState(writeMask, BlendMode.One, BlendMode.Zero,
                                                                BlendMode.One, BlendMode.Zero, BlendOp.Add, BlendOp.Add);
            m_RenderStateBlock.blendState = blendState;

            m_ClearColor = new Color(dimmerClearValue, dimmerClearValue, dimmerClearValue, dimmerClearValue);

            m_RenderTexture = renderTexture;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            if (m_RenderTexture != null)
            {
                // Do the render target configs here
                ConfigureTarget(m_RenderTexture);
                ConfigureColorStoreAction(RenderBufferStoreAction.Store);
                ConfigureClear(clearAlphaMaterial == null ? ClearFlag.Color : ClearFlag.None, m_ClearColor);
            }
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                if (clearAlphaMaterial != null)
                {
                    using (new ProfilingScope(cmd, m_ProfilingSamplerClearAlpha))
                    {
                        // Clear alpha
                        clearAlphaMaterial.SetFloat(ClearValueShaderProperty, m_ClearColor.a);
                        cmd.DrawProcedural(Matrix4x4.identity, clearAlphaMaterial, 0, MeshTopology.Triangles, 3, 1);
                    }
                }
                
                // Ensure we flush our command-buffer with the profiler scope and clear call before we render...
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                
#if UNITY_2023_1
                var renderListDesc =
                    new RendererListDesc(ShaderTagId, renderingData.cullResults, renderingData.cameraData.camera)
                    {
                        sortingCriteria = SortingCriteria.CommonOpaque,
                        renderQueueRange = RenderQueueRange.opaque,
                        layerMask = m_LayerMask,
    #if !MAGIC_LEAP_SEGMENTED_DIMMER_EXPOSE_OVERRIDE_MATERIAL_OPTION
                        overrideMaterial = overrideMaterial,
                        overrideMaterialPassIndex = overrideMaterialPassIndex,
    #endif
                    };

                var rendererList = context.CreateRendererList(renderListDesc);
                cmd.DrawRendererList(rendererList);
#else
                // Draw Renderers
                var drawingSettings = CreateDrawingSettings(ShaderTagId, ref renderingData, SortingCriteria.CommonOpaque);
    #if !MAGIC_LEAP_SEGMENTED_DIMMER_EXPOSE_OVERRIDE_MATERIAL_OPTION
                drawingSettings.overrideMaterial = overrideMaterial;
                drawingSettings.overrideMaterialPassIndex = overrideMaterialPassIndex;
    #endif
                drawingSettings.perObjectData = PerObjectData.None;
                
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref m_FilteringSettings, ref m_RenderStateBlock);
#endif
            }
            
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            
            CommandBufferPool.Release(cmd);
        }
    }
}
#endif