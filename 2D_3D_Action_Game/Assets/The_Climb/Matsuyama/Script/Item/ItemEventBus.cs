using System;
using UnityEngine;

namespace TheClimb.Item
{
    public class ItemEventBus   //  アイテムのイベントバス
    {
        public static event Action<AttractEventArg> onAttractiong;      //  引き寄せられた時
        public static event Action<AttractEventArg> onExplosion;    //  爆発タイマーを超えた時

        public static void OnAttractingStart(Transform targetTF)    //  引き寄せが始まった時の関数
        {
            onAttractiong?.Invoke(new AttractEventArg
            {
                targeTransform = targetTF
            });    //  サブスク発火
        }

        public static void OnOverExplosionTimer(Transform targetTF)    //  爆発タイマーをオーバーしたとき
        {
            onExplosion?.Invoke(new AttractEventArg
            {
                targeTransform = targetTF
            });
        }
        //public static void CatchSuccess()    //  キャッチ成功時の処理
        //{
        //    OnCatchSuccess?.Invoke();
        //}
    }
}