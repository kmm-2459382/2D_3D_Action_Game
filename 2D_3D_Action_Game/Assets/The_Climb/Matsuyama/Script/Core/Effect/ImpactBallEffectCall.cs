using System;

namespace TheClimb.Core
{
    public sealed class EffectCall : IEffectCall     //  下位クラスからエフェクトを呼ぶための窓口
    {
        public void Play<T>(T effectKey) where T : Enum
        {

        }
    }
}