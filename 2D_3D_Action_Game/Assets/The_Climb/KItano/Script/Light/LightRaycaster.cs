using UnityEngine;

public class LightRaycaster : MonoBehaviour
{
    public float rayDistance = 10f;

    // Rayが当たる対象すべて（Block + Reactive）
    public LayerMask hitLayer;

    // 光を遮るレイヤー
    public LayerMask blockLayer;

    void Update()
    {
        Vector3 direction = transform.right; // XY平面のライト向き

        // コライダーから少しだけ外に出すオフセット
        const float startOffset = 0.1f;

        Vector3 startPos = transform.position - direction * startOffset;

        Ray ray = new Ray(startPos, direction);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, rayDistance, hitLayer))
            return;

        // Block 判定
        if (((1 << hit.collider.gameObject.layer) & blockLayer) != 0)
        {
            Debug.Log("hit");
            return;
        }

        var reactive = hit.collider.GetComponent<RaycastReactiveObject>();
        if (reactive != null)
        {
            reactive.OnRaycastHit();
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            transform.position,
            transform.position + transform.right * rayDistance
        );
    }
}
