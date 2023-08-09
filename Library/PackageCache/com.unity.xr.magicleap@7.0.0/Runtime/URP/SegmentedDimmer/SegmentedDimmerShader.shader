Shader "Hidden/MagicLeap/Universal Render Pipeline/SegmentedDimmer/MaskShader"
{
    Properties
    {
        _DimmingValue("DisplayOpacity", Range(0.0, 1.0)) = 1
    }

    SubShader
    {
        Tags { "LightMode"="SRPDefaultUnlit" }
                
        Pass
        {
            Name "SegmentedDimmerShader"
            Tags { "RenderType"="Opaque" "Queue"="Background" "RenderPipeline"="UniversalPipeline" "ForceNoShadowCasting"="True" "IgnoreProjector" = "True" "CanUseSpriteAtlas" = "False" }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            struct appdata
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }
            
            CBUFFER_START(UnityPerMaterial)
            float _DimmingValue;
            CBUFFER_END
            fixed4 frag() : COLOR
            {
                return _DimmingValue.xxxx;
            }
            ENDCG
        }
    }
}
