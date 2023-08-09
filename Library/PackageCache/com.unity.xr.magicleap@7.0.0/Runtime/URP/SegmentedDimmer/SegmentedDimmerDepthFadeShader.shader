Shader "Hidden/MagicLeap/Universal Render Pipeline/SegmentedDimmer/DepthFadeMaskShader"
{
    Properties
    {
        _DimmingValue("DisplayOpacity", Range(0.0, 1.0)) = 1
        _FadeStart("FadeStart", Range(0.0, 100.0)) = 5
        _FadeEnd("FadeEnd", Range(0.0, 100.0)) = 12
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
                float3 worldPos : TEXCOORD0;
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
                
                o.worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0)).xyz;
                return o;
            }
            
            CBUFFER_START(UnityPerMaterial)
            float _DimmingValue;
            float _FadeStart;
            float _FadeEnd;
            CBUFFER_END
            fixed4 frag(v2f i) : COLOR
            {
                float distFromCamera = distance(i.worldPos, _WorldSpaceCameraPos);
                float fader = 1.0 - min(1.0, max(0.0, distFromCamera - _FadeStart) / (_FadeEnd - _FadeStart));
                return _DimmingValue.xxxx * fader * fader * fader;
            }
            ENDCG
        }
    }
}
