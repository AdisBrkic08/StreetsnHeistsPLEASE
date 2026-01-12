Shader "Custom/DamageMotionBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurAmount ("Blur", Range(0,1)) = 0.5
        _Distort ("Distort", Range(0,0.1)) = 0.05
        _RedTint ("Red Tint", Color) = (1,0,0,0)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _BlurAmount;
            float _Distort;
            fixed4 _RedTint;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 pos : SV_POSITION; };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 offset = (i.uv - 0.5) * _Distort;

                fixed4 col = tex2D(_MainTex, i.uv);
                col += tex2D(_MainTex, i.uv + offset) * _BlurAmount;
                col += tex2D(_MainTex, i.uv - offset) * _BlurAmount;

                col /= (1 + 2 * _BlurAmount);

                // Add red tint (more visible damage)
                col = lerp(col, _RedTint, _BlurAmount * 0.7);

                return col;
            }
            ENDCG
        }
    }
}
