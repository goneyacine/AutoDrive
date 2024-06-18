Shader "Custom/DepthShader"
{
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float depth : TEXCOORD0;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.depth = o.pos.z / o.pos.w;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                return half4(i.depth, i.depth, i.depth, 1.0);
            }
            ENDCG
        }
    }
}