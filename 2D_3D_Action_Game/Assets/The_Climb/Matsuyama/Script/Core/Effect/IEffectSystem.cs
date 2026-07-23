using UnityEngine;

namespace TheClimb.Core
{
    public interface IEffectSystem    //  エフェクトシステムのインターフェース
    {
        void Play(EffectKey key, Vector3 pos);
        void Play(EffectKey key, Transform parent);
        void Stop(EffectKey key);
        void StopSudden(EffectKey key);    //  エフェクト停止
        public void ActiveEffect(EffectKey key, GameObject effectRoot);
    }
}