Shader "Custom/SimpleShader"
{
    Properties
    {
       
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

            struct MeshData
            {
                float4 objectPos : POSITION;
            };

            struct Interpolator
            {
                float4 clipPos : SV_POSITION;
            };

            Interpolator vert (MeshData i)
            {
                Interpolator o;
                o.clipPos = UnityObjectToClipPos(i.objectPos); 
                return o;
            }

            float4 frag (Interpolator i) : SV_Target
            {
                return float4(1,1,1,1);
            }
            ENDCG
        }
    }
}
