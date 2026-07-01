Shader "Custom/TriggerInsideStencil"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry-1" "RenderPipeline"="UniversalPipeline" }

        // --- ここがステンシルの設定です ---
        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace
        }

        // 画面には何も描画せず、ステンシルの目印（Ref 1）だけを書き込む設定
        ColorMask 0
        ZWrite On

        Pass
        {
            Name "ForwardLit"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                return half4(0,0,0,0);
            }
            ENDHLSL
        }
    }
}