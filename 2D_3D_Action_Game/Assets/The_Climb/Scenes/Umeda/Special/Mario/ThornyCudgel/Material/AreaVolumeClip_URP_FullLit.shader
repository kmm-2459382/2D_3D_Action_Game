Shader "Custom/AreaVolumeClip_URP_FullLit"
{
    Properties
    {
        _Color ("Base Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        // URPの標準的なLit（不透明）設定
        Tags { 
            "RenderType"="Opaque" 
            "RenderPipeline"="UniversalPipeline" 
            "Queue"="Geometry" 
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // URPのコアライブラリとライティングライブラリをインクルード
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
            };

            struct Varyings {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD1;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD3;
            };

            sampler2D _MainTex;
            
            // CBUFFERを使うことでSRP Batcherに対応し、効率化
            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _Smoothness;
                float _Metallic;
                float4x4 _AreaWorldToLocal;
            CBUFFER_END

            Varyings vert(Attributes IN) {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target {
                // --- 1. エリア判定（クリッピング） ---
                // 行列が未設定（初期値0）の場合は描画しない
                if (_AreaWorldToLocal[0][0] == 0) discard;

                float3 localPos = mul(_AreaWorldToLocal, float4(IN.positionWS, 1.0)).xyz;
                float3 d = abs(localPos) - 0.5;
                if (max(d.x, max(d.y, d.z)) > 0) discard;

                // --- 2. PBR（物理ベース）ライティング計算 ---
                // テクスチャと色のサンプリング
                float4 albedo = tex2D(_MainTex, IN.uv) * _Color;

                // 入力データの準備
                InputData inputData = (InputData)0;
                inputData.positionWS = IN.positionWS;
                inputData.normalWS = normalize(IN.normalWS);
                inputData.viewDirectionWS = SafeNormalize(GetCameraPositionWS() - IN.positionWS);
                
                // シャドウ（影）の計算
                float4 shadowCoord = TransformWorldToShadowCoord(IN.positionWS);
                inputData.shadowCoord = shadowCoord;

                // 表面データの準備（ここでMetallicとSmoothnessを適用）
                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = albedo.rgb;
                surfaceData.metallic = _Metallic;
                surfaceData.smoothness = _Smoothness;
                surfaceData.specular = half3(0, 0, 0); // Metallic使用時は0でOK
                surfaceData.occlusion = 1.0;
                surfaceData.alpha = albedo.a;

                // URP標準のPBR計算を実行
                return UniversalFragmentPBR(inputData, surfaceData);
            }
            ENDHLSL
        }

        // 影を落とすためのパス
        UsePass "Universal Render Pipeline/Lit/ShadowCaster"
    }
}