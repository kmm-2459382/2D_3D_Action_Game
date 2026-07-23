using UnityEngine;

namespace TheClimb.Core
{
    public abstract class PlanetAbilityStatsBase : ScriptableObject
    {
        public abstract float PrimaryEffectSpawnTime { get; }
        public abstract float SecondaryEffectSpawnTime { get; }
        public abstract float ChargeEffectDelay { get; }
        public abstract float ChargeCompleteTime { get; }
        public virtual float RepulsiveFouce { get; }
    }
}