using TheClimb.Core;

namespace TheClimb.Item
{
    public abstract class ItemStateBase : IItemState    //  アイテムStatePatternのBaseClass
    {
        protected IItemStateContext _context { get; private set; }    //  コンテキスト
        public virtual void SetContext(IItemStateContext context)    //  コンテキスト代入
        {
            _context = context;
        }

        public virtual void OnEnter() { }          //  State突入時の処理
        public virtual void OnUpdate() { }         //  State中の処理(Update)
        public virtual void OnFixedUpdate() { }    //  State中の処理(FixedUpdate)
        public virtual void OnExit() { }           //  State退場時の処理
    }
}