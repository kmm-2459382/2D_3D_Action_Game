//  敵の状態のインターフェイス
public interface IEnemyState
{
    void Enter();
    void FixedUpdate();
    void Exit();
}
//  以下コード保存所  //
//EnemyMovingState EnemyMovingStateProperty { get; }
