using UnityEngine;
using TheClimb.Core;

namespace TheClimb.Item
{
    [RequireComponent(typeof(ImpactBallController))]
    public class ImpactBallBootstrap : MonoBehaviour   //  衝撃球Bootstrap
    {
        [SerializeField] ImpactBallConfigSO _configSO;    //  衝撃球コンフィグ
        //[SerializeField] CoroutineRunner _coroutineRunner;    //  コルーチンランナー
        [SerializeField] GameObject explosionEffectPrefab_Deprecated;    //  爆発時のエフェクト(仮実装だから削除するかも)
        [SerializeField] GameObject sparkingEffect;    //  ビリビリしてるエフェクト(仮実装だから削除するかも)
        [SerializeField] GameObject impactBallRoot;    //  衝撃球Root(仮実装だから削除するかも)

        ImpactBallRuntimeData _runtimeData;    //  実行中のデータ

        void Start()
        {
            _runtimeData = new ImpactBallRuntimeData(_configSO);

            ImpactBallContext context = new ImpactBallContext(
                _configSO,
                _runtimeData,
                this.transform,
                this.GetComponent<Rigidbody>()
                );

            GetComponent<ImpactBallController>().Initialize(context, explosionEffectPrefab_Deprecated, sparkingEffect, impactBallRoot);
        }
    }
}