Shader "Hidden/SketchyShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EdgeMap("Edge Map", 2D) = "white" {}
        _NoiseMap("Noise Map", 2D) = "white" {}
        _UncertaintyMatrix("Uncertainty Matrix - a, b, c, d", Vector) = (1, 1, 1, 1)
        _EdgesOn("Display outlines", Int) = 1
        _UncertaintyOn("Apply sketchy effect", Int) = 1
    }

    SubShader
    {
        // No culling, now writing into depth texture
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.vertex;
                return o;
            }

            sampler2D _MainTex;  // flat colors
            sampler2D _EdgeMap;  // precomputed edge map
            sampler2D _NoiseMap; // Perlin noise generated from code
            float4 _UncertaintyMatrix; // user defined uncertainty
            int _EdgesOn;
            int _UncertaintyOn;
                          
            float sampleEdgeMap(float2 uv) 
            {
                return tex2D(_EdgeMap, uv).r;
            }

            float3 sampleColor(float2 uv)
            {
                return tex2D(_MainTex, uv).rgb;
            }

            float sampleNoise(float2 uv) 
            {
                return tex2D(_NoiseMap, uv).r;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float offu = sampleNoise(i.uv) / _ScreenParams.x;
                float offv = sampleNoise(float2(1 - i.uv.r, 1 - i.uv.g)) / _ScreenParams.y;

                float2 newUV = i.uv + 
                    float2(
                        _UncertaintyMatrix.r * offu + _UncertaintyMatrix.g * offv, 
                        _UncertaintyMatrix.b * offu + _UncertaintyMatrix.a * offv
                        );

                float2 finalUV = _UncertaintyOn == 1 ? newUV : i.uv;

                float4 col = tex2D(_MainTex, finalUV);
                float edges = tex2D(_EdgeMap, finalUV).r;

                fixed4 final = _EdgesOn == 1 ? col * edges : col;

                fixed4 debug = 1;
                debug.rgb = sampleEdgeMap(i.uv);

                return final;
            }

            ENDCG
        }
    }
}
