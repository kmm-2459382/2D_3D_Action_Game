using UnityEngine;

namespace TheClimb.Astral
{
    [CreateAssetMenu(menuName = "Astral/AttractTargetStatus")]
    public class AttractTargetStatusBlock : ScriptableObject    //  万有引力を受けるオブジェクトのデータ
    {
        [Header("対象の分類")]
        public AttractTargetType attractTargetType;    //  ターゲットタグ
        [Header("質量")]
        public float Mass;    //  質量
    }
}