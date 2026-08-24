Shader "Custom/PixelatedParticles"
{
    Properties
    {
        _PixelCount ("Resolution (Pixels)", Float) = 32.0
        _Radius ("Circle Radius", Range(0.0, 0.5)) = 0.4
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off 
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            float _PixelCount;
            float _Radius;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color; 
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 pixelUV = floor(i.uv * _PixelCount) + 0.5;
                
                float2 center = _PixelCount * 0.5;
                
                float dist = distance(pixelUV, center) / _PixelCount;
                
                float circleAlpha = step(dist, _Radius);
                
                fixed4 col = i.color;
                col.a *= circleAlpha;
                
                clip(col.a - 0.01);
                
                return col;
            }
            ENDCG
        }
    }
}