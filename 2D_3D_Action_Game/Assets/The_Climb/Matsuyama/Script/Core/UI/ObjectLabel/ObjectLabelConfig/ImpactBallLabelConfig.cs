using UnityEngine;
using TheClimb.Core;

namespace TheClimb.Item
{
    [CreateAssetMenu(fileName = "LabelConfig", menuName = "Label/Config/ImpactBall")]
    public class ImpactBallLabelConfig : ObjectLabelConfigBase    //  インパクトボールコンフィグ
    {
        [Header("オブジェクトの種類")]
        [SerializeField] ObjectKind _obejctKind;
        [Header("オブジェクトのエフェクトの種類")]
        [SerializeField] LabelEffectType _effectType;
        [Header("ラベルを表示するオフセット")]
        [SerializeField] Vector3 _labelOffest;

        [Header("テキスト震え設定")]
        [Tooltip("震えの強さ")]
        [SerializeField] float _shakeAmplitude;
        [Tooltip("震えの頻度")]
        [SerializeField] float _shakeFrequency;

        //  --  Public API

        public override ObjectKind ObjectKind => _obejctKind;    //  オブジェクトの種類提供
        public override LabelEffectType EffectType => _effectType;    //  オブジェクトの種類提供
        public override Vector3 LabelOffset => _labelOffest;    //  ラベルオフセット提供    
        public override float ShakeAmplitude => _shakeAmplitude;    //  震え強さ
        public override float ShakeFrequency => _shakeFrequency;    //  震え頻度
    }
}