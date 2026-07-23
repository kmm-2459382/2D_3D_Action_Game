using UnityEngine;

namespace TheClimb.Astral
{
    public abstract class AttractableBase : MonoBehaviour, IAttractable    //  引き寄せ用マーカーコンポーネントの基底クラス
    {
        protected AttractTargetStateID curretStateID; 

        //  --  Public API

        public abstract AttractTargetStatusBlock statProperty { get; }    //  引き寄せられるアイテムのステータス取得
        public abstract AttractTargetStateID currentStateIDProperty { get; }   //  現在状態State取得

        public virtual void OnAttract()    //  引き寄せがスタートした瞬間の処理
        {
            curretStateID = AttractTargetStateID.Attracting;
        }
    }
}