Shader "Custom/2DJuicyPull"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        // Передаются из скрипта в UV координатах (от 0 до 1)
        _GrabPoint ("Grab Point (UV)", Vector) = (0.5, 0.5, 0, 0)
        _MousePoint ("Mouse Point (UV)", Vector) = (0.5, 0.5, 0, 0)
        
        _PullRadius ("Pull Radius", Float) = 0.15 // Радиус захвата пикселей
        _PullStrength ("Pull Strength", Float) = 1.0
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

        Cull Off Lighting Off ZWrite Off
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
                float2 texcoord : TEXCOORD0;
            };

            fixed4 _Color;
            sampler2D _MainTex;
            
            float4 _GrabPoint;
            float4 _MousePoint;
            float _PullRadius;
            float _PullStrength;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.texcoord;
                
                // Вектор вытягивания (от точки захвата до текущей позиции мыши)
                float2 pullDir = _MousePoint.xy - _GrabPoint.xy;
                float pullDist = length(pullDir);
                
                if (pullDist > 0.001)
                {
                    // Расстояние от текущего пикселя до линии вытягивания
                    float2 v = uv - _GrabPoint.xy;
                    float2 nPullDir = normalize(pullDir);
                    
                    // Проекция пикселя на вектор вытягивания
                    float t = dot(v, nPullDir);
                    t = clamp(t, 0.0, pullDist);
                    
                    // Ближайшая точка на отрезке вытягивания
                    float2 closestPoint = _GrabPoint.xy + nPullDir * t;
                    
                    // Расстояние от текущего UV до "линии вытягивания"
                    float distToLine = distance(uv, closestPoint);
                    
                    // Если пиксель находится в радиусе действия линии
                    if (distToLine < _PullRadius)
                    {
                        // Вычисляем мягкий спад эффекта к краям линии
                        float influence = 1.0 - (distToLine / _PullRadius);
                        influence = smoothstep(0.0, 1.0, influence);
                        
                        // Смещаем UV координаты обратно вдоль линии,
                        // чтобы "притянуть" пиксели из точки захвата в точку мыши
                        float progressAlongLine = t / pullDist;
                        uv -= pullDir * influence * _PullStrength * (1.0 - progressAlongLine);
                    }
                }

                // Отрисовка искаженной текстуры
                fixed4 c = tex2D(_MainTex, uv) * IN.color;
                c.rgb *= c.a; 
                return c;
            }
            ENDCG
        }
    }
}
