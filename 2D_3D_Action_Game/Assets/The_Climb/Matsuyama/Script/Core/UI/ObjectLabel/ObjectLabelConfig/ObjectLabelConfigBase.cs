using TheClimb.Item;
using UnityEngine;

namespace TheClimb.Core
{
    public abstract class ObjectLabelConfigBase : ScriptableObject    //  オブジェクトラベルの設定
    {
        public abstract ObjectKind ObjectKind { get; }    //  オブジェクトの種類
        public abstract LabelEffectType EffectType { get; }    //  ラベルのエフェクトタイプ
        public abstract Vector3 LabelOffset { get; }    //  ラベルオフセット
        public virtual float ShakeAmplitude { get; }    //  テキスト震えの強さ
        public virtual float ShakeFrequency { get; }    //  テキストが震える頻度
    }
}