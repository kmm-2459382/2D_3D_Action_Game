using UnityEngine;

namespace TheClimb.Astral
{
    [System.Serializable]
    public class GravitationStatusBlock    //  天体ステータスブロック
    {
        [Header("万有引力")]
        [Tooltip("質量(引き寄せ力)")]
        public float Mass;             //  質量(引き寄せ力)
        [Tooltip("引き寄せ半径")]
        public float AttractRange;     //  引き寄せ半径
        [Tooltip("自転速度")]
        public float RotationSpeed;    //  天体自転速度
    }
}