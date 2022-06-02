Shader "Hidden/EdgeShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DepthTex("Depth Texture", 2D) = "white" {}
        _NormalsTex("Normals Texture", 2D) = "white" {}
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
            sampler2D _DepthTex;
            sampler2D _NormalsTex;

            float3x3 fSobelX = {
                -1, 0, 1,
                -2, 0, 2, 
                -1, 0, 1
            };

            float3x3 fSobelY = {
                -1, -2, -1,
                 0,  0,  0,
                 1,  2,  1
            };

            float3x3 dummy = {
                -1, 0, 1,
                -1, 0, 1,
                -1, 0, 1
            };


            float SobelLine(float factor, sampler2D tex, float2 uv) 
            {
                return factor * tex2D(tex, uv).x;
            }

            // expects a grayscale texture
            float4 Sobel(float3x3 sobel, sampler2D tex, float2 uv)
            {
                float4 ret = 0;
                                        
                float incX = 1 / _ScreenParams.x;
                float incY = 1 / _ScreenParams.y;

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        float2 idx = float2(uv.x + x * incX, uv.y + y * incY);

                        ret += sobel[x+1][y+1] * tex2D(tex, idx);
                    }
                }
                /*
                float2 idx = uv;
                idx.y += incY;
                idx.x -= incX;

                ret += SobelLine(-1, tex, idx); 

                idx.x += incX;

                ret += SobelLine(0, tex, idx);

                idx.x += incX;

                ret += SobelLine(1, tex, idx);

                // --------

                idx = uv;
                idx.x -= incX;

                ret += SobelLine(-2, tex, idx);

                idx.x += incX;

                ret += SobelLine(0, tex, idx);

                idx.x += incX;

                ret += SobelLine(2, tex, idx);


                // --------

                idx = uv;
                idx.y -= incY;
                idx.x -= incX;

                ret += SobelLine(-1, tex, idx);

                idx.x += incX;

                ret += SobelLine(0, tex, idx);

                idx.x += incX;

                ret += SobelLine(1, tex, idx);
                */

                return ret;
            }

            float diagonalSamples(sampler2D tex, float2 uv, int component)
            {
                // index increment
                float incX = 1 / _ScreenParams.x;
                float incY = 1 / _ScreenParams.y;

                // indexes
                float2 idxA = float2(uv.x - incX, uv.y + incY);
                float2 idxH = float2(uv.x + incX, uv.y - incY);

                float2 idxC = float2(uv.x + incX, uv.y + incY);
                float2 idxF = float2(uv.x - incX, uv.y - incY);

                // texture access
                float A = tex2D(tex, idxA)[component];
                float H = tex2D(tex, idxH)[component];

                float C = tex2D(tex, idxC)[component];
                float F = tex2D(tex, idxF)[component];

                // compute discontinuities
                return (1 - 0.5f * abs(A - H)) * (1 - 0.5f * abs(A - H))
                    * (1 - 0.5f * abs(C - F)) * (1 - 0.5f * abs(C - F));
            }

            float edgeDetection(float2 uv) 
            {
                // index increment
                float incX = 1 / _ScreenParams.x;
                float incY = 1 / _ScreenParams.y;

                // indexes
                float2 idxA = float2(uv.x - incX, uv.y + incY);
                float2 idxH = float2(uv.x + incX, uv.y - incY);

                float2 idxC = float2(uv.x + incX, uv.y + incY);
                float2 idxF = float2(uv.x - incX, uv.y - incY);

                // normals
                float2 normA = tex2D(_NormalsTex, idxA).rg;
                float2 normH = tex2D(_NormalsTex, idxH).rg;

                float2 normC = tex2D(_NormalsTex, idxC).rg;
                float2 normF = tex2D(_NormalsTex, idxF).rg;

                // discontinutity in normals
                float In = 0.5f * (dot(normA, normH) + dot(normC, normF));

                // depth
                float depthA = tex2D(_DepthTex, idxA).r;
                float depthH = tex2D(_DepthTex, idxH).r;

                float depthC = tex2D(_DepthTex, idxC).r;
                float depthF = tex2D(_DepthTex, idxF).r;

                // discontinutity in depth
                float Iz = diagonalSamples(_DepthTex, uv, 0);

                // normals
                float Inr = diagonalSamples(_NormalsTex, uv, 0);
                float Ing = diagonalSamples(_NormalsTex, uv, 1);

                return Inr * Ing;

            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 depthOnly = tex2D(_DepthTex, i.uv);
                float4 normalsOnly = tex2D(_NormalsTex, i.uv);

                fixed4 edges = 0;

                float4 sx = Sobel(fSobelX, _DepthTex, i.uv);
                float4 sy = Sobel(fSobelY, _DepthTex, i.uv);

                edges.rgb = edgeDetection(i.uv);

                float4 col = tex2D(_MainTex, i.uv);

                return col * edges;
            }

            ENDCG
        }
    }
}
