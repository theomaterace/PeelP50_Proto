Shader "Przemek/InsidePanoramaURP"
{
    Properties
    {
        _MainTex ("Equirectangular 360", 2D) = "white" {}
        _Tint    ("Tint", Color) = (1,1,1,1)
        _Exposure("Exposure", Float) = 1.0
        _Tiling  ("Tiling (X,Y)", Vector) = (1,1,0,0)
        _Offset  ("Offset (X,Y)", Vector) = (0,0,0,0)
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100

        // Patrzymy od œrodka kuli, wiêc renderujemy TYLNE œciany (fronty odcinamy)
        Cull Front
        ZWrite Off
        ZTest LEqual
        Blend One Zero

        Pass
        {
            Name "Unlit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            // URP core
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Tekstura
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;       // Unity automatycznie podaje (tiling, offset) z materia³u, ale u¿yjemy w³asnych suwaków te¿
                float4 _Tint;
                float  _Exposure;
                float4 _Tiling;           // xy u¿ywane
                float4 _Offset;           // xy u¿ywane
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                // standardowy transform URP
                VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = posInputs.positionCS;

                // UV z mesha + rêczny tiling/offset
                float2 uv = IN.uv * _Tiling.xy + _Offset.xy;
                OUT.uv = uv;
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // sample z Repeat (ustaw w imporcie tekstury)
                float2 uv = IN.uv;
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

                col.rgb *= _Tint.rgb * _Exposure;
                col.a   *= _Tint.a;
                return col;
            }
            ENDHLSL
        }
    }

    FallBack Off
}
