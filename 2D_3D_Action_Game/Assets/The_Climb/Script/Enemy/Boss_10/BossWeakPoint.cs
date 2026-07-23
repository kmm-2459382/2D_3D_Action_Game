using UnityEngine;

public class BossWeakPoint : MonoBehaviour
{
    private BossEnemy_HevvyMovement boss;

    private void Start()
    {
        boss = GetComponentInParent<BossEnemy_HevvyMovement>();
        if (boss == null)
        {
            Debug.LogError("[WeakPoint] BossEnemy_HevvyMovement が親に見つかりません！");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            boss?.OnHit(); // 親のダメージ処理を呼び出す
        }
    }

    public void ActivateWeakPoint()
    {
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = true;
    }

    public void DeactivateWeakPoint()
    {
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }
}