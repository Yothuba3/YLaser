          Shader "YLazer"
{
    Properties
    {
        [HDR]_Color("Main Color", Color) =(1,1,1,1)
    }
    SubShader
    {
        Cull Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #define UNITY_INDIRECT_DRAW_ARGS IndirectDrawIndexedArgs
            #include "UnityIndirect.cginc"
            

            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR0;
            };

            float random(fixed2 st)
            {
	            return frac(sin(dot(st.xy, fixed2(12.9898, 78.233))) * 43758.5453);
            }

            float2 rotation(float2 p, float theta)
{
	return float2((p.x) * cos(theta) - p.y * sin(theta), p.x * sin(theta) +  p.y * cos(theta));
}

            CBUFFER_START(UnityPerMaterial);
            uniform float4x4 _ObjectToWorld;
            uniform float _lazerLength;
            uniform float width;
            uniform float4 _Color;
            CBUFFER_END;
            v2f vert(appdata_full v, uint svInstanceID : SV_InstanceID)
            {
                InitIndirectDrawArgs(0);
                v2f o;
                uint cmdID = GetCommandID(0);
                uint instanceID = GetIndirectInstanceID(svInstanceID);
                v.vertex.z = (v.vertex.z + _lazerLength) * v.color.r;
                v.vertex.x = v.vertex.x + v.vertex.z * width * v.vertex.x;
                float randomVal = random(float2((float)instanceID, 352134.0));
                 half c = cos(1 * sin(instanceID * 100*_Time.z*0.0011));
                half s = sin(1* sin(instanceID*100*_Time.z*0.001));
                 half4x4 rotateMatrixY = half4x4(c, 0, s, 0,
                                               0, 1, 0, 0,
                                               -s, 0, c, 0,
                                               0, 0, 0, 1);

               v.vertex = mul(rotateMatrixY, v.vertex);
                float4 wpos = mul(_ObjectToWorld, v.vertex);
                o.pos = mul(UNITY_MATRIX_VP, wpos);
                o.color = _Color;
                
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}