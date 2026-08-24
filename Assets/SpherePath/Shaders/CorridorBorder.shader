Shader "SpherePath/CorridorBorder"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1, 0, 0, 0.85)
        _FillAlpha("Fill Alpha", Range(0, 1)) = 0.14
        _BorderThickness("Border Thickness", Range(0.001, 0.25)) = 0.015
        _BorderSoftness("Border Softness", Range(0.001, 0.25)) = 0.035
        _PathMinX("Path Min X", Range(0, 0.5)) = 0
        _PathMaxX("Path Max X", Range(0.5, 1)) = 1
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
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float normalY : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float _FillAlpha;
                float _BorderThickness;
                float _BorderSoftness;
                float _PathMinX;
                float _PathMaxX;
            CBUFFER_END

            Varyings Vertex(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.normalY = input.normalOS.y;
                return output;
            }

            half4 Fragment(Varyings input) : SV_Target
            {
                clip(input.normalY - 0.5);
                float lineDistance = min(abs(input.uv.x - _PathMinX), abs(input.uv.x - _PathMaxX));
                float insidePath = step(_PathMinX, input.uv.x) * step(input.uv.x, _PathMaxX);
                half borderAlpha = (half)(1.0 - smoothstep(_BorderThickness, _BorderThickness + _BorderSoftness, lineDistance)) * _BaseColor.a;
                half fillAlpha = (half)insidePath * _BaseColor.a * (half)_FillAlpha;
                half alpha = max(borderAlpha, fillAlpha);
                return half4(_BaseColor.rgb, alpha);
            }
            ENDHLSL
        }
    }
}
