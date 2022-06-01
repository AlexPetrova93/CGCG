Shader "Hidden/Edge2Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            sampler2D _MainTex;  

            float2 offsets[4] = {
                    float2(-1, 1),  // A
                    float2(1, 1),   // C
                    float2(-1, -1), // F
                    float2(1, -1)   // H
            };

            float3 expand3(float3 color) 
            {
                float3 ret = 0;
                ret.x = 2 * color.x - 1;
                ret.y = 2 * color.y - 1;
                ret.z = 2 * color.z - 1;

                return ret;
            }

            float2 expand2(float2 color)
            {
                float2 ret = 0;
                ret.x = 2 * color.x - 1;
                ret.y = 2 * color.y - 1;

                return ret;
            }

            float pow2(float x)
            {
                return x * x;
            }

            float getDiscontinuity(float a, float c, float f, float h) 
            {
                return pow2(1 - 0.5f * abs(a - h)) * pow2(1 - 0.5f * abs(c - f));
            }

            float edgeDetection(float2 uv)
            {
                // index increment
                float2 texelSize = float2(1 / _ScreenParams.x, 1 / _ScreenParams.y);

                // indexes - A, C, F, H
                float2 idA = uv + float2(-1, 1) * texelSize;  // A
                float2 idC = uv + float2(1, 1) * texelSize;   // C
                float2 idF = uv + float2(-1, -1) * texelSize; // F
                float2 idH = uv + float2(1, -1) * texelSize;

                // normals 
                float3 A = tex2D(_MainTex, idA).rgb;
                float3 C = tex2D(_MainTex, idC).rgb;
                float3 F = tex2D(_MainTex, idF).rgb;
                float3 H = tex2D(_MainTex, idH).rgb;

                // discontinutity in normals
                float In = 0.5f * ( dot(expand3(A), expand3(H)) + dot(expand3(C), expand3(F)) );
                float Inr = getDiscontinuity(A.r, C.r, F.r, H.r);
                float Ing = getDiscontinuity(A.g, C.g, F.g, H.g);
                float Inb = getDiscontinuity(A.b, C.b, F.b, H.b);


                // depth
                float a = tex2D(_MainTex, idA).a;
                float c = tex2D(_MainTex, idC).a;
                float f = tex2D(_MainTex, idF).a;
                float h = tex2D(_MainTex, idH).a;

                // discontinutity in depth
                float Iz = getDiscontinuity(a, c, f, h);


                return In * Iz;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 depthNormals = tex2D(_MainTex, i.uv);

                fixed4 debug = 0;
                debug.rgb = depthNormals.rgb;

                fixed4 edges = 0;
                edges.rgb = edgeDetection(i.uv);

                return edges;
            }

            ENDCG
        }
    }
}
