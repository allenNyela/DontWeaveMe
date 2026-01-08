Shader "URP/SpiderWebCurved"
{
    Properties
    {
        _LineColor("Line Color", Color) = (1,1,1,1)
        _BgColor("Background Color", Color) = (0,0,0,0)

        _WebCenter("Web Center (World)", Vector) = (0,0,0,0)

        _RingCount("Ring Count", Float) = 5
        _RingSpacing("Ring Spacing", Float) = 0.5
        _RingThickness("Ring Thickness", Float) = 0.03
        _RingSagAmount("Ring Sag Amount", Float) = 0.25     // 弧度，下垂程度 0~0.5 左右

        _RadialCount("Radial Count", Float) = 8
        _RadialThickness("Radial Thickness", Float) = 0.03
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

            float _RingCount;
            float _RingSpacing;
            float _RingThickness;
            float _RingSagAmount;

            float _RadialCount;
            float _RadialThickness;

            #define MY_PI 3.14159265
            #define MAX_RINGS 16        // 防止 shader 循环过长

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
                return d * d;
            }

            float4 frag(Varyings i) : SV_Target
            {
                // 以 WebCenter 为原点的世界坐标
                float2 p = i.worldPos.xy - _WebCenter.xy;

                float r = length(p);
                float angle = atan2(p.y, p.x);          // [-PI, PI]
                // 归一化到 [0,1]
                float angle01 = (angle + MY_PI) / (2.0 * MY_PI);

                // ---------- 放射线 ----------
                float radialMask = 0.0;
                if (_RadialCount >= 1.0)
                {
                    float stepAngle = 1.0 / _RadialCount;
                    float aNorm = angle01 / stepAngle;
                    float fracPart = frac(aNorm);
                    float distFrac = min(fracPart, 1.0 - fracPart);
                    float dist = distFrac * stepAngle;

                    radialMask = LineMask(dist, stepAngle * _RadialThickness * 0.5);
                }

                // ---------- 弧形“环” ----------
                float ringMask = 0.0;

                if (_RingCount > 0 && _RingSpacing > 0.0001)
                {
                    float stepAngle = 1.0 / max(_RadialCount, 1.0);
                    float sectorNorm = angle01 / stepAngle;
                    float sectorFrac = frac(sectorNorm);       // 0~1，扇区内局部角

                    // 让弧形在扇区中间鼓起：u ∈ [0,1]
                    float u = sectorFrac;
                    // 弧形偏移：0 在两端，1 在中间
                    float sagShape = 4.0 * u * (1.0 - u);       // 抛物线

                    int rings = (int)min(_RingCount, (float)MAX_RINGS);

                    for (int k = 0; k < MAX_RINGS; k++)
                    {
                        if (k >= rings) break;

                        float baseR = (k + 1) * _RingSpacing;
                        // 在两端接到放射线，中间向外鼓一点
                        float sag = _RingSagAmount * baseR;
                        float targetR = baseR - sag * sagShape;

                        float distR = abs(r - targetR);
                        float thisRing = LineMask(distR, _RingThickness * 0.5);
                        ringMask = max(ringMask, thisRing);
                    }
                }

                float lineMask = saturate(max(radialMask, ringMask));

                float4 col = lerp(_BgColor, _LineColor, lineMask);
                return col;
            }
            ENDHLSL
        }
    }
}
