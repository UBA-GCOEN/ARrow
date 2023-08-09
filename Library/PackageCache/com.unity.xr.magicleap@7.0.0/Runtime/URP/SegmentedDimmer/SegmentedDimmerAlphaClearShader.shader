Shader "Hidden/MagicLeap/Universal Render Pipeline/SegmentedDimmer/AlphaClearShader"
{
    Properties
    {
        _ClearValue("ClearValue", Range(0.0, 1.0)) = 0.0
    }
    
    SubShader
    {        
        Tags { "LightMode"="SRPDefaultUnlit" }
        Pass
        {
            Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline"}
            
            Name "SegmentedDimmerAlphaClearShader"
            ZTest Always
            ZWrite Off
            Cull Off
            ColorMask A

            HLSLPROGRAM
            #pragma vertex FullscreenVert
            #pragma fragment Fragment

            #include "UnityCG.cginc"
            #include "Packages/com.unity.xr.magicleap/Runtime/URP/SegmentedDimmer/FullscreenVertex.hlsl"

            CBUFFER_START(UnityPerMaterial)
            float _ClearValue;
            CBUFFER_END

            half4 Fragment(Varyings input) : SV_Target
            {
                return _ClearValue.xxxx;
            }
            ENDHLSL
        }
    }
}
