Shader "Custom/AdvancedCRT"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScanlineIntensity ("Scanline Intensity", Range(0,1)) = 0.5
        _Curvature ("Screen Curvature", Range(0,1)) = 0.15
        _Aberration ("Chromatic Aberration", Range(0,3)) = 1.0
        _Vignette ("Vignette Strength", Range(0,1)) = 0.4
        _FlickerSpeed ("Flicker Speed", Range(0,20)) = 8.0
        _NoiseStrength ("Noise Strength", Range(0,1)) = 0.1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _ScanlineIntensity;
            float _Curvature;
            float _Aberration;
            float _Vignette;
            float _FlickerSpeed;
            float _NoiseStrength;

            // Random generator for noise
            float rand(float2 co)
            {
                return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
            }

            float4 frag(v2f_img i) : COLOR
            {
                float2 uv = i.uv;

                // Screen curvature (barrel distortion)
                float2 centered = uv - 0.5;
                float r2 = dot(centered, centered);
                uv += centered * r2 * _Curvature;

                // Chromatic aberration (RGB offset)
                float2 offset = float2(0.002 * _Aberration, 0);
                float3 col;
                col.r = tex2D(_MainTex, uv + offset).r;
                col.g = tex2D(_MainTex, uv).g;
                col.b = tex2D(_MainTex, uv - offset).b;

                // Scanlines
                float scan = sin(uv.y * _ScreenParams.y * 1.5);
                col *= 1.0 - (scan * 0.5 + 0.5) * _ScanlineIntensity;

                // Vignette
                float vignette = smoothstep(0.8, 0.2, length(centered) + _Vignette * 0.2);
                col *= vignette;

                // Flicker and Noise (using built-in _Time.y)
                float flicker = sin(_Time.y * _FlickerSpeed) * 0.02;
                float noise = (rand(uv * _Time.y) - 0.5) * _NoiseStrength;
                col += flicker + noise;

                return float4(col, 1.0);
            }
            ENDCG
        }
    }
}
