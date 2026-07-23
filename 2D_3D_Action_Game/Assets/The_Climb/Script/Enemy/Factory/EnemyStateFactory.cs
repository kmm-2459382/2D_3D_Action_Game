//  敵の状態を生成するファクトリー
public class EnemyStateFactory : IEnemyStateFactory
{
    readonly KickerMoveCommander _kickerMoveCommander;
    readonly EnemyStateMachine _enemyStateMachine;

    public EnemyStateFactory(KickerMoveCommander kickerMoveCommmander, EnemyStateMachine enemyStateMachine)
    {
        _kickerMoveCommander = kickerMoveCommmander;
        _enemyStateMachine = enemyStateMachine;
    }

    //  移動状態生成
    public IEnemyState CreateWalkState()
    {
        return new WalkState(_kickerMoveCommander, _enemyStateMachine, this);
    }
    
    //  ジャンプ状態生成
    public IEnemyState CreateJumpState()
    {
        return new JumpState(_kickerMoveCommander, _enemyStateMachine, this);
    }
}
