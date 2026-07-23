using UnityEngine;

namespace TheClimb.Item
{
    public abstract class ItemControllerBase : MonoBehaviour    //  アイテムコントローラーのBase
    {
        public virtual float Count { get; }    //  カウントを返すプロパティ
    }
}