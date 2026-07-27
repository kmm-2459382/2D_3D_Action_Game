Shader "Custom/EditorOnly/SceneGizmoOverlay"
{
    Properties
    {
        _Color ("Scene View Color", Color) = (0.0, 0.8, 1.0, 0.4) // Sceneビューでの色と透明度
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
            "RenderPipeline"="UniversalPipeline" 
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off // 両面描画

        Pass
        {
            Name "ForwardLit"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                #if defined(UNITY_EDITOR)
                // SceneViewカメラ判別判定
                // SceneView描画時、_WorldSpaceCameraPos.w は 0 ではなく 1 や特定の値を返し、
                // また orthographic / perspective 切替時の投影行列値に特徴が出ます。
                // 以下の判定で SceneView 上の描画（ortho / perspective 両対応）を安定検知します。
                
                bool isSceneCamera = (unity_CameraProjection[3][3] == 1.0) || // Ortho Scene Camera
                                     (unity_CameraProjection[0][2] != 0.0) || // Offset Projection (Scene Gizmos)
                                     (_ProjectionParams.x < 0.0);             // Scene View Flip

                if (isSceneCamera)
                {
                    return _Color;
                }
                #endif

                // Gameビューおよび実機ビルド時は Alpha 0（完全透明）
                return half4(0, 0, 0, 0);
            }
            ENDHLSL
        }
    }
}