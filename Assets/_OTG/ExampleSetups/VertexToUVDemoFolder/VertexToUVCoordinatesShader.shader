Shader "Unlit/VertexToUVCoordinatesShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _VertexToUVAmount ("Vertex To UV Amount", Range(0,1)) = 0 
        _VertexToUvMultiplier ("Vertex TO UV Multiplier", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull off

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
            float _VertexToUVAmount;
            float _VertexToUvMultiplier;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                float4 uvVertexPos = float4((v.uv.xy - 0.5) * _VertexToUvMultiplier ,0,1);
                v.vertex = lerp(v.vertex, uvVertexPos, _VertexToUVAmount);
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2Dlod(_MainTex, float4(i.uv, 0,0));
                return col;
            }
            ENDCG
        }
    }
}
