using UnityEngine;

public class OrbitalMotion : MonoBehaviour
{
    [Header("ライトの移動用スクリプト（円軌道）")]
    public Transform center; // 回転の中心

    [Header("回転設定")]
    public float radius = 2f;         // 回転半径
    public float speed = 30f;         // 回転スピード（度/秒）
    public bool clockwise = true;     // 時計回りかどうか
    public Axis rotationAxis = Axis.Y; // 回転軸の選択

    [Header("配置設定")]
    [Tooltip("開始角度のオフセット（度）")]
    public float startAngleOffset = 0f; // 複数個を等間隔にずらすときに使う

    private float angle; // 現在の角度（度）

    public enum Axis { X, Y, Z }

    void Start()
    {
        if (center == null)
            center = transform.parent;

        if (center != null)
            transform.position = center.position + GetOffset(startAngleOffset);
    }

    void Update()
    {
        if (center == null) return;

        float direction = clockwise ? -1f : 1f;

        angle += speed * Time.deltaTime * direction;
        if (angle > 360f || angle < -360f) angle = 0f;

        transform.position = center.position + GetOffset(angle + startAngleOffset);
    }

    private Vector3 GetOffset(float angleDeg)
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        switch (rotationAxis)
        {
            case Axis.X: return new Vector3(0f, cos, sin) * radius; // X軸中心
            case Axis.Y: return new Vector3(cos, 0f, sin) * radius; // Y軸中心
            case Axis.Z: return new Vector3(cos, sin, 0f) * radius; // Z軸中心
            default: return Vector3.zero;
        }
    }
}