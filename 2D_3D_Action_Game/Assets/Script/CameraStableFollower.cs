using UnityEngine;

public class CameraStableFollower : MonoBehaviour
{
    public Transform PlayerTransform; // PlayerArmatureを指定
    public float VerticalOffset = 1.375f; // カメラの高さ

    void LateUpdate()
    {
        if (PlayerTransform == null) return;

        // 【重要】回転は一切無視し、位置だけを同期
        // キャラクターのボーンがどう動こうが、このオブジェクトは「直立」を維持します
        transform.position = PlayerTransform.position + Vector3.up * VerticalOffset;
    }
}