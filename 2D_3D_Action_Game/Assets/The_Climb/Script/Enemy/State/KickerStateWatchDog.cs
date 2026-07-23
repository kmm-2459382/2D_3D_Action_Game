using UnityEngine;
using System.Collections;

[RequireComponent(typeof(KickerMoveCommander))]
//  キッカー状態異常を監視・修正
public class KickerStateWatchDog : MonoBehaviour
{
    KickerMoveCommander kickerMoveCommander;    //  キッカームーブコマンダーのインスタンス
    EnemyStateMachine enemyStateMachine;    //  エネミーステートマシーンのインスタンス
    Coroutine JumpMonitaringCoroutines;    //  ジャンプ監視ルーチンの変数

    ILandingHandler KickerLandingHandler;    //  キッカーのランディングハンドラー

    [Header("ジャンプ状態に入って、接地判定するまでのディレイ")]
    [SerializeField] float AbnormalJumpStateMonitaringTime;    //  異常なジャンプ状態を監視する時間

    void Awake()
    {
        kickerMoveCommander = GetComponent<KickerMoveCommander>();
        KickerLandingHandler = kickerMoveCommander;
    }
    void Start()
    {
        enemyStateMachine = kickerMoveCommander.EnemyStateMachineProperty;
        enemyStateMachine.OnStateChanged += StartStateMonitaring;
    }
    //  ジャンプ状態を監視・修正する処理
    IEnumerator JumpStateWatch()
    {
        yield return new WaitForSeconds(AbnormalJumpStateMonitaringTime);
        if (enemyStateMachine.CurrentStateProperty is JumpState && kickerMoveCommander.IsGround())
        {
            Debug.Log("異常なジャンプ状態を検知、状態を歩行状態に変更");
            KickerLandingHandler.OnLandStage();
        }
    }
    //  状態監視を開始する
    void StartStateMonitaring(IEnemyState newState)
    {
        if (newState is JumpState)
        {
            if (JumpMonitaringCoroutines != null)
            {
                StopCoroutine(JumpMonitaringCoroutines);
            }
            JumpMonitaringCoroutines = StartCoroutine(JumpStateWatch());
        }
    }
    //  破壊時の処理
    void OnDestroy()
    {
        enemyStateMachine.OnStateChanged -= StartStateMonitaring;
        if (JumpMonitaringCoroutines != null)
        {
            StopCoroutine(JumpMonitaringCoroutines);
        }
    }
}
