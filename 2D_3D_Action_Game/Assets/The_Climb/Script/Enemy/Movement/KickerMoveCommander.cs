using Zenject;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using TheClimb.Player;
using UnityEngine.Playables;

[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]
[assembly: InternalsVisibleTo("Assembly-CSharp-Tests")]
[RequireComponent(typeof(EnemyMover))]
[RequireComponent(typeof(CharacterGroundChecker))]
[RequireComponent(typeof(LandGroundNotifier))]
[RequireComponent(typeof(CollideEnemyNotifier))]
[RequireComponent(typeof(PlayerCollisionNotifier))]
[RequireComponent(typeof(CharacterStateVisualizer))]
//  移動スクリプトに移動値を渡す
public class KickerMoveCommander : MonoBehaviour, IWallHitTable, ILandingHandler, IBlowable, ICollideEnemy
{
    public enum KickerCommanderMethod    //  このクラス内の関数一覧
    {
        MOVE,    //  基礎移動
        IS_EDGE_POS,    //  端か判定
        FLIP_MOVE_DIR,    //  移動方向反転
        JUMP,    //  ジャンプ
    }
    
    [Header("Instance")]
    [SerializeField] [Inject] internal KickerStatus kickerStatus;    //  キッカーステータス
    KickerStatBlock kickerStatBlock;    //  キッカーステータスクラス
    EnemyMover enemyMover;    //  エネミームーバー
    CharacterGroundChecker characterGroundChecker;    //  グラウンドチェッカー
    EnemyStateMachine enemyStateMachine;    //  エネミーステートマシーン
    PlayerState playerState;    //  プレイヤーステート
    CharacterStateVisualizer characterStateVisualizer;    //  キャラクターステートビジュアライザー
    public Dictionary<KickerCommanderMethod, ICommand_Enemy> CommanderMethodMap;    //  このスクリプトの関数の辞書
    public event Action OnJumpTime;    //  ジャンプタイムのサブスク
    public event Action OnLandGround;    //  地面着地のサブスク
    Coroutine JumpLoop;    //  ジャンプループコルーチンの変数

    ICommandProvider commandProvider;    //  インターフェース型変数
    IEnemyStateFactory enemyStateFactory;    //  インターフェース型変数
    ITimeProvider TimeProvider;    //  時間値提供インターフェース
    IDownFading DownFading;    //  ダウンフェードインターフェース
    EnemyMode CurrentEnemyMode;    //  現在の敵状態　

    Vector3 Velocity;    //  キャラクター移動値
    Vector3 EdgeRayOffset;    //  端を検知するRayのオフセット

    [Header("Move Value")]
    [SerializeField] MoveDir CurrentMoveDir;    //  現在の動く方向(X軸)
    float CurrentMoveSpd;    //  現在の移動速度
    float CurrentJumpForce;    //  現在のジャンプ力
    float CurrentJumpFrequency;    //  現在のジャンプ頻度

    //  インジェクトによる初期化
    [Inject]
    void Construct(
        ITimeProvider TimeProvider,
        IDownFading DownFading
        )
    {
        this.TimeProvider = TimeProvider;
        this.DownFading = DownFading;
        TimeProvider.OnChangedNight += ChangeToNightMode;
    }

    public EnemyStateMachine EnemyStateMachineProperty => enemyStateMachine;
    void Awake()
    {
        if (kickerStatus == null)
        {
            Debug.LogWarning($"{nameof(KickerMoveCommander)} : kickerStatus is not assigned");
            return;
        }

        enemyMover = GetComponent<EnemyMover>();
        kickerStatBlock = kickerStatus.GetStats(EnemyMode.NORMAL);
        characterGroundChecker = GetComponent<CharacterGroundChecker>();
        enemyStateMachine = new EnemyStateMachine();
        //playerState = GameObject.FindAnyObjectByType<PlayerState>();
        commandProvider = new DefaultCommandProvider(this);
        enemyStateFactory = new EnemyStateFactory(this, enemyStateMachine);
        CurrentEnemyMode = EnemyMode.NORMAL;
        characterStateVisualizer = GetComponent<CharacterStateVisualizer>();

        //  初期状態をWalkに変更
        CommanderMethodMap =commandProvider.GetCommandMap();
        enemyStateMachine.ChangeState(enemyStateFactory.CreateWalkState());
        
        //  数値初期化
        Initialize();
    }
    //  テストのための初期化
    internal void InitializeForTest(KickerStatus status)
    {
        enemyMover = GetComponent<EnemyMover>();
        this.kickerStatBlock = status.GetStats(EnemyMode.NORMAL);
        characterGroundChecker = GetComponent<CharacterGroundChecker>();
        enemyStateMachine = new EnemyStateMachine();
        playerState = GameObject.FindAnyObjectByType<PlayerState>();
        commandProvider = new DefaultCommandProvider(this);
        enemyStateFactory = new EnemyStateFactory(this, enemyStateMachine);
        CurrentEnemyMode = EnemyMode.NORMAL;
        characterStateVisualizer = GetComponent<CharacterStateVisualizer>();


        //  初期状態をWalkに変更
        CommanderMethodMap = commandProvider.GetCommandMap();
        enemyStateMachine.ChangeState(enemyStateFactory.CreateWalkState());

        //  数値初期化
        Initialize();
    }

    void Start()
    {
        playerState = PlayerContext.Instance._PlayerState;
        //  定期的なジャンプのループを開始
        JumpLoop = StartCoroutine(PeriodicallyJump());
    }
    void FixedUpdate()
    {
        Debug.DrawRay(transform.position, Vector3.down * characterGroundChecker.GroundCheckDisProperty, Color.red);
        Debug.DrawRay(EdgeRayOffset + transform.position, Vector3.down * characterGroundChecker.GroundCheckDisProperty, Color.pink);
        enemyStateMachine.FixedUpdate();
        characterStateVisualizer.MoveDirectionPropety(Velocity);
    }
    //  初期化
    void Initialize()
    {
        EdgeRayOffset = kickerStatBlock.EdgeRayOffset;
        if(CurrentMoveDir != MoveDir.NONE && Mathf.Sign(EdgeRayOffset.x) != Mathf.Sign((int)CurrentMoveDir))
        {
            Debug.LogWarning($"CurrentMoveDir : {CurrentMoveDir} and EdgeRayOffset direction of x :{EdgeRayOffset.x} is not same direction.{Environment.NewLine}  Fix to CurrentMoveDirection");
            EdgeRayOffset *= (int)MoveDir.LEFT;
        }

        CurrentMoveSpd = kickerStatBlock.MoveSpd;
        CurrentJumpForce = kickerStatBlock.JumpForce;
        CurrentJumpFrequency = kickerStatBlock.JumpFrequency;
    }
    //  定期的なジャンプ発火ループ
    IEnumerator PeriodicallyJump()
    {
        while (true)
        {
            yield return new WaitForSeconds(CurrentJumpFrequency);
            if (characterGroundChecker.CheckIsGround())
            {
                OnJumpTime?.Invoke();
            }
        }
    }
    //  移動
    public void Move()
    {
        Velocity = new Vector3(CurrentMoveSpd * (int)CurrentMoveDir * Time.fixedDeltaTime, Velocity.y, 0f);
        //  基本移動
        enemyMover.BaseMove(Velocity);
    }
    //  地面か判定する
    public bool IsGround()
    {
        return characterGroundChecker.CheckIsGround();
    }
    //  端か判定する
    public bool IsEdgePos()
    {
        return !characterGroundChecker.CheckIsGround(EdgeRayOffset + this.transform.position) &&
                characterGroundChecker.CheckIsGround();
    }
    //  移動方向反転
    public void FlipMoveDir()
    {
        CurrentMoveDir = CurrentMoveDir switch
        {
            MoveDir.RIGHT => MoveDir.LEFT,
            MoveDir.LEFT => MoveDir.RIGHT,
            _ => MoveDir.NONE,
        };
        EdgeRayOffset.x *= -1f;
    }
    //  ジャンプ
    public void Jump()
    {
        enemyMover.Jump(CurrentJumpForce);
    }
    //  壁に当たった時の処理
    public void OnHitWall()
    {
        if(CurrentMoveDir != MoveDir.NONE && CurrentMoveDir == MoveDir.RIGHT)
        {
            CurrentMoveDir = MoveDir.LEFT;
        }
        else if(CurrentMoveDir == MoveDir.LEFT)
        {
            CurrentMoveDir = MoveDir.RIGHT;
        }
        EdgeRayOffset.x *= -1f;
    }
    //    ステージに着地した時の処理
    public void OnLandStage()
    {
        OnLandGround?.Invoke();
    }
    //  吹き飛ばし処理
    public void Blow(Rigidbody rigidbody, float Direction)
    {
        float CurrentBlowForceX = kickerStatBlock.BlowForceX;    //  X軸の吹き飛ばし力
        float CurrentBlowForceY = kickerStatBlock.BlowForceY;    //  Y軸の吹き飛ばし力
        float PlayerResistPowerX = 100f;    //  プレイヤーステータスに追加予定
        float EffectiveForceX = MathF.Max(0, CurrentBlowForceX - PlayerResistPowerX);    //  実際に適応される力

        ForceMode GroundBlowMode = kickerStatBlock.GroundBlowMode;    //  地上時の吹っ飛ばし方
        ForceMode AriBlowMode = kickerStatBlock.AirBlowMode;    //  空中時の吹っ飛ばし方

        Vector3 GroundTotalBlowForce = new Vector3(Direction * (EffectiveForceX), 0, 0);    //  地上時の吹き飛ばし力合計
        Vector3 AirTotalBlowForce = new Vector3(Direction * CurrentBlowForceX, Direction * CurrentBlowForceY, 0f);    //  空中時の吹き飛ばし力合計

        Debug.Log(playerState);
        if (playerState.isGrounded)
        {
            Debug.DrawRay(ObjectRegistry.Get("Player_Spine_c0c99d2d").transform.position, GroundTotalBlowForce, Color.blue, 0.5f);
            rigidbody.AddForce(GroundTotalBlowForce, ForceMode.Acceleration);
        }
        else
        {
            Debug.DrawRay(ObjectRegistry.Get("Player_Spine_c0c99d2d").transform.position, AirTotalBlowForce, Color.blue, 0.5f);
            rigidbody.AddForce(AirTotalBlowForce, ForceMode.Impulse);
        }
        FlipMoveDir();  //  移動方向反転
    }
    //  敵と当たった時に実行されるインターフェイス関数
    public void OnCollideEnemy()
    {
        FlipMoveDir();  //  移動方向反転
    }
    private void ChangeToNightMode(bool IsNight)
    {
        CurrentEnemyMode = IsNight ? EnemyMode.NIGHT : EnemyMode.NORMAL;
        
        kickerStatBlock = kickerStatus.GetStats(CurrentEnemyMode);
        CurrentMoveSpd = kickerStatBlock.MoveSpd;
        CurrentJumpForce = kickerStatBlock.JumpForce;
        CurrentJumpFrequency = kickerStatBlock.JumpFrequency;
    }
    void OnDestroy()
    {
        if(TimeProvider != null)
        {
            TimeProvider.OnChangedNight -= ChangeToNightMode;
        }
    }
}