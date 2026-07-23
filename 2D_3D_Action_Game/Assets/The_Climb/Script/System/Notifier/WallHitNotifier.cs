using UnityEngine;

//  壁への衝突を通知
public class WallHitNotifier : CollisionNotifier<IWallHitTable>
{
    void OnCollisionEnter(Collision collision)
    {
        //  壁に当たった時の処理を実行
        NotifyIfTagMatches(collision, TagName.Wall, h => h.OnHitWall());
    }
}
