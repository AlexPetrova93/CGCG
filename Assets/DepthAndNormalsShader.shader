Shader "Hidden/DepthAndNormalsShader"
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

            sampler2D _MainTex;  // the screen (original img)
            sampler2D _CameraDepthTexture;  // camera depth
            sampler2D _CameraDepthNormalsTexture;  // camera depth + normals

            fixed4 frag(v2f i) : SV_Target
            {
                float4 screen = tex2D(_MainTex, i.uv);
                float4 depthOnly = tex2D(_CameraDepthTexture, i.uv);
                float4 depthNormals = tex2D(_CameraDepthNormalsTexture, i.uv);

                fixed4 col = 0;
                //col.rgb = i.uv;
                //col.rgb = screen;

                fixed4 depth = 0;
                depth.rgb = depthOnly.rrr;

                fixed4 normals = 0;
                normals.rg = depthNormals.rg;



                return normals;
            }
            ENDCG
        }
    }
}
