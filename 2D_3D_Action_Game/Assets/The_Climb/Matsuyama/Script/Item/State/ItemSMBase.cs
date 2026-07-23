using TheClimb.Core;

namespace TheClimb.Item
{
    public class ItemSMBase : IStateMachine<IItemState>    //  アイテムStateMachineBaseクラス
    {
        protected IItemState currentState;    //  現在の状態

        public IItemState CurrentState => currentState;    //  現在の状態を返す

        public virtual void Initialize() { }                 //  初期化
        public virtual void Update() { }                     //  State中の処理(Update)
        public virtual void FixedUpdate() { }                //  State中の処理(FixedUpdate)
        public virtual void ChangeState(IItemState nextState, IItemStateContext stateContext)    //  状態変更
        {
            currentState?.OnExit();
            nextState.SetContext(stateContext);
            currentState = nextState;
            currentState?.OnEnter();
        }
    }
}