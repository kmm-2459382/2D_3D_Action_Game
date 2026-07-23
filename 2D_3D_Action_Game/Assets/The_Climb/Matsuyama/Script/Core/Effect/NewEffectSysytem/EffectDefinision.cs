using UnityEngine;

namespace TheClimb.Core
{
    [CreateAssetMenu(fileName = "EffectDefinition", menuName = "Effect/Definition")]
    public class EffectDefinition : ScriptableObject    //  エフェクト定義SO
    {
        [Header("エフェクトの鍵")]
        public EffectKey effectKey;    //  エフェクトの鍵
        [Header("再生対象")]
        public GameObject prefab;    //  再生するエフェクト
    }
}