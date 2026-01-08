Shader "URP/SpiderWebFill"
{
    Properties
    {
        _LineColor("Line Color", Color) = (1,1,1,1)
        _BgColor("Background Color", Color) = (0,0,0,0)

        _WebCenter("Web Center (World)", Vector) = (0,0,0,0)

        _RingSpacing("Ring Spacing", Float) = 0.5
        _RingThickness("Ring Thickness", Float) = 0.04

        _RadialCount("Radial Count", Float) = 16
        _RadialThickness("Radial Thickness", Float) = 0.04
    }

        SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _LineColor;
            float4 _BgColor;
            float4 _WebCenter;

            float _RingSpacing;
            float _RingThickness;

            float _RadialCount;
            float _RadialThickness;

            #define MY_PI 3.14159265

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos    : TEXCOORD0;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.worldPos = TransformObjectToWorld(v.positionOS.xyz);
                return o;
            }

            // 中间亮，两边渐隐的线形 mask
            float LineMask(float distToCenter, float halfWidth)
            {
                halfWidth = max(halfWidth, 1e-5);
                float d = saturate(1.0 - distToCenter / halfWidth);
                return d * d; // 稍微柔一点
            }

            float4 frag(Varyings i) : SV_Target
            {
                // 以 WebCenter 为原点的世界坐标
                float2 p = i.worldPos.xy - _WebCenter.xy;

                // 极坐标
                float r = length(p);
                float angle = atan2(p.y, p.x);          // [-PI, PI]
                angle = (angle + MY_PI) / (2.0 * MY_PI); // [0,1]

                // ------- 同心圆 --------
                float ringMask = 0.0;
                if (_RingSpacing > 0.0001)
                {
                    float k = r / _RingSpacing;
                    float fracPart = frac(k);                        // [0,1)
                    float distFrac = min(fracPart, 1.0 - fracPart);  // 最近到 0/1 的距离
                    float dist = distFrac * _RingSpacing;

                    ringMask = LineMask(dist, _RingThickness * 0.5);
                }

                // ------- 放射线 --------
                float radialMask = 0.0;
                if (_RadialCount >= 1.0)
                {
                    float stepAngle = 1.0 / _RadialCount;
                    float aNorm = angle / stepAngle;
                    float fracPart = frac(aNorm);
                    float distFrac = min(fracPart, 1.0 - fracPart);
                    float dist = distFrac * stepAngle;

                    radialMask = LineMask(dist, stepAngle * _RadialThickness * 0.5);
                }

                float lineMask = saturate(ringMask + radialMask);

                float4 col = lerp(_BgColor, _LineColor, lineMask);
                return col;
            }
            ENDHLSL
        }
    }
}
