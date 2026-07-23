using System;

namespace TheClimb.Core
{
    public interface IEffectCall    //  下位クラス専用のエフェクトコール<T>を束ねるクラス
    {
        void Play<T>(T effectKey) where T : Enum ;    //  判断層を通ってエフェクトを発生させる
    }
}