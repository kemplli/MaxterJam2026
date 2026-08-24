Shader "Custom/PixelateFullscreen"
{
    Properties
    {
        _Columns ("Columns", Float) = 320
        _Rows ("Rows", Float) = 180
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZTest Always ZWrite Off Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // “екстура экрана в URP
            Texture2D _BlitTexture;

            float _Columns;
            float _Rows;

            struct CustomAttributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct CustomVaryings
            {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;
            };

            CustomVaryings vert(CustomAttributes input)
            {
                CustomVaryings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            half4 frag(CustomVaryings input) : SV_Target
            {
                float2 uv = input.uv;
                
                // Ёффект пикселизации через округление сетки
                uv.x = floor(uv.x * _Columns) / _Columns;
                uv.y = floor(uv.y * _Rows) / _Rows;

                // „итаем цвет пиксел€ из буфера экрана, использу€ встроенный sampler_LinearClamp
                half4 color = _BlitTexture.Sample(sampler_LinearClamp, uv);
                return color;
            }
            ENDHLSL
        }
    }
}
