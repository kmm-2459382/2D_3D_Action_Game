Shader "Shader/EdgeDarkDefusion"
{
    Properties
    {
        _Progress ("Progress", Range(0,1)) = 1    //  闇の進行度
    }
    SubShader
    {
        Tags { "Queue" = "Transparent+10" "RenderType" = "Transparent" }
        LOD 100

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            Stencil    //  ステンシル設定
            {
                Ref 1
                comp Always
                Pass Keep
            }

            HLSLPROGRAM
            #pragma vertex vert    //  頂点シェーダのエントリーポイント
            #pragma fragment frag    //  フラグメントシェーダーのエントリーポイント
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            struct Attributes    //  モデルから渡される頂点情報
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings    //  頂点からフラグメントへ渡す保管データ情報
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float _Progress;    //  闇の進行度

            Varyings vert(Attributes IN)    //  頂点関数
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target    //  ピクセル関数
            {
                float2 uv = IN.uv;    //  テクスチャ座標
                float2 center = float2(0.5, 0.5);    //  UV空間によるテクスチャ中央値

                // 四隅を強調した矩形上のマスク
                float2 offset = abs(uv - center);
                float dist = length(offset);

                float fadeStart = -0.5;    //  フェード開始値(UV座標)
                float fadeComplete = 0;    //  フェード完了値(UV座標)
                //  進行値に応じて闇の色に画面端から近づける
                 float mask = smoothstep(fadeStart, fadeComplete, dist - _Progress);

                half4 orig = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                half3 col = orig.rgb * (1 - mask);
                return half4(col, mask);
            }
            ENDHLSL
        }
    }
}
//    コード保存所    //
// float dist = max(offset.x, offset.y);    闇(□に迫ってくる)
