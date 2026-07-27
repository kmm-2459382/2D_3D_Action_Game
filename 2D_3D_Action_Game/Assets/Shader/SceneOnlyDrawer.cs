using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Sceneビューの描画時のみMeshRendererを有効化し、Gameビューやビルド時は非表示にする
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
public class SceneOnlyDrawer : MonoBehaviour
{
    private Renderer targetRenderer;

    private void OnEnable()
    {
        targetRenderer = GetComponent<Renderer>();
        // レンダーパイプラインのカメラ描画前イベントに登録
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
    }

    private void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (targetRenderer == null) return;

        // 描画を行おうとしているカメラが「SceneViewカメラ」の場合のみ描画を有効にする
        bool isSceneCamera = camera.cameraType == CameraType.SceneView;
        targetRenderer.enabled = isSceneCamera;
    }
}