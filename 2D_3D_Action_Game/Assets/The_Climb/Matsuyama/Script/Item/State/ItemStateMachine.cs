using UnityEngine;

namespace TheClimb.Item
{
    public class ItemStateMachine : ItemSMBase , ICommandContext    //  アイテムStateMachine
    {
        ItemStateFactory _itemStateFactory;

        IItemStateContext itemStateContext;

        public ItemStateMachine(ItemStateFactory stateFactory, ItemCommandProvider commadProvider, Transform mainBodyTF)    //  コンストラクタ
        {
            _itemStateFactory = stateFactory;

            itemStateContext = new ItemStateContext(this, mainBodyTF, stateFactory);
        }
        public override void Initialize()    //  初期化
        {
            ChangeState(_itemStateFactory.CreateState(ItemStateID.Idle), itemStateContext);    //  状態変更
        }

        public override void Update()        //  CurrentStateの常時処理を回す
        {
            base.Update();
        }
        public override void ChangeState(IItemState nextState, IItemStateContext stateContext)    //  State変更
        {
            base.ChangeState(nextState, stateContext);
        }
    }
}