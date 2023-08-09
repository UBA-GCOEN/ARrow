#if URP_14_0_0_OR_NEWER
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace URP.SegmentedDimmer
{
    public class SegmentedDimmerAlphaBlit : ScriptableRenderPass
    {
        private readonly ProfilingSampler m_ProfilingSampler;
        private readonly Material m_Material;
        private readonly RTHandle m_RenderTexture;
        private static readonly int SourceTex = Shader.PropertyToID("_SourceTex");
    
        public SegmentedDimmerAlphaBlit(string profilerTag, RenderPassEvent renderPassEvent, RTHandle renderTexture, Material material)
        {
            base.profilingSampler = new ProfilingSampler(nameof(SegmentedDimmerAlphaBlit));

            m_RenderTexture = renderTexture;
            m_ProfilingSampler = new ProfilingSampler(profilerTag);
            this.renderPassEvent = renderPassEvent;
            m_Material = material;
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                // Ensure we flush our command-buffer before we render...
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                
                m_Material.SetTexture(SourceTex, m_RenderTexture);
                cmd.DrawProcedural(Matrix4x4.identity, m_Material, 0, MeshTopology.Triangles, 3, 1);
            }
            
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            
            CommandBufferPool.Release(cmd);
        }
    }
}
#endif