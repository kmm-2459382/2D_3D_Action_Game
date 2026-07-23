namespace TheClimb.Item
{
    public interface IItemStateContext    //  ItemStateコンテキスト
    {
        void NotityCountStart();    //  アイテム作動までのカウントスタート
        void NotityExplosionStart();    //  衝撃炸裂スタート
    }
}