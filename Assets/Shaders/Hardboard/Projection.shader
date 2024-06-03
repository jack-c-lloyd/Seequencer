Shader "Hardboard/Projection"
{
    Properties
    {
        _Depth ("Depth", float) = 1000
        _InnerDiameter ("InnerDiameter", Range(0, 10.0)) = 1.5
        _OuterDiameter ("OuterDiameter", Range(0.00872665, 10.0)) = 2.0
        _DistanceInMeters ("DistanceInMeters", Range(0.0, 20.0)) = 2.0
    }

    SubShader
    {
        Tags
        {
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "Queue"="Overlay"
        }

        Pass
        {
            Name "Projection"

            Blend OneMinusDstColor OneMinusSrcColor
            ZTest NotEqual
            ZWrite On

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            uniform float _Depth;
            uniform float _InnerDiameter;
            uniform float _OuterDiameter;
            uniform float _DistanceInMeters;

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(float4 pos : POSITION)
            {
                float scale = lerp(_OuterDiameter, _InnerDiameter, pos.z);
                float3 vert_out = float3(pos.x * scale, pos.y * scale, _DistanceInMeters);
                v2f o = { UnityObjectToClipPos(vert_out) };
                return o;
            }

            fixed4 frag(v2f i, out float depth : SV_Depth) : SV_Target
            {
                #if defined(UNITY_REVERSED_Z)
                    depth = 1;
                #else
                    depth = -1;
                #endif

                return fixed4(1,1,1,1);
            }

            ENDHLSL
        }
    }

    Fallback Off
}