Shader "Custom/SpaceNebulaIndividualTwinkle"
{
    Properties
    {
        _NebulaColor ("Nebula Color", Color) = (0.15,0.2,0.4,1)
        _StarDensity ("Star Density", Range(50,300)) = 150
        _StarBrightness ("Star Brightness", Range(0,5)) = 2
        _TwinkleMinSpeed ("Twinkle Min Speed", Range(0,5)) = 0.5
        _TwinkleMaxSpeed ("Twinkle Max Speed", Range(0,10)) = 4
        _NebulaStrength ("Nebula Strength", Range(0,2)) = 0.7
        _StarFlowSpeed ("Star Flow Speed", Range(0,5)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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

            float4 _NebulaColor;
            float _StarDensity;
            float _StarBrightness;
            float _TwinkleMinSpeed;
            float _TwinkleMaxSpeed;
            float _NebulaStrength;
            float _StarFlowSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * 10;
                return o;
            }

            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1,311.7))) * 43758.5453);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float a = hash(i);
                float b = hash(i + float2(1,0));
                float c = hash(i + float2(0,1));
                float d = hash(i + float2(1,1));
                float2 u = f * f * (3 - 2 * f);
                return lerp(a, b, u.x) +
                       (c - a) * u.y * (1 - u.x) +
                       (d - b) * u.x * u.y;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float time = _Time.y;

                // ===== ネビュラ =====
                float nebula = noise(i.uv * 0.4 + time * 0.05);
                float3 nebulaCol = _NebulaColor.rgb * nebula * _NebulaStrength;

                // ===== 星UVを流す =====
                float2 starUV = i.uv;
                starUV.y += time * _StarFlowSpeed;

                float2 cell = floor(starUV * _StarDensity);

                // 星が存在するか
                float starExist = step(0.995, hash(cell));

                // 星ごとの個性
                float phase = hash(cell + 10.1) * 6.28318; // 0〜2π
                float speed = lerp(_TwinkleMinSpeed, _TwinkleMaxSpeed, hash(cell + 23.7));
                float intensityRand = hash(cell + 55.5);

                // 個別瞬き
                float twinkle =
                    sin(time * speed + phase) * 0.5 + 0.5;

                // 急に消えないようにカーブ調整
                twinkle = smoothstep(0.2, 1.0, twinkle);

                float starIntensity =
                    starExist * twinkle * intensityRand * _StarBrightness;

                float3 color = nebulaCol + starIntensity;

                return float4(color, 1);
            }
            ENDCG
        }
    }
}
