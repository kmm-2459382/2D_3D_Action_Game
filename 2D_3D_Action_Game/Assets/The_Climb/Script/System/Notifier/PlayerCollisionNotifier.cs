using UnityEngine;

//  プレイヤーへの衝突を通知
public class PlayerCollisionNotifier : CollisionNotifier<IBlowable>
{
    void OnCollisionEnter(Collision collision)
    {
        if(collision.rigidbody == null)
        {
            return;
        }

        var playerObj = ObjectRegistry.Get("Player_Spine_c0c99d2d");

        if (playerObj == null)
        {
            return;
        }

        float Direction = Mathf.Sign(playerObj.transform.position.x - transform.position.x);    //  吹き飛ばし方向
        //  壁に当たった時の処理を実行
        NotifyIfTagMatches(collision, TagName.Player, h => h.Blow(collision.rigidbody, Direction));
    }
}
//  以下コード保存所 // 
//Vector3 Direction = ObjectRegistry.Get("PlayerSkin_Shirt_28fe1bb6").transform.position - transform.position;    //  吹き飛ばし方向
