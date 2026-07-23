using UnityEngine;

namespace TheClimb.Item
{
    public class ItemStateContext : IItemStateContext
    {
        readonly ItemStateMachine _stateMachine;    //  ステートマシーン
        readonly Transform _mainBodyTF;             //  本体トランスフォーム

        readonly ICommandBus _ItemCommandBus;               //  ItemCommadBus
        readonly IItemStateFactory _ItemStateFactory;       //  StateFactory

        public ItemStateContext(ItemStateMachine stateMachine, Transform mainBodyTF, IItemStateFactory stateFactory)    //  コンストラクタ
        {
            _stateMachine = stateMachine;
            _mainBodyTF = mainBodyTF;
            _ItemStateFactory = stateFactory;
        }
        
        public void NotityCountStart()    //  アイテム作動までのカウントスタート
        {
            ItemEventBus.OnAttractingStart(_mainBodyTF);
        }
        public void NotityExplosionStart()    //  衝撃炸裂スタート
        {
            ItemEventBus.OnOverExplosionTimer(_mainBodyTF);
        }
    }
}
