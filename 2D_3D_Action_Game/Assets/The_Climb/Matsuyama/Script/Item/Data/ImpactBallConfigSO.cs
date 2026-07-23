using UnityEngine;

namespace TheClimb.Item
{
    [CreateAssetMenu(fileName = "ImpactBallConfigSO", menuName = "Config/Item/ImpactBall")]
    public class ImpactBallConfigSO : ScriptableObject    //  衝撃球の設定
    {
        [Header("衝撃球エフェクト")]

        [Tooltip("爆発カウント中のエフェクト")]
        GameObject FusingEffect;

        [Tooltip("爆発エフェクト")]
        GameObject ExplosionEffect;

        [Header("衝撃球挙動設定")]
        [Tooltip("爆発までの時間")]
        public float FuseTime;           //  爆発時間(初期値コピー用)

        [Tooltip("爆発持続時間")]
        public float ExplosionDuration;      //  爆発持続時間(初期値コピー用)
        
        [Tooltip("爆発の威力")]
        public float ExplosionForce;     //  爆発半径

        [Tooltip("爆発の半径")]
        public float ExplosionRadius;    //  爆発半径
    }
}