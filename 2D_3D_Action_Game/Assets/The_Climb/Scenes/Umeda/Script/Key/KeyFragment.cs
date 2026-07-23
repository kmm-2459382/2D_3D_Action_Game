using UnityEngine;

public class KeyFragment : MonoBehaviour
{
    [Header("この欠片が属する鍵グループID")]
    public string keyID = "KeyA";

    private bool isCollected = false; // ← 追加：多重衝突を防ぐ

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;  // ← すでに処理中なら無視

        var collector = other.GetComponent<KeyCollector>();
        if (collector != null)
        {
            isCollected = true;   // ← ここでロックして二度目以降の呼び出しを防ぐ
            collector.CollectFragment(keyID);
            Destroy(gameObject);
        }
    }
}
