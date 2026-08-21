Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Speed ("Wave Speed", Float) = 0.5
    _PixelDensity ("Pixel Density", Float) = 64.0
    _CardTex ("Card Texture (Sprite)", 2D) = "white" {}
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

            sampler2D _CardTex;
            float _Speed;        
float _PixelDensity; 

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

            fixed4 frag (v2f i) : SV_Target
            {
    float2 uv = floor(i.uv * _PixelDensity) / _PixelDensity;
    float t = _Time.y * _Speed;

    uv.x += sin(uv.y * 10.0 + t) * 0.04;
    uv.y += cos(uv.x * 8.0 - t) * 0.04;

    fixed4 cardColor = tex2D(_CardTex, uv);

    float wave1 = sin(uv.x * 12.0 + t);
    float wave2 = cos(uv.y * 10.0 - t);
    float plasma = frac((wave1 + wave2) * 2.0);
    plasma = smoothstep(0.15, 0.35, plasma);

    fixed4 plasmaColor = fixed4(0, plasma, plasma * 0.5, 1);
    
    fixed4 finalColor = cardColor + (plasmaColor * 0.3);

    finalColor.a = cardColor.a;

    return finalColor;
            }
            ENDCG
        }
    }
}
