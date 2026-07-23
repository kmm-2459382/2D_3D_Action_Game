using TheClimb.Core;
using UnityEngine;

namespace TheClimb.Astral
{
    [CreateAssetMenu(fileName = "PlanetAbilityStats" ,menuName = "Astral/AbilityStats")]
    public class PlanetAbilityStatus : PlanetAbilityStatsBase    //  天体の能力の能力値
    {
        public override float PrimaryEffectSpawnTime => primaryEffectSpawnTime;
        public override float SecondaryEffectSpawnTime => secondaryEffectSpawnTime;
        public override float ChargeEffectDelay => chargeEffectDelay;
        public override float ChargeCompleteTime => chargeCompleteTime;
        public override float RepulsiveFouce => repulsiveFouce;

        [Header("天体のアビリティの能力値")]
        [Tooltip("入力からチャージ開始のエフェクトが出るまでの時間")]
        public float primaryEffectSpawnTime;
        [Tooltip("入力から二段階目のエフェクトが出るまでの時間")]
        public float secondaryEffectSpawnTime;
        [Tooltip("SecondaryEffectからチャージ完了エフェクトが出るまでのDelay")]
        public float chargeEffectDelay;
        [Tooltip("入力からチャージ完了するまでの時間")]
        public float chargeCompleteTime;
        [Tooltip("反発力(吹き飛ばし力)")]
        public float repulsiveFouce;
    }
}