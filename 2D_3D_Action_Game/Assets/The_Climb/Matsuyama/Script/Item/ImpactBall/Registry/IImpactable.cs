using UnityEngine;

namespace TheClimb.Item
{
    public interface IImpactable    //  衝撃球の影響を受ける物に付ける
    {
        Transform TransformGetter { get; }    //  トランスフォームゲッター
        Rigidbody RigidbodyGetter { get; }    //  リジッドボディゲッター
    }
}