Shader "Custom/SimpleEpicIBLShader"
{
    Properties
    {
      _MainTex ("Albedo Map", 2D) = "white" {}
      _NormalMap ("Normal Map", 2D) = "bump" {}
      _SkyMap ("Sky Map", 2D) = "white" {}
      
      
      _AmbientLight ("Ambient Light", Color) = (1,1,1,1)
      _AmbientAmount ("Ambient Amount", Range(0,1)) = 0
      _DiffuseIBLMipLevel ("Diffuse IBL Mip Level", Range(0,10)) = 0
      
      _NormalMapAmount ("Normal Map Amount", Range(0,1)) = 1
      _GlossAmount ("Gloss Amount", Range(0,1)) = 1
      _GlossIBLAmount ("Gloss IBL Amount", Range(0,1)) = 0
      _GlossFresnelMultiplier ("Gloss Fresnel Multiplier", Range(0,10)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            struct MeshData
            {
                float4 objectPos : POSITION;
                float2 vertUV : TEXCOORD0; 
                float3 objectNormal : NORMAL;
                float4 objectTangent : TANGENT;
            };

            struct Interpolator
            {
                float4 clipPos : SV_POSITION;
                float2 fragUV : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 worldTangent : TEXCOORD3;
                float3 worldBitangent : TEXCOORD4;
            };

            Interpolator vert (MeshData i)
            {
                Interpolator o;
                o.clipPos = UnityObjectToClipPos(i.objectPos); 
                o.fragUV = i.vertUV;
                o.worldPos = mul(unity_ObjectToWorld, float4(i.objectPos.xyz, 1));
                o.worldNormal = UnityObjectToWorldNormal(i.objectNormal); // mul(unity_ObjectToWorld, float4(i.objectNormal, 0));
                o.worldTangent = UnityObjectToWorldDir(i.objectTangent.xyz); // mul(unity_ObjectToWorld, float4(i.objectTangent, 0));
                o.worldBitangent = cross(o.worldNormal, o.worldTangent) * i.objectTangent.w * unity_WorldTransformParams.w;
                o.worldPos = mul(unity_ObjectToWorld, i.objectPos);                

                return o;
            }
            
            sampler2D _NormalMap;
            sampler2D _MainTex;
            sampler2D _SkyMap;
            
            
            float4 _AmbientLight;
            float _AmbientAmount, _DiffuseIBLMipLevel;
            float _NormalMapAmount;
            float _GlossAmount, _GlossIBLAmount, _GlossFresnelMultiplier;

            #define PI 3.141592653589793238
            float2 DirectionToUV(float3 dir)
            {
                float v = (dir.y * 0.5) + 0.5;
                float u = atan2(dir.z, dir.x) / (PI * 2) + 0.5 ; 
                return float2(u,v);
            }
            

            float4 frag (Interpolator i) : SV_Target
            {
               float3 fragNormal = normalize(i.worldNormal);
               float3 tangent = normalize(i.worldTangent);
               float3 bitangent = normalize(i.worldBitangent);
                              
               float3x3 tangentToWorldMatrix = {
                tangent.x, bitangent.x, fragNormal.x,
                tangent.y, bitangent.y, fragNormal.y,
                tangent.z, bitangent.z, fragNormal.z
               };
               
               float3 normalMapVal = UnpackNormal(tex2D(_NormalMap, i.fragUV));
               float3 tangentSpaceNorm = normalize(lerp(float3(0,0,1), normalMapVal, _NormalMapAmount));
               
               float3 N = mul(tangentToWorldMatrix, tangentSpaceNorm);
               float3 V = normalize(_WorldSpaceCameraPos - i.worldPos);
               float3 L = normalize(_WorldSpaceLightPos0);
               
               float3 H = normalize(L + V);
               float3 lightColor = _LightColor0;
               
               float3 lambert = saturate(dot(N,L));
               
               
               // Diffuse IBL
               float2 DiffuseIBLDirToUV = DirectionToUV(N);
               float3 diffuseIBL = tex2Dlod(_SkyMap, float4(DiffuseIBLDirToUV.xy, _DiffuseIBLMipLevel, _DiffuseIBLMipLevel)).rgb;
               // Diffuse IBL
                              
               float3 ambient = _AmbientLight * _AmbientAmount;
               float3 diffuseLight = lambert * lightColor + ambient + diffuseIBL;
                          
               // Specular  
               float3 specularLight = saturate(dot(H, N)) * (lambert > 0);
               float specularExponent = exp2(12 * _GlossAmount) + 1; 
               specularLight = pow(specularLight, specularExponent) * _GlossAmount ;
               specularLight *= lightColor;
               // Specular
               
               // Specular IBL
               float3 fresnel = pow((1-saturate(dot(V, N))), _GlossFresnelMultiplier);
               float3 viewReflection = reflect(-V, N);
               float2 specularIBLDirToUV = DirectionToUV(viewReflection);
               float specularMip = (1-_GlossAmount) * 6;
               float3 specularIBL = tex2Dlod(_SkyMap, float4(specularIBLDirToUV.xy, specularMip, specularMip)).rgb;
               specularLight += specularIBL * _GlossIBLAmount * fresnel;
               // Specular IBL
               
               
               float3 surfaceColor = tex2D(_MainTex, i.fragUV).rgb;
               
               float3 finalColor = (surfaceColor * diffuseLight + specularLight).xyz;
               return float4(finalColor.xyz, 1);
               
            }
            ENDCG
        }
    }
}
