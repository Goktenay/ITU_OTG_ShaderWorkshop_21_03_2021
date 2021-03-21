Shader "Unlit/DirectionToUVCoordinatesShader"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        _WrapAmount ("Wrap Amount", Range(0,1)) = 0
        _YWrapAmount ("Y Wrap Amount", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        cull off
   
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _WrapAmount, _YWrapAmount;
            
            
            #define TAU 6.28318530718
            float2 DirectionToUv(float3 dir)
            {
                float u = atan2(dir.z, dir.x) / TAU;
                float v = (dir.y + 1) * 0.5;
                return float2(u,v);
            }
            
            float3 UVToDirection(float2 uv)
            {
                float y = (uv.y - 0.5) * 2;
                float z = sin(uv.x * TAU);
                float x = cos(uv.x * TAU);
         
                
                float oneMinusY = 1-abs(lerp(0, y, _YWrapAmount));
                float2 xz = normalize(float2(x,z)) * oneMinusY;
                float3 finVal = float3(xz.x, y,xz. y);
                
                return lerp(finVal, normalize(finVal), _YWrapAmount);
            }


            v2f vert (appdata v)
            {
                v2f o;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
                float3 worldPosWrapped = UVToDirection(v.uv);
                float3 finalWorldPos = lerp(worldPos, worldPosWrapped, _WrapAmount);
                v.vertex = mul(unity_WorldToObject, float4(finalWorldPos.xyz,1));
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2Dlod(_MainTex, float4(i.uv,0,0));
                return col;
            }
            ENDCG
        }
    }
}
