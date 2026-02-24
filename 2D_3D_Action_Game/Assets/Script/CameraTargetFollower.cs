using UnityEngine;

public class CameraTargetFollower : MonoBehaviour
{
    [Tooltip("追従したい対象（PlayerArmatureなど）")]
    public Transform Target;

    [Tooltip("キャラクターの足元からの高さオフセット")]
    public Vector3 Offset = new Vector3(0, 1.37f, 0);

    // キャラクターの移動が終わった後に実行するため LateUpdate を使用
    private void LateUpdate()
    {
        if (Target == null) return;

        // 位置だけをコピーし、回転はコピーしない（カメラの基準を安定させる）
        transform.position = Target.position + Offset;
    }
}