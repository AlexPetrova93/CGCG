Shader "Hidden/EdgeDetectionShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ThresholdDepth("Depth Edge Threshold", Float) = 0.01
        _ThresholdNormal("Normal Edge Threshold", Float) = 0.01
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

            sampler2D _MainTex;  // the screen (original img)
            sampler2D _CameraDepthTexture;  // camera depth
            sampler2D _CameraDepthNormalsTexture;  // camera depth + normals

            float _ThresholdDepth;
            float _ThresholdNormal;

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

            float4 getDepthNormal(float2 uv)
            {
                half3 normal;
                float depth;
                DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, uv), depth, normal);

                return float4(normal, depth);
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
                //float Inr = getDiscontinuity(A.r, C.r, F.r, H.r);
                //float Ing = getDiscontinuity(A.g, C.g, F.g, H.g);


                // depth
                float a = tex2D(_CameraDepthNormalsTexture, idA).b;
                float c = tex2D(_CameraDepthNormalsTexture, idC).b;
                float f = tex2D(_CameraDepthNormalsTexture, idF).b;
                float h = tex2D(_CameraDepthNormalsTexture, idH).b;

                // discontinutity in depth
                float Iz = getDiscontinuity(a, c, f, h);


                return In * Iz;
            }

            fixed4 frag(v2f i) : SV_Target
            {

                float4 depthNormals = tex2D(_CameraDepthNormalsTexture, i.uv);
                fixed4 debug = 0;
                debug.rgb = depthNormals.rgb;

                fixed4 edges = 0;
                //edges.rgb = edgeDetection(i.uv);

                float2 texelSize = float2(1 / _ScreenParams.x, 1 / _ScreenParams.y);

                float3x3 SobelX = {
                    -1, 0, 1,
                    -2, 0, 2,
                    -1, 0, 1
                };

                float3x3 SobelY = {
                    -1, -2, -1,
                     0,  0,  0,
                     1,  2,  1
                };

                float2 offsets[9] = {
                    float2(-1, 1),
                    float2(0, 1),
                    float2(1, 1),

                    float2(-1, 0),
                    float2(0, 0),
                    float2(1, 0),

                    float2(-1, -1),
                    float2(0, -1),
                    float2(1, -1),
                };

                float samplex = 0;
                float sampley = 0;
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        float2 idx = i.uv + texelSize * float2(j - 1, k - 1);
                        samplex += SobelX[j][k] * getDepthNormal(idx).a;
                        sampley += SobelY[j][k] * getDepthNormal(idx).a;
                    }
                }
                float mag = sqrt(samplex * samplex + sampley * sampley);

                edges.rgb = mag > _ThresholdDepth ? 0 : 1;

                return edges;
            }

            ENDCG
        }
    }
}
