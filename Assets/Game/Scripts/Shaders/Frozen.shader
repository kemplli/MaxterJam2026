Shader "Custom/FrozenSpriteWithCrystals"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        [Header(Ice Settings)]
        _IceTex ("Ice / Crystal Texture (Noise)", 2D) = "gray" {}
        _IceColor ("Ice Color", Color) = (0.3, 0.7, 1.0, 1.0)
        _FreezeAmount ("Freeze Amount", Range(0,1)) = 0.5
        _CrystalSharpness ("Crystal Sharpness", Range(0.01, 0.5)) = 0.1
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "RenderPipeline"="UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
            };

            Texture2D _MainTex;
            SamplerState sampler_MainTex;

            Texture2D _IceTex;
            SamplerState sampler_IceTex;

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _IceColor;
                float _FreezeAmount;
                float _CrystalSharpness;
                float4 _IceTex_ST;
            CBUFFER_END

            Varyings vert (Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.color = input.color * _Color;
                return output;
            }

            float4 frag (Varyings input) : SV_Target
            {
                float4 texColor = _MainTex.Sample(sampler_MainTex, input.uv) * input.color;

                if (texColor.a < 0.01)
                    discard;

                float2 iceUV = input.uv * _IceTex_ST.xy + _IceTex_ST.zw;
                float4 iceNoise = _IceTex.Sample(sampler_IceTex, iceUV);

                float crystalMask = smoothstep(_FreezeAmount - _CrystalSharpness, _FreezeAmount + _CrystalSharpness, iceNoise.r);
                crystalMask = 1.0 - crystalMask; 

                float3 finalRGB = lerp(texColor.rgb, texColor.rgb * _IceColor.rgb + iceNoise.rgb * 0.4, crystalMask);

                float edge = smoothstep(0.4, 0.5, crystalMask * (1.0 - crystalMask));
                finalRGB += _IceColor.rgb * edge * _FreezeAmount;

                return float4(finalRGB, texColor.a);
            }
            ENDHLSL
        }
    }
}
