Shader "Unlit/Gold"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    _CardTex ("Card Texture (Sprite)", 2D) = "white" {}
    _PixelDensity ("Pixel Density", Float) = 64.0
    _Active ("Active", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            float _PixelDensity; 
            sampler2D _CardTex;

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float _Active;
            fixed4 frag (v2f i) : SV_Target
            {
float2 uv = floor(i.uv * _PixelDensity) / _PixelDensity;
    float t = _Time.y * 1.5;

   //Плющит
    uv.x += sin(uv.y * 10.0 + t) * 0.01 * _Active;
    uv.y += cos(uv.x * 8.0 - t) * 0.01 * _Active;

    float diagonal = frac((uv.x + uv.y) - t) * 0.1;
    float lineMask = step(diagonal, 0.15);

    float3 rainbow;
    rainbow.r = sin(uv.x * 4.0 + _Time.y) * 0.5 + 0.5;
    rainbow.g = sin(uv.x * 4.0 + _Time.y + 2.0) * 0.5 + 0.5;
    rainbow.b = sin(uv.x * 4.0 + _Time.y + 4.0) * 0.5 + 0.5;
    float3 finalGlint = rainbow * lineMask * _Active;

    fixed4 cardColor = tex2D(_CardTex, uv);

    fixed4 finalColor = cardColor + fixed4(finalGlint * 0.5, 0);

    finalColor.a = cardColor.a;

    return finalColor;
            }
            ENDCG
        }
    }
}
