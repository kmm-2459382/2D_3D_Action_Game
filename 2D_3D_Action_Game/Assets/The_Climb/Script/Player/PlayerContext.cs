using UnityEngine;

namespace TheClimb.Player
{
    [DefaultExecutionOrder(-100)]
    public class PlayerContext : MonoBehaviour    //  プレイヤーコンテキスト
    {
        public Transform playerTransform;    //  プレイヤートランスフォーム
        public Rigidbody playerRigidbody;    //  プレイヤーリジッドボディ

        public static PlayerContext Instance { get; private set; }              //  プロパティ
        public PlayerMove _PlayerMove{ get; private set; }          //  プロパティ
        public PlayerState _PlayerState{ get; private set; }    //  プレイヤーStateインスタンス
        
        public IPlayerDataProvider _PlayerDataProvider { get; private set; }    //  プロパティ

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _PlayerDataProvider = new PlayerDataProvider(playerTransform, playerRigidbody);
        }

        public void RegistPlayerMove(PlayerMove playerMove)    //  コントローラー登録メソッド
        {
            _PlayerMove = playerMove;
        }
        public void RegistPlayerState(PlayerState playerState)    //  コントローラー登録メソッド
        {
            _PlayerState = playerState;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}