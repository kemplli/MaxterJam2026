Shader "UI/PixelWipeTransition"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Wipe Color", Color) = (0,0,0,1)
        _Progress ("Transition Progress", Range(0.0, 1.0)) = 0.0
        
        _PixelScaleX ("Pixel Count X", Float) = 64.0
        _PixelScaleY ("Pixel Count Y", Float) = 36.0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc" // Стандартная библиотека Built-in рендера

            struct Attributes
            {
                float4 vertex   : POSITION;
                float2 uv       : TEXCOORD0;
            };

            struct Varyings
            {
                float4 vertex   : SV_POSITION;
                float2 uv       : TEXCOORD0;
            };

            fixed4 _Color;
            float _Progress;
            float _PixelScaleX;
            float _PixelScaleY;

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.vertex = UnityObjectToClipPos(input.vertex);
                output.uv = input.uv;
                return output;
            }

          fixed4 frag(Varyings input) : SV_Target
{
    float2 pixelUV;
    pixelUV.x = floor(input.uv.x * _PixelScaleX);
    pixelUV.y = floor(input.uv.y * _PixelScaleY);

    float2 localUV = frac(input.uv.xy * float2(_PixelScaleX, _PixelScaleY));

    float ditherPattern = abs(localUV.x - 0.5) + abs(localUV.y - 0.5);

    float globalGradient = pixelUV.x / _PixelScaleX;

    float combined = globalGradient + (ditherPattern * 0.5);

    float alpha = step(combined, _Progress * 1.5);

    return fixed4(_Color.rgb, alpha);
}
            ENDCG
        }
    }
}
