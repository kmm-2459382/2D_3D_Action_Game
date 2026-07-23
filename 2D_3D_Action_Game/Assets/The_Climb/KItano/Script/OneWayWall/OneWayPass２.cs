using UnityEngine;

public class OneWayPass2 : MonoBehaviour
{
    [Header("一方通行の基準方向（例：下向きに通過を許可したいなら下を向ける）")]
    [SerializeField] private Transform directionRef;

    [Header("通行を制御する壁コライダー")]
    [SerializeField] private Collider wallCollider;

    [Header("対象プレイヤータグ")]
    [SerializeField] private string playerTag = "Player";

    [Header("方向しきい値（0.1〜0.3程度が推奨）")]
    [SerializeField, Range(0f, 1f)] private float directionThreshold = 0.2f;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        Vector3 dirToPlayer = (other.transform.position - transform.position).normalized;
        float dot = Vector3.Dot(directionRef.up, dirToPlayer);

        // 上→下通過を許可: dot > directionThreshold のとき通過OK
        if (dot > directionThreshold)
        {
            //Debug.Log("通行許可");
            wallCollider.enabled = false;
            //Physics.IgnoreCollision(other, wallCollider, true);
        }
        else if (dot < -directionThreshold)
        {
            wallCollider.enabled = true;
            //Physics.IgnoreCollision(other, wallCollider, false);
        }
        // dotがしきい値内（-0.2〜0.2）は何もしない＝状態を維持
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            wallCollider.enabled = true;
            //Physics.IgnoreCollision(other, wallCollider, false);
        }
    }
}
