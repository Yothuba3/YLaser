Shader "Custom/URP BillboardLaserYAxis"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Geometry" }
        LOD 200

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionHCS : SV_POSITION;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _Color;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                // Transform the position to world space
                float3 worldPos = TransformObjectToWorld(IN.positionOS).xyz;

                // Compute billboard orientation with Y-axis constraint
                float3 toCamera = GetCameraPositionWS() - worldPos;
               // toCamera.x = 0.0; // Ignore Y component to constrain rotation around Y axis
                toCamera = normalize(toCamera);

                float3 up = float3(0, 1, 0);
                float3 right = normalize(cross(up, toCamera));
                up = cross(toCamera, right);

                // Calculate the offset position
                float3 offset = right * IN.positionOS.x + up * IN.positionOS.y;
                float3 billboardPos = worldPos + offset;

                // Transform the position to Homogeneous Clip Space
                OUT.positionHCS = TransformWorldToHClip(billboardPos);
                OUT.uv = IN.uv;

                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                return tex * _Color;
            }
            ENDHLSL
        }
    }
}

