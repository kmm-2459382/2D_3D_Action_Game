// //  画面フェードアウト処理
// Shader "Custom/FullscreenFadeOut"
// {
//     Properties
//     {
//         _Fade ("Fade", Range(0, 1)) = 0    //  フェードアウト進行価
//         _Color ("Fade Color", Color) = (0, 0, 0, 1)    //  フェードアウトする色
//     }
//     SubShader
//     {
//         Tags { "RenderType"="Opaque" }
//         Pass
//         {
//             ZTest Always Cull Off ZWrite Off
//             Blend SrcAlpha OneMinusSrcAlpha

//             HLSLPROGRAM
//             #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

//             #pragma vertex vert
//             #pragma fragment frag

//             float _Fade;    //  フェードアウト進行価
//             float4 _Color;    //  フェードアウトする色

//             struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
//             struct v2f { float4 vertex : SV_POSITION; float2 uv : TEXCOORD0; };

//             v2f vert (appdata v) {
//                 v2f o;    //  V2f型の変数
//                 o.vertex = TransformObjectToHClip(v.vertex.xyz);
//                 o.uv = v.uv;
//                 return o;
//             }

//             half4 frag (v2f i) : SV_Target {
//                 return float4(_Color.rgb, _Fade);
//             }
//             ENDHLSL
//         }
//     }
//     Fallback Off
// }