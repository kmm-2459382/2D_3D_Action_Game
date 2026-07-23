using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LuminaLightBallController : MonoBehaviour
{
    [Header("Impact生成設定")]
    public LightImpactCollider impactPrefab; // ← プレハブ参照
    public float impactLifetime = 1.5f;      // 消えるまでの時間

    [Header("衝突対象レイヤー")]
    public LayerMask targetLayers; // 衝突反応するレイヤー

    [Header("バウンド設定")]
    public float bounceForce = 8f;
    public float moveSpeed = 3f;
    public bool startRight = true;

    private Rigidbody rb;
    private bool initialized = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        InitializeMotion();
    }

    void InitializeMotion()
    {
        float dir = startRight ? 1f : -1f;
        rb.linearVelocity = new Vector3(dir * moveSpeed, bounceForce, 0f);
        initialized = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!initialized) return;
        if (((1 << collision.gameObject.layer) & targetLayers.value) == 0)
            return;

        //// 💥 Impact生成
        //Vector3 hitPos = collision.contacts[0].point;
        //if (impactPrefab != null)
        //{
        //    LightImpactCollider impact = Instantiate(impactPrefab, hitPos, Quaternion.identity);
        //    impact.TriggerExpand(); // 拡大処理スタート
        //    Destroy(impact.gameObject, impactLifetime); // 一定時間後に削除
        //}

        // 🔁 跳ね方制御
        Vector3 normal = collision.contacts[0].normal;

        if (normal.y > 0.5f)
        {
            // 地面に当たったら上方向に跳ねる
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, bounceForce, 0f);
        }
        else if (Mathf.Abs(normal.x) > 0.5f)
        {
            // 壁に当たったら反対方向＋上に跳ねる
            float dir = -Mathf.Sign(rb.linearVelocity.x);
            rb.linearVelocity = new Vector3(dir * moveSpeed, bounceForce, 0f);
        }
        else
        {
            // 上から当たったら下へ
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -bounceForce, 0f);
        }
    }
}