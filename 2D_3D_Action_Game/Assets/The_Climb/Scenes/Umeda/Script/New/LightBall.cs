using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LightBall : MonoBehaviour
{
    [Header("Impact生成マネージャー")]
    public LightImpactCollider impactManager; // シーン内のLightImpactColliderをInspectorで設定

    [Header("衝突対象レイヤー")]
    public LayerMask targetLayers; // ← Inspectorで複数レイヤーを選択可能

    private void OnCollisionEnter(Collision collision)
    {
        // 指定レイヤーに含まれていなければ無視
        if (((1 << collision.gameObject.layer) & targetLayers.value) == 0)
            return;

        // 🔸衝突したコライダーの中心座標を取得
        Vector3 hitCenter = collision.collider.bounds.center;

        // 🔸Impact生成
        if (impactManager != null)
        {
            impactManager.CreateImpact(hitCenter);
        }
        else
        {
            Debug.LogWarning("LightImpactColliderが設定されていません。");
        }
    }
}
