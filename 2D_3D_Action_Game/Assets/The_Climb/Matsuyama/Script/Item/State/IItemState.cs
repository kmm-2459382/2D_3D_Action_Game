namespace TheClimb.Item
{
    public interface IItemState    //  ItemStateようInterface
    {
        void OnEnter();     //  State突入時の処理
        void OnUpdate();    //  State中の処理(Update)
        void OnFixedUpdate();    //  State中の処理(FixedUpdate)
        void OnExit();      //  Stateを抜けるときの処理
        void SetContext(IItemStateContext context);    //  コンテキストSet
    }
}