Shader "SpherePath/SoftTransient"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1, 0.62, 0.08, 0.72)
        _EdgeSoftness("Edge Softness", Range(0.001, 0.5)) = 0.28
        _TailFadeStart("Tail Fade Start", Range(0, 1)) = 0.08
        _TailFadeEnd("Tail Fade End", Range(0, 1)) = 1
        _UseRadialFade("Use Radial Fade", Range(0, 1)) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float _EdgeSoftness;
                float _TailFadeStart;
                float _TailFadeEnd;
                float _UseRadialFade;
            CBUFFER_END

            Varyings Vertex(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            half4 Fragment(Varyings input) : SV_Target
            {
                float stripEdge = smoothstep(0.0, _EdgeSoftness, input.uv.y) * smoothstep(0.0, _EdgeSoftness, 1.0 - input.uv.y);
                float stripTail = 1.0 - smoothstep(_TailFadeStart, _TailFadeEnd, input.uv.x);
                float radialDistance = distance(input.uv, float2(0.5, 0.5)) * 2.0;
                float radialEdge = 1.0 - smoothstep(1.0 - _EdgeSoftness, 1.0, radialDistance);
                float alpha = lerp(stripEdge * stripTail, radialEdge, step(0.5, _UseRadialFade)) * _BaseColor.a;
                return half4(_BaseColor.rgb, alpha);
            }
            ENDHLSL
        }
    }
}
