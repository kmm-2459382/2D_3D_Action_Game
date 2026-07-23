using UnityEngine;

public class LockObject : MonoBehaviour
{
    [Header("アンロック時に破壊するか")]
    public bool destroyOnUnlock = true;

    [Header("アンロック時のエフェクト（任意）")]
    public GameObject unlockEffect;

    private bool isUnlocked = false;

    public void Unlock()
    {
        if (isUnlocked) return;
        isUnlocked = true;

        Debug.Log($"🔓 {gameObject.name} がアンロックされました！");

        // エフェクト生成
        if (unlockEffect != null)
        {
            Instantiate(unlockEffect, transform.position, Quaternion.identity);
        }

        // ロック解除後、一定時間後に削除
        if (destroyOnUnlock)
        {
            Destroy(gameObject, 0.5f); // 少し余韻を残して削除
        }
        else
        {
            // 破壊しない場合は無効化だけでも可
            gameObject.SetActive(false);
        }
    }
}
