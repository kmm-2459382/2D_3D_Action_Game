using UnityEngine;

namespace TheClimb.Item
{
    public class ImpactBallContext
    {
        public ImpactBallConfigSO ConfigSO { get; }
        public ImpactBallRuntimeData RuntimeData { get; }
        public Transform Transform { get; }
        public Rigidbody Rigidbody { get; }

        public IEffectSpawner EffectSpawner { get; }
        public ImpactBallContext(ImpactBallConfigSO configSO, ImpactBallRuntimeData runtimeData, Transform transform, Rigidbody rigidbody)
        {
            ConfigSO = configSO;
            RuntimeData = runtimeData;
            Transform = transform;
            Rigidbody = rigidbody;
        }
    }
}