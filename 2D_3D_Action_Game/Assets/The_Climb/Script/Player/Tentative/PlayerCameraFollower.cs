using UnityEngine;

public class PlayerCameraFollower : MonoBehaviour
{
    public enum FollowMode
    {
        Tight,   // カッチリ追従
        Smooth   // ふわっと追従＋ズーム効果付き
    }

    [Header("追従設定")]
    public Camera targetCamera;
    public FollowMode followMode = FollowMode.Tight;

    [Header("オフセット設定")]
    public Vector3 offset = new Vector3(0, 3, -5);

    [Header("スムーズ設定")]
    public float smoothSpeed = 2f;           // 追従スピード
    public float zoomOutDistance = 0f;       // 動いたときのズームアウト距離
    public float zoomSpeed = 2f;             // ズーム変化スピード
    public float heightFollowSpeed = 10f;     // 高さ追従のスピード（ジャンプ対策）

    [Header("移動判定")]
    public float moveThreshold = 0.05f;      // 動いてるとみなす速度

    private Vector3 velocity;                // 位置補間用
    private Vector3 currentOffset;
    private Vector3 lastPosition;
    private float zoomT = 0f;                // ズーム補間用

    private void Start()
    {
        if (targetCamera == null)
        {
            Debug.LogWarning("[PlayerCameraFollower] カメラが指定されていません！");
            return;
        }

        lastPosition = transform.position;
        currentOffset = offset;
    }

    private void LateUpdate()
    {
        if (targetCamera == null) return;

        switch (followMode)
        {
            case FollowMode.Tight:
                FollowTight();
                break;

            case FollowMode.Smooth:
                FollowSmooth();
                break;
        }

        lastPosition = transform.position;
    }

    // ======================
    // カッチリ追従モード
    // ======================
    private void FollowTight()
    {
        targetCamera.transform.position = transform.position + offset;
    }

    // ======================
    // ふわっと追従モード
    // ======================
    private void FollowSmooth()
    {
        // --- プレイヤー移動判定 ---
        Vector3 moveDelta = transform.position - lastPosition;
        bool isMoving = moveDelta.magnitude > moveThreshold;

        // --- ズーム補間 ---
        float targetZoomT = isMoving ? 1f : 0f;
        zoomT = Mathf.Lerp(zoomT, targetZoomT, Time.deltaTime * zoomSpeed);

        // --- オフセット調整（ズームアウト）---
        Vector3 zoomedOffset = offset + Vector3.back * zoomOutDistance * zoomT;

        // --- 高さのふわっと追従 ---
        float targetHeight = transform.position.y + zoomedOffset.y;
        float newY = Mathf.Lerp(targetCamera.transform.position.y, targetHeight, Time.deltaTime * heightFollowSpeed);

        // --- カメラの目標位置 ---
        Vector3 targetPos = new Vector3(
            transform.position.x + zoomedOffset.x,
            newY,
            transform.position.z + zoomedOffset.z
        );

        // --- スムーズに移動 ---
        targetCamera.transform.position = Vector3.Lerp(
            targetCamera.transform.position,
            targetPos,
            Time.deltaTime * smoothSpeed
        );
    }
}
