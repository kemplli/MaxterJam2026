Shader "Custom/2D/SpriteOutlineUniversal"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1,0,0,1)
        _OutlineWidth ("Outline Width", Range(0, 10)) = 1
    }

    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
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

            sampler2D _MainTex;
            float4 _MainTex_TexelSize; 
            fixed4 _Color;
            fixed4 _OutlineColor;
            float _OutlineWidth;

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
                fixed4 mainColor = tex2D(_MainTex, IN.texcoord) * IN.color;
                
                float2 texelOffset = _MainTex_TexelSize.xy * _OutlineWidth;
                
                float alphaUp    = tex2D(_MainTex, IN.texcoord + float2(0, texelOffset.y)).a;
                float alphaDown  = tex2D(_MainTex, IN.texcoord - float2(0, texelOffset.y)).a;
                float alphaLeft  = tex2D(_MainTex, IN.texcoord - float2(texelOffset.x, 0)).a;
                float alphaRight = tex2D(_MainTex, IN.texcoord + float2(texelOffset.x, 0)).a;
                
                float maxAlpha = max(max(alphaUp, alphaDown), max(alphaLeft, alphaRight));
                
                float outlineMask = saturate(maxAlpha - mainColor.a);
                
                fixed4 finalColor = lerp(mainColor, _OutlineColor, outlineMask);
                finalColor.a = max(mainColor.a, outlineMask * _OutlineColor.a);
                
                return finalColor;
            }
            ENDCG
        }
    }
}