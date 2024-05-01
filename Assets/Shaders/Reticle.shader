Shader "Seequencer/Reticle"
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
            "Queue"="Overlay"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
        }

        Pass
        {
            Blend OneMinusDstColor OneMinusSrcAlpha
            ZTest Less
            ZWrite On

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            uniform float _Depth;
            uniform float _InnerDiameter;
            uniform float _OuterDiameter;
            uniform float _DistanceInMeters;

            struct vertexInput
            {
                float4 vertex : POSITION;
            };

            struct fragmentInput
            {
                float4 position : SV_POSITION;
            };

            fragmentInput vert(vertexInput i)
            {
                float scale = lerp(_OuterDiameter, _InnerDiameter, i.vertex.z);
                float3 vert_out = float3(i.vertex.x * scale, i.vertex.y * scale, _DistanceInMeters);
                fragmentInput o;
                o.position = UnityObjectToClipPos(vert_out);
                return o;
            }

            fixed4 frag(fragmentInput i, out float depth : SV_Depth) : SV_Target
            {
                depth = _Depth;
                return fixed4(1,1,1,1);
            }

            ENDCG
        }
    }
}