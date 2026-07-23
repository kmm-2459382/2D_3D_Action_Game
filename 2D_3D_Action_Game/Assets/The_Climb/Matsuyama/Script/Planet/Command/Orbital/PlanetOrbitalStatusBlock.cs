using UnityEngine;

namespace TheClimb.Astral
{
    [System.Serializable]
    public class OrbitalStatusBlock    //  天体の追従軌道のステータス
    {
        [Header("追従軌道")]
        public float OrbitRadius;     //  天体軌道円の半径
        public float HeightOffset;    //  天体の浮遊する位置
        public int OrbitalSamples;    //  円を何個の点で表現するか

        [Header("追従のスムーズさ")]
        public AnimationCurve orbitEasing = AnimationCurve.EaseInOut(0, 0, 1, 1);    //  回転・追従時の速度変化を時間に応じて補間するためのカーブ。

        public float SmoothTime;      //  追従の滑らかさ
        public float AngularSpeed;    // ラジアン毎秒
        public float OrbitSpeed;      //  軌道進行速度
        public float Duration;        //  目標位置到達秒数

        [Header("軌道の揺らぎ")]
        public float NoiseAmount;    //  軌道の揺らぎ
        public float NoiseFrequencyf;    //  揺らぎの影響頻度
    }
}