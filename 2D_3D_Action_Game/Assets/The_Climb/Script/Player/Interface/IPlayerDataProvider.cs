using UnityEngine;

namespace TheClimb.Player
{
    public interface IPlayerDataProvider    //  プレイヤーのデータを提供するプロパティ
    {
        Transform TransformProperty { get; }    //  トランスフォームプロパティ
        Rigidbody RigidbodyProperty { get; }    //  リジッドボディプロパティ
        Vector3 PositionProperty { get; }    //  ポジションプロパティ
    }
}