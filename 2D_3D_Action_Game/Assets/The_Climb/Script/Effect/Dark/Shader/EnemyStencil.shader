Shader "Unlit/EnemyStencil"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}    //  メインテクスチャ
        _Color ("Color", Color) = (1,1,1,0.4)    //  色
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        LOD 100

        Pass
        {
            Stencil    //  ステンシル設定
            {
                Ref 1
                Comp Always
                Pass Replace
            }

            ZWrite Off
            Cull Back
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert    //  頂点シェーダーのエントリーポイント
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

            float4 _Color;    //  色値

            Varyings vert(Attributes IN)    //  頂点関数
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target    //  ピクセル関数
            {
                half4 texcol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                return texcol * _Color;
            }
            ENDHLSL
        }
    }
}