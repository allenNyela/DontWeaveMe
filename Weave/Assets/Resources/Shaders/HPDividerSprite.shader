Shader "URP/HPDividerSprite"
{
    Properties
    {
        _TickColor("Tick Color", Color) = (1,1,1,1)
        _TickWidth("Tick Width (UV)", Range(0.001, 0.1)) = 0.02
        _TickCount("Tick Count", Range(1, 128)) = 5
        _Softness("Edge Softness", Range(0, 0.02)) = 0.002
    }

        SubShader
    {
        Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _TickColor;
            float _TickWidth;
            float _TickCount;
            float _Softness;

            struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; };
            struct v2f { float4 vertex:SV_POSITION; float2 uv:TEXCOORD0; };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float x = i.uv.x;
                float N = max(1.0, _TickCount);
                float tickW = _TickWidth;

                float minDist = 9999.0;

                // 避免循环太多，最多 128
                int count = (int)min(N, 128.0);

                for (int k = 0; k < 128; k++)
                {
                    if (k >= count) break;

                    // 平均分布：忽略首尾
                    float center = (k + 1.0) / (N + 1.0);

                    // 距离当前 tick 中心的距离
                    float d = abs(x - center);
                    minDist = min(minDist, d);
                }

                float halfW = tickW * 0.5;
                float alpha = 1.0 - smoothstep(halfW - _Softness, halfW + _Softness, minDist);

                return float4(_TickColor.rgb, _TickColor.a * alpha);
            }
            ENDCG
        }
    }
}
