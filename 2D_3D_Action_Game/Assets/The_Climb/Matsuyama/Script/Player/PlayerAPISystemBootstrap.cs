using TheClimb.Player;
using UnityEngine;

namespace TheClimb.Core
{
    public class PlayerAPISystemBootstrap : MonoBehaviour    //  PlayerAPISystemの初期化役
    {
        [SerializeField] Rigidbody rigidBody;    //  プレイヤーのリジッドボディ

        PlayerAPIFacadeBase apiFacade;    //  ServiceLocatorに登録する窓口クラス
        PlayerPhysicsJudge physicsJudge;    //  物理挙動関数を使っていいかを判断するクラス
        PlayerPhysics playerPhysics;    //  物理挙動関数持ちクラス


        void Awake()
        {
            playerPhysics = new PlayerPhysics(rigidBody);
            physicsJudge = new PlayerPhysicsJudge();
            apiFacade = new PlayerAPIFacade(playerPhysics, physicsJudge);
        }

        private void Start()
        {
            ServiceLocator.Register<PlayerAPIFacadeBase>(apiFacade);
        }
        private void OnDestroy()
        {
            ServiceLocator.Unregister<PlayerAPIFacadeBase>(apiFacade);
        }
    }
}