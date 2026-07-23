using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace TheClimb.Core
{
    [CreateAssetMenu(fileName = "EffectSOCatalog", menuName = "Effect/Catalog")]
    public class EffectCatalog : ScriptableObject    //  エフェクトSOを集約してるSO
    {
        [SerializeField] List<EffectDefinition> _effects;    //  エフェクトデータリスト

        Dictionary<EffectKey, EffectDefinition> _cache;     //  エフェクトデータ辞書(内部処理用)

        //  --  Public API

        public EffectDefinition Get(EffectKey key)    //  エフェクトデーター取得
        {
            _cache ??= _effects.ToDictionary(e => e.effectKey);
            return _cache[key];
        }
    }
}