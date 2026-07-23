using UnityEngine;

/// <summary>
/// シンプルなカメラ追従スクリプト（1プレイヤー向け）
/// LateUpdateでターゲットを追い、スムーズに移動
/// Optional: X/Yそれぞれの追従ON/OFF、オフセット、移動範囲制限が可能。
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraFollowPlayer : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("追従するプレイヤーのTransformをセットしてください。")]
    public Transform target;

    [Header("Follow Settings")]
    [Tooltip("追従をスムーズにする時間（小さいほど速く追従）")]
    public float smoothTime = 0.12f;
    [Tooltip("プレイヤーに対するカメラのオフセット")]
    public Vector3 offset = new Vector3(0, 1.5f, -10f);

    [Header("Axis Control")]
    public bool followX = true;
    public bool followY = true;
    public bool followZ = true; // 普通はカメラのZは固定

    [Header("Bounds (optional)")]
    public bool useBounds = false;
    public Vector2 minBounds = new Vector2(-Mathf.Infinity, -Mathf.Infinity);
    public Vector2 maxBounds = new Vector2(Mathf.Infinity, Mathf.Infinity);

    [Header("Other")]
    [Tooltip("プレイヤーが消えたときに追従を停止する")]
    public bool stopWhenTargetNull = true;

    // 内部
    private Vector3 velocity = Vector3.zero;

    void Reset()
    {
        // デフォルトオフセットをカメラ基準で設定
        offset = new Vector3(0f, 1.5f, -10f);
    }

    void LateUpdate()
    {
        if (target == null)
        {
            if (stopWhenTargetNull) return;
            else return;
        }

        // 目標位置 = ターゲット位置 + オフセット（ワールド空間）
        Vector3 desired = target.position + offset;

        // 現在のカメラ位置
        Vector3 current = transform.position;

        // 各軸ごとに追従するかを決める
        Vector3 targetPos = current;
        if (followX) targetPos.x = desired.x;
        if (followY) targetPos.y = desired.y;
        if (followZ) targetPos.z = desired.z;

        // スムーズに移動
        Vector3 smoothPos = Vector3.SmoothDamp(current, targetPos, ref velocity, smoothTime);

        // 範囲制限を適用
        if (useBounds)
        {
            float clampedX = Mathf.Clamp(smoothPos.x, minBounds.x, maxBounds.x);
            float clampedY = Mathf.Clamp(smoothPos.y, minBounds.y, maxBounds.y);
            smoothPos.x = clampedX;
            smoothPos.y = clampedY;
        }

        transform.position = smoothPos;
    }

    /// <summary>
    /// ターゲットを動的にセットするヘルパー
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        // ターゲット設定時にカメラを瞬時にセンターさせたい場合は以下を有効化
        // transform.position = target.position + offset;
        velocity = Vector3.zero;
    }
}
