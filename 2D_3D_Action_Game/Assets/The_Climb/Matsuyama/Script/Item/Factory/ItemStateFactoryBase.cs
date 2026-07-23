using System;
using System.Collections.Generic;

namespace TheClimb.Item
{
    public class ItemStateFactoryBase : IItemStateFactory    //  ItemState生成クラス
    {
        protected readonly Dictionary<ItemStateID, Func<IItemState>> itemStateRegistry = new();    //  State登録用辞書

        public virtual IItemState CreateState(ItemStateID state)    //  Stateを生成
        {
            if (itemStateRegistry.TryGetValue(state, out Func<IItemState> creator))
            { return creator(); }

            throw new Exception("_state is not registed");
        }

        protected void Register(ItemStateID stateID, Func<IItemState> ceator)    //  生成関数登録
        {
            itemStateRegistry[stateID] = ceator;
        }
    }
}