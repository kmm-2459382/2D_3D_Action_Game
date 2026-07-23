using UnityEngine;

namespace TheClimb.Item
{
    [System.Serializable]
    public class ImpactBallStatusBlock    //  インパクトボールのステータスブロック
    {
        [Header("秒数系")]
        public float ExplosionCount;       //  爆発までの秒数
        public float ExplosionDuration;    //  衝撃波の発生継続時間

        [Header("衝撃波の威力")]
        public float InpactForce;    //  衝撃波の強さ
    }
}