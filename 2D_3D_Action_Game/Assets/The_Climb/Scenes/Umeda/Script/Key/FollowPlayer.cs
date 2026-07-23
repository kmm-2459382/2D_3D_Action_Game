using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [Header("追従するターゲット（プレイヤー）")]
    public Transform target;

    [Header("ターゲットとのオフセット位置")]
    public Vector3 offset = new Vector3(0, 1.5f, 0);

    [Header("追従速度")]
    public float followSpeed = 5f;

    private void Update()
    {
        if (target == null) return;

        // 目標位置を計算
        Vector3 targetPos = target.position + offset;

        // スムーズに追従
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);
    }
}
