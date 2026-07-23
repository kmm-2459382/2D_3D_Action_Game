
using UnityEngine;

public class EnemyAreaStunner : MonoBehaviour
{
    public float stunRadius = 5f;               // 検知範囲の半径
    public LayerMask enemyLayer;                // 敵レイヤー
    public KeyCode stunKey = KeyCode.E;         // スタン発動キー
    public LineRenderer ropePrefab;             // ロープ演出用のPrefab

    void Update()
    {
        if (Input.GetKeyDown(stunKey))
        {
            Collider[] enemies = Physics.OverlapSphere(transform.position, stunRadius, enemyLayer);
            foreach (var enemy in enemies)
            {
                // スタン処理
                EnemyStunnable stunnable = enemy.GetComponent<EnemyStunnable>();
                if (stunnable != null)
                {
                    stunnable.Stun();

                    // ロープ演出
                    CreateRope(enemy.transform);
                }
            }
        }
    }

    void CreateRope(Transform enemy)
    {
        LineRenderer rope = Instantiate(ropePrefab);
        rope.positionCount = 2;
        rope.SetPosition(0, transform.position);
        rope.SetPosition(1, enemy.position);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stunRadius);
    }
}
