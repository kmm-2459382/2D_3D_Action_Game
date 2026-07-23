using UnityEngine;

public class EnemyTackleBreaker : MonoBehaviour
{
    [Header("Break Settings")]
    [SerializeField] private float breakRadius = 1.5f;

    [Header("Offset (Local XY)")]
    [SerializeField] private float offsetX = 0f;
    [SerializeField] private float offsetY = 0f;
    private bool isTackling;
    private void Update()
    {
        if (!isTackling) return;

        Vector3 center = GetBreakCenter();

        Collider[] hits = Physics.OverlapSphere(center, breakRadius);

        foreach (var hit in hits)
        {
            var block = hit.GetComponentInParent<DestructibleBlock>();

            if (block != null)
            {
                block.BreakBlock();
            }
        }
    }

    public void StartTackleBreak()
    {
        isTackling = true;
    }

    public void StopTackleBreak()
    {
        isTackling = false;
    }
    private Vector3 GetBreakCenter()
    {
        // ローカルXYをワールド座標に変換
        Vector3 localOffset = new Vector3(offsetX, offsetY, 0f);
        return transform.TransformPoint(localOffset);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = GetBreakCenter();
        Gizmos.DrawWireSphere(center, breakRadius);
    }
}