using UnityEngine;

namespace TheClimb.Player
{
    public class PlayerDataProvider : IPlayerDataProvider    //  プレイヤーの情報を提供する
    {
        private readonly Transform _playerTransform;    //  プレイヤーのトランスフォーム
        private readonly Rigidbody _playerRigidBody;    //  プレイヤーのトランスフォーム

        public PlayerDataProvider(Transform PlayerTransform, Rigidbody playerRB)    //  コンストラクタ
        {
            _playerTransform = PlayerTransform;
            _playerRigidBody = playerRB;
        }

        public Rigidbody RigidbodyProperty => _playerRigidBody;
        public Transform TransformProperty => _playerTransform;
        public Vector3 PositionProperty => _playerTransform.position;
    }
}