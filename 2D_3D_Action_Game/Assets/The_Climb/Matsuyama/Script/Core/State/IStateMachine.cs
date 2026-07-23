using TheClimb.Item;

namespace TheClimb.Core
{
    public interface IStateMachine<T>    //  インターフェース
    {
        void Initialize() { }    //  初期化
        void Update();    //  State中の処理(Update)
        void  FixedUpdate();    //  State中の処理(FixedUpdate)
        void ChangeState(T stateID, IItemStateContext stateContext);    //  状態変更
    }
}