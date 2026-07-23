using UnityEngine;

//  敵キャラ移動状態
public class JumpState : IEnemyState
{
    KickerMoveCommander _kickerMoveCommander;    //  KickerMoveCommanderのインスタンス
    EnemyStateMachine _enemyStateMachine;    //  EnemyStateMachineのインスタンス
    System.Action LamdGroundListener;    //  地面着地した時の発火を待つ変数
    
    //  コンストラクタ
    public JumpState(KickerMoveCommander kickerMoveCommander, EnemyStateMachine enemyStateMachine, IEnemyStateFactory enemyStateFactory)
    {
        _kickerMoveCommander = kickerMoveCommander;
        _enemyStateMachine = enemyStateMachine;
        LamdGroundListener += () =>
        {
            _enemyStateMachine.ChangeState(enemyStateFactory.CreateWalkState());
        };
    }
    public void Enter()
    {
        _kickerMoveCommander.OnLandGround += LamdGroundListener;
        _kickerMoveCommander.CommanderMethodMap[KickerMoveCommander.KickerCommanderMethod.JUMP].Execute();
    }
    public void FixedUpdate()
    {
        //  移動
        _kickerMoveCommander.CommanderMethodMap[KickerMoveCommander.KickerCommanderMethod.MOVE].Execute();
    }
    public void Exit()
    {
        _kickerMoveCommander.OnLandGround -= LamdGroundListener;
    }
}
//  以下コード保存所  //
//  現在の移動状態を返す
//public EnemyMovingState EnemyMovingStateProperty => EnemyMovingState.JUMP_STATE;