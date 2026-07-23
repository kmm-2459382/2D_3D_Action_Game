//  敵キャラ状態生成インターフェース
public interface IEnemyStateFactory
{
    //  移動状態生成
    IEnemyState CreateWalkState();
    //  ジャンプ状態生成
    IEnemyState CreateJumpState();
}
