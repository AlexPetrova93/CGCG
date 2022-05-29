Shader "Hidden/NormalsShader"
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

            fixed4 frag(v2f i) : SV_Target
            {
                float4 depthOnly = tex2D(_DepthTex, i.uv);
                float4 normalsOnly = tex2D(_NormalsTex, i.uv);

                fixed4 col = 0;
                //col.rgb = i.uv;

                fixed4 depth = 0;
                depth.rgb = depthOnly;


                return depth;
            }
            ENDCG
        }
    }
}
