using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerAutoPushout : MonoBehaviour
{
    [Header("押し出し設定")]
    public float pushForce = 5f;         // 押し出しの強さ
    public LayerMask pushOutLayers;      // 押し出し対象のレイヤー
    public float checkRadius = 0.5f;     // 衝突検知の半径
    public float verticalBoost = 0.5f;   // 上方向に加える補助（めり込み防止用）

    private Rigidbody rb;
    private Collider col;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    private void FixedUpdate()
    {
        // 周囲のコライダーを取得
        Collider[] hits = Physics.OverlapSphere(transform.position, checkRadius, pushOutLayers);

        foreach (var hit in hits)
        {
            if (hit == col) continue; // 自分自身は無視
            if (!col.bounds.Intersects(hit.bounds)) continue; // 実際に重なっていなければスキップ

            // 押し出し方向（相手の中心 → 自分の中心）
            Vector3 direction = transform.position - hit.bounds.center;
            direction.y += verticalBoost; // 少し上方向に力を加える
            direction.Normalize();

            // 押し出し力を加える
            rb.AddForce(direction * pushForce, ForceMode.Force);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
#endif
}
