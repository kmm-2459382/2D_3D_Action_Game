using UnityEngine;

namespace TheClimb.Core
{
    public class EffectSystemBootstrap : MonoBehaviour
    {
        [Header("エフェクトカタログ")]
        [SerializeField] private EffectCatalog catalog;

        EffectSystem effectSystem;

        private void Awake()
        {
            {
                //effectSystem = new EffectSystem(catalog);
            }
        }
        private void Start()
        {
            ServiceLocator.Register<IEffectSystem>(effectSystem);    // ServiceLocator に自分を登録
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<IEffectSystem>(effectSystem);
        }

    }
}