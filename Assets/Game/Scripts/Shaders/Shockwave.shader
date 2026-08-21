Shader "Custom/2DShockwave"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        // Параметры волны (управляются из скрипта)
        _WaveCenter ("Wave Center (UV)", Vector) = (0.5, 0.5, 0, 0)
        _WaveProgress ("Wave Progress", Range(0, 1)) = 0
        
        // Настройки внешнего вида волны
        _WaveForce ("Wave Force", Float) = 0.1
        _WaveWidth ("Wave Width", Float) = 0.05
        _WaveFrequency ("Wave Frequency", Float) = 40.0
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
            };

            fixed4 _Color;
            sampler2D _MainTex;
            
            float4 _WaveCenter;
            float _WaveProgress;
            float _WaveForce;
            float _WaveWidth;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            float _WaveFrequency; 
            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.texcoord;
                
                float dist = distance(uv, _WaveCenter.xy);
                
                if (dist <= _WaveProgress + _WaveWidth && dist >= _WaveProgress - _WaveWidth)
                {
                    float diff = dist - _WaveProgress;
                    float powDiff = 1.0 - abs(diff / _WaveWidth); // 1 в центре волны, 0 на краях
                    
                    float2 dir = normalize(uv - _WaveCenter.xy);
                    
                    float fade = 1.0 - _WaveProgress;
                    
                    float wave = sin((dist - _WaveProgress) * _WaveFrequency) * powDiff;
                     uv += dir * wave * _WaveForce * fade;
                }

                fixed4 c = tex2D(_MainTex, uv) * IN.color;
                c.rgb *= c.a; 
                return c;
            }
            ENDCG
        }
    }
}
