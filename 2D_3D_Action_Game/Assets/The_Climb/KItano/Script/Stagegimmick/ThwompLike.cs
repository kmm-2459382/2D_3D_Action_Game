using UnityEngine;
using System.Collections;

public class ThwompLike : MonoBehaviour
{
    [Header("Raycast設定")]
    [SerializeField] private float rayLength = 10f;
    [SerializeField] private LayerMask detectLayer;

    [Header("速度設定")]
    [SerializeField] private float fallSpeed = 20f;   // 落下速度
    [SerializeField] private float riseSpeed = 5f;    // 戻る速度

    [Header("待機時間")]
    [SerializeField] private float interval = 1.0f;   // 地面で止まる時間

    [Header("接地判定")]
    [SerializeField] private LayerMask groundLayer;

    private Vector3 startPosition;
    private bool isMoving = false;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if (isMoving) return;

        // 下向きRaycastでプレイヤー検知
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength, detectLayer))
        {
            StartCoroutine(ThwompMove());
        }
    }

    private IEnumerator ThwompMove()
    {
        isMoving = true;

        // 落下
        while (true)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;

            // 地面に当たったら停止
            if (Physics.Raycast(transform.position, Vector3.down, 0.6f, groundLayer))
            {
                break;
            }

            yield return null;
        }

        // インターバル
        yield return new WaitForSeconds(interval);

        // 上昇（元の位置へ）
        while (Vector3.Distance(transform.position, startPosition) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                startPosition,
                riseSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = startPosition;
        isMoving = false;
    }

    // デバッグ用Ray表示
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * rayLength);
    }
}
