namespace TheClimb.Item
{
    public interface ICommandContext    //  コマンドコンテキスト
    {
        void ChangeState(IItemState stateID, IItemStateContext stateContext);    //  状態変更
    }
}