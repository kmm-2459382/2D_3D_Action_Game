using UnityEngine;

namespace TheClimb.Core
{
    public sealed class EffectCallBootstrap : MonoBehaviour
    {
        EffectCall effectCall = new EffectCall();
        private void Awake()
        {
            ServiceLocator.Register<EffectCall>(effectCall);
        }
    }
}