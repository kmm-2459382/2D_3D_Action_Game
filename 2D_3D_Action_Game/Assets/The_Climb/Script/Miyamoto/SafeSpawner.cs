//using UnityEngine;

//public class SafeSpawner : MonoBehaviour
//{
//    private BoxCollider col;
//    [SerializeField] private LayerMask collisionMask; // 壁や地面のレイヤー

//    [Header("探索設定")]
//    [SerializeField] private float step = 1.0f;     // 上に動かす距離
//    [SerializeField] private int maxAttempts = 20;  // 最大試行回数

//    private void Awake()
//    {
//        col = GetComponent<BoxCollider>();
//    }

//    /// <summary>
//    /// 現在位置から安全なスポーン位置を探す
//    /// </summary>
//    public Vector3 FindSafePosition(Vector3 startPos)
//    {
//        Vector3 pos = startPos;

//        if (IsColliding(pos))
//        {
//            Debug.Log("埋まっているので安全な位置を探索します…");

//            // 埋まっている間は BoxCollider を無効化
//            col.enabled = false;

//            for (int i = 0; i < maxAttempts; i++)
//            {
//                pos += Vector3.up * step;

//                if (!IsColliding(pos))
//                {
//                    Debug.Log("安全な位置を発見！ " + pos);

//                    // 安全な場所を見つけたら Collider を再有効化
//                    col.enabled = true;
//                    transform.position = pos;
//                    return pos;
//                }
//            }

//            Debug.LogWarning("安全な位置が見つかりませんでした。最後の位置を使用します。");
//            col.enabled = true; // 失敗しても必ず戻す
//        }

//        return pos;
//    }

//    /// <summary>
//    /// その位置に Box が埋まっているかどうかを判定
//    /// </summary>
//    private bool IsColliding(Vector3 centerPos)
//    {
//        // BoxColliderの大きさを取得
//        Vector3 halfExtents = col.size * 0.5f;

//        // ワールド座標に合わせる（BoxColliderはオフセットがある場合があるので注意）
//        Vector3 worldCenter = centerPos + col.center;

//        return Physics.CheckBox(worldCenter, halfExtents, transform.rotation, collisionMask);
//    }
//}
