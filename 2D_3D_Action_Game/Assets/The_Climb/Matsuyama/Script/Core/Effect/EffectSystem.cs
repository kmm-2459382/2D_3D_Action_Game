using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheClimb.Core
{
    public class EffectSystem : MonoBehaviour, IEffectSystem    //  エフェクト処理システム
    {
        [Header("エフェクトカタログ")]
        [SerializeField] private EffectCatalog catalog;

        private readonly Dictionary<EffectKey, GameObject> playingEffects = new();

        private void Awake()
        {
            ServiceLocator.Register<IEffectSystem>(this);    // ServiceLocator に自分を登録
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<IEffectSystem>(this);
        }

        //  --  Public API

        public void Play(EffectKey key, Vector3 position)    //  エフェクト再生
        {
            // 定義解決
            var definition = catalog.Get(key);
            if (definition == null)
            {
                Debug.LogWarning($"EffectDefinition not found : {key}");
                return;
            }

            Stop(key);    //  エフェクトを一回再生終了する

            var instance = Instantiate(definition.prefab, position, definition.prefab.transform.rotation);

            playingEffects[key] = instance;
        }

        public void Play(EffectKey key, Transform parent)    //  エフェクト再生(親追従)
        {
            // 定義解決
            var definition = catalog.Get(key);
            if (definition == null)
            {
                Debug.LogWarning($"EffectDefinition not found : {key}");
                return;
            }

            Stop(key);    //  エフェクトを一回再生終了する

            var instance = Instantiate(definition.prefab, parent);

            playingEffects[key] = instance;
        }

        public async void Stop(EffectKey key)    //  エフェクト停止
        {
            if (!playingEffects.TryGetValue(key, out var instance))
                return;

            var ps = instance.GetComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            

            await WaitUntillDead(ps);

            Destroy(instance);
            playingEffects.Remove(key);
        }
        public void StopSudden(EffectKey key)    //  エフェクト停止
        {
            if (!playingEffects.TryGetValue(key, out var instance))
                return;

            var trails = instance.GetComponents<TrailRenderer>();

            for (int i = 0; i < trails.Length; i++)
            {
                trails[i].emitting = false;
            }

            Destroy(instance);
            playingEffects.Remove(key);
        }

        public void ActiveEffect(EffectKey key, GameObject effectRoot)    //  渡されたEffectをアクティブにする
        {
            if(!effectRoot.activeSelf)
            {
                effectRoot.SetActive(true);
            }
            playingEffects[key] = effectRoot;
        }

        //  --  Private method

        async Task WaitUntillDead(ParticleSystem ps)    //  エフェクトが再生終了するまで待つ
        {
            while (ps != null && ps.IsAlive(true))
            {
                await Task.Yield();
            }
        }
    }
}