using UnityEngine;

namespace TheClimb.Item
{
    public abstract class ItemCommandBase : IItemCommand    //  アイテムコマンドパターン基底クラス
    {
        protected ItemCommandBase()
        {

        }

        public abstract void Execute();
        public virtual void Undo() { }
    }
}