namespace TheClimb.Item
{
    public interface IItemStateFactory    //  アイテムインターフェース
    {
        IItemState CreateState(ItemStateID stateID);    //  State生成
    }
}