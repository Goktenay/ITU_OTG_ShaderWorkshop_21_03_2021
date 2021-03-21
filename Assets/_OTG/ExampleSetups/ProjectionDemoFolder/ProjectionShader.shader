Shader "Unlit/ProjectionShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AmbientCol ("Ambient Color", Color) = (1,1,1,1)
        _AmbientAmount ("Ambient Amount", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityLightingCommon.cginc"
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL; 
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
            };

            float4x4 _CamViewProjection;
            float4x4 _CamLocalToWorld;
            float4 _AmbientCol;
            float _AmbientAmount;            

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
               o.vertex = mul(UNITY_MATRIX_M, v.vertex);
               o.normal =  mul( (float3x3) UNITY_MATRIX_M, v.normal);
                             
               o.vertex = mul(_CamViewProjection, o.vertex);
               o.vertex.xyzw /= o.vertex.w;
               //o.vertex.z = 1.1 ;
               o.vertex = mul(_CamLocalToWorld, o.vertex);
               o.vertex = mul(UNITY_MATRIX_VP, o.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 N = normalize(i.normal);
                float3 L = normalize(_WorldSpaceLightPos0.xyz);
                float NDL = saturate(dot(N,L));
                
                
                fixed4 col = tex2D(_MainTex, i.uv);
                return col * NDL + _AmbientCol *_AmbientAmount;
            }
            ENDCG
        }
    }
}
