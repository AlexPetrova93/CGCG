Shader "Hidden/DepthAndNormalsShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                half3 normal : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;

            fixed4 frag(v2f i) : SV_Target
            {
                float4 depthNormals = tex2D(_CameraDepthTexture, i.vertex);

                fixed4 col = 0; 
                col.rgb = i.normal * .5 + .5;
                col.rgb = depthNormals;
                return col;
            }
            ENDCG
        }
    }
}
