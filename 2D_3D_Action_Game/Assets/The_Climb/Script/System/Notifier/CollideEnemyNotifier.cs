using UnityEngine;

//  敵と衝突したことを通知
public class CollideEnemyNotifier : CollisionNotifier<ICollideEnemy>
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody == null)
        {
            return;
        }
        //  壁に当たった時の処理を実行
        NotifyIfTagMatches(collision, TagName.Enemy, h => h.OnCollideEnemy());
    }
}
