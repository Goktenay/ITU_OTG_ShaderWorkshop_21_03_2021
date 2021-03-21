Shader "Unlit/RawBlockUnlitShader"
{
    Properties
    {
        _MainTex ("Albedo Texture", 2D) = "white" {}
        [NoScaleOffset] _NormalTex ("Normal Tex", 2D) = "bump" {}
        _NormalAmount ("Normal Amount", Range(0,1)) = 1
  
        [NoScaleOffset] _SkyTex ("Sky Tex", 2D) = "white" {}
        [NoScaleOffset] _RenderTex ("Mask Render Texture", 2D) = "white" {}
    
        [Header(Settings)]
        _AmbientCol ("Ambient Color", Color) = (1,1,1,1)
        _AmbientAmount ("Ambient Amount", Range(0,1)) = 0.1 
        [Space(10)]
        _GlossCol ("Gloss Color", Color) = (1,1,1,1)
        _GlossAmount ("Gloss Amount", Range(0,1)) = 0.1 
        [Space(10)]
        _ReflectionMipLevel ("Reflection Mip Level", Range(0,10)) = 0
        _ReflectionAmount ("Reflection Amount", Range(0,1)) = 0
        [Space(10)]
        _FresnelExp ("Fresnel Exponent", Range(0,10)) = 1
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
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float3 tangent : TEXCOORD3;
                float3 bitangent : TEXCOORD4;
                float2 albedoUV : TEXCOORD5;
            };

            sampler2D _SkyTex, _RenderTex;
            sampler2D _MainTex, _NormalTex;
            float4 _MainTex_ST;
            float _NormalAmount;
            
            float4 _AmbientCol, _GlossCol;
            float _AmbientAmount, _GlossAmount;
            float _ReflectionMipLevel, _ReflectionAmount;
            float _FresnelExp;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.uv = v.uv;
                o.albedoUV = TRANSFORM_TEX(v.uv, _MainTex);
                
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.normal = normalize(mul(unity_ObjectToWorld, v.normal));
                o.tangent = normalize(mul(unity_ObjectToWorld, v.tangent.xyz)); 
                o.bitangent = cross(o.normal, o.tangent);
                o.bitangent *=  v.tangent.w * unity_WorldTransformParams.w; 
                
        
                o.vertex = UnityObjectToClipPos(v.vertex);
       
                return o;
            }
            
            
            #define TAU 6.28318530718
            float2 DirectionToUv(float3 dir)
            {
                float u = atan2(dir.z, dir.x) / TAU;
                float v = (dir.y + 1) * 0.5;
                return float2(u,v);
            }

            float4 frag (v2f i) : SV_Target
            { 
                float3 tangentSpaceNormal = UnpackNormal(tex2Dlod(_NormalTex, float4(i.albedoUV,0,0)));
                float3 tangetNorm = normalize(i.tangent.xyz);
                float3 bitangentNorm = normalize(i.bitangent.xyz);
                float3 normalNorm = normalize(i.normal.xyz);
                
                float3x3 mtxTangToWorld = {
                    tangetNorm.x, bitangentNorm.x, normalNorm.x,
                    tangetNorm.y, bitangentNorm.y, normalNorm.y,
                    tangetNorm.z, bitangentNorm.z, normalNorm.z,
                };
                
                float3 worldSpaceNorm = lerp(i.normal, mul(mtxTangToWorld, tangentSpaceNormal), _NormalAmount);
                float3 N = normalize(worldSpaceNorm);
                float3 V = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
                float3 L = normalize(_WorldSpaceLightPos0);//normalize(_WorldSpaceLightPos0.xyz - i.worldPos.xyz);  
                                
                float3 refVal = reflect(-V, N);
               
                
                float4 reflectSample = tex2Dlod(_SkyTex, float4(DirectionToUv(refVal), _ReflectionMipLevel,_ReflectionMipLevel));
                
                float3 lambert = saturate(dot(N, L));
                float3 diffuseLight = lambert * _LightColor0 ;
                float3 ambientLight = _AmbientAmount *  reflectSample;
               
                float3 H = normalize(L + V);
		        float3 specularLight = saturate( dot(N, H) ) * (lambert > 0);
		        
                float fresnel =pow(1- saturate(dot(N, V)), _FresnelExp);
		        float specularExponent = exp2( (1-_GlossAmount) * 10) + 1 ;
		        specularLight = pow(specularLight, specularExponent) * _GlossCol * _LightColor0  ; 
                specularLight += reflectSample.xyz * _ReflectionAmount * fresnel;
                

                float4 col = tex2D(_MainTex, i.albedoUV);               
                float3 finalCol =  col.xyz * diffuseLight  + specularLight  + ambientLight;
              
                
                
                return float4(finalCol, 1);
            }
            ENDCG
        }
    }
}
