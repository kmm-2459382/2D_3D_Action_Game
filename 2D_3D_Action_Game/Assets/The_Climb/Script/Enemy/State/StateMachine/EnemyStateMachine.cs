using System;

//  敵キャラのステート管理
public class EnemyStateMachine
{
    public event Action<IEnemyState> OnStateChanged;

    IEnemyState CurrentEnemyState;    //  敵キャラの現在のステート

    //  現在の状態を返すプロパティ
    public IEnemyState CurrentStateProperty => CurrentEnemyState;
    
    //  状態変更関数
    public void ChangeState(IEnemyState newState)
    {
        CurrentEnemyState?.Exit();
        CurrentEnemyState = newState;
        CurrentEnemyState?.Enter();

        OnStateChanged?.Invoke(CurrentEnemyState);
    }
    //  ステートごとの状態実行
    public void FixedUpdate()
    {
        CurrentEnemyState?.FixedUpdate();
    }
}
