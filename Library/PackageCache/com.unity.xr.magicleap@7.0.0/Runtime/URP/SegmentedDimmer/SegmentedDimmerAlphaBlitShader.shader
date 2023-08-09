Shader "Hidden/MagicLeap/Universal Render Pipeline/SegmentedDimmer/AlphaBlitShader"
{
    SubShader
    {
        Tags { "LightMode"="SRPDefaultUnlit" }
        
        Pass
        {
            Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline"}
            
            Name "SegmentedDimmerAlphaBlit"
            ZTest Always
            ZWrite Off
            Cull Off
            ColorMask A

            HLSLPROGRAM
            #pragma vertex FullscreenVert
            #pragma fragment Fragment
            
            #include "UnityCG.cginc"
            #include "Packages/com.unity.xr.magicleap/Runtime/URP/SegmentedDimmer/FullscreenVertex.hlsl"

            UNITY_DECLARE_TEX2DARRAY(_SourceTex);

            half4 Fragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                return UNITY_SAMPLE_TEX2DARRAY(_SourceTex, float3(input.uv, unity_StereoEyeIndex)).rrrr;
            }
            ENDHLSL
        }
    }
}
