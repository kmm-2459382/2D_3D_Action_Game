namespace TheClimb.Item
{
    public class ItemStateFactory : ItemStateFactoryBase    //  アイテムstate
    {
        public ItemStateFactory()    //  コンストラクタ
        {
            Register(ItemStateID.Idle,       () => new InpactBallIdleState());
            Register(ItemStateID.Attracting, () => new InpactBallAttractingState());
            Register(ItemStateID.Expolosing, () => new InpactBallExplosionState());
        }

        public override IItemState CreateState(ItemStateID stateID)    //  state生成
        {
            return base.CreateState(stateID);
        }
    }
}