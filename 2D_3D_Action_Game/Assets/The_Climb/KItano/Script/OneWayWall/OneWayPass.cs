using UnityEngine;

public class OneWayPass : MonoBehaviour
{
    [Header("一方通行の基準方向")]
    [SerializeField] private Transform forwardDirection; // 通過OKの方向
    [Header("通行を制御する壁コライダー")]
    [SerializeField] private Collider wallCollider;
    [Header("対象プレイヤータグ")]
    [SerializeField] private string playerTag = "Player";

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        Vector3 dirToPlayer = (other.transform.position - transform.position).normalized;
        float dot = Vector3.Dot(forwardDirection.forward, dirToPlayer);

        // dot > 0 → 正しい方向（通過OK）／ dot < 0 → 逆方向（ブロック）
        if (dot > 0)
        {
            Debug.Log("通行許可");
            Physics.IgnoreCollision(other, wallCollider, true);
        }
        else
        {
            Physics.IgnoreCollision(other, wallCollider, false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 範囲外に出たら常に再有効化（安全処理）
        if (other.CompareTag(playerTag))
        {
            Physics.IgnoreCollision(other, wallCollider, false);
        }
    }
}
