using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]

//  敵キャラクターを実際に動かすスクリプト
public class EnemyMover : MonoBehaviour
{
    Rigidbody RB;    //  リジッドボディインスタンス
    void Awake()
    {
        RB = GetComponent<Rigidbody>();
    }
    //  基本移動
    public void BaseMove(Vector3 Velocity)
    {
        RB.MovePosition(RB.position + Velocity);
    }
    //  ジャンプ
    public void Jump(float JumpForce)
    {
        Vector3 Velocity = RB.linearVelocity;
        Velocity.y = 0;
        RB.linearVelocity = Velocity;
        RB.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
    }
}
