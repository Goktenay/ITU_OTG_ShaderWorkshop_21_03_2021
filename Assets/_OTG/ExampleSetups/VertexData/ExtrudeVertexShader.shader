Shader "Unlit/ExtrudeVertexShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Gloss ("Gloss Val", Range(0,1)) = 0
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
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

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
                float3 worldPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _ExtrudeAmount;
            float _Gloss;
            v2f vert (appdata v)
            {
                v2f o;
                o.worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz,1));
                o.normal = mul((float3x3)unity_ObjectToWorld, v.normal);
                float3 worldSpace = mul(unity_ObjectToWorld, v.normal);
                worldSpace = normalize(worldSpace) * _ExtrudeAmount ;
                float3 objectSpace = mul(unity_WorldToObject, worldSpace);
                o.vertex = UnityObjectToClipPos(v.vertex + objectSpace);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
   
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 L = normalize(_WorldSpaceLightPos0.xyz);
                float3 N = normalize(i.normal.xyz);
                float nDl = dot(L , N);
               // return pow(saturate(nDl),10);
                float3 V = normalize(_WorldSpaceCameraPos - i.worldPos.xyz);
                float3 H = normalize(L + V);
                float spec =  saturate(dot(H, N));
                float glossExponent = exp2(12 * _Gloss) +  1;
                float specularVal = pow(spec, glossExponent); 
                
                
                fixed4 col = tex2D(_MainTex, i.uv);
                return float4(1,1,1,1) * saturate(nDl) + specularVal * (nDl > 0) * 0.1 + float4(1,1,1,0) * 0.1;
            }
            ENDCG
        }
    }
}
