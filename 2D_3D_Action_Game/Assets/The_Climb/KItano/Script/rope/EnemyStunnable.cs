
using UnityEngine;
using System.Collections;

public class EnemyStunnable : MonoBehaviour
{
    public float stunDuration = 3f;
    private bool isStunned = false;

    public void Stun()
    {
        if (!isStunned)
        {
            isStunned = true;
            Debug.Log($"{gameObject.name} はスタン状態になりました！");
            StartCoroutine(StunCoroutine());
        }
    }

    private IEnumerator StunCoroutine()
    {
        // 行動停止処理（例：AI停止）
        // GetComponent<EnemyAI>().enabled = false;

        yield return new WaitForSeconds(stunDuration);

        // 行動再開
        // GetComponent<EnemyAI>().enabled = true;
        isStunned = false;
        Debug.Log($"{gameObject.name} のスタンが解除されました。");
    }
}
