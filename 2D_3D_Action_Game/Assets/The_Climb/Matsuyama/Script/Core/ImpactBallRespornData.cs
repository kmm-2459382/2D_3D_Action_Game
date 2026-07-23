using UnityEngine;

namespace TheClimb.Astral
{
    [System.Serializable]
    public class ImpactBallRespornData    //  プレイヤーのリスポーン時に衝撃球をリセットするための生成情報(納期に間に合わせるため、破壊して再生成する事でリセットする)
    {
        [Header("生成する衝撃球")] 
        public GameObject ImpactBall;
        [Header("生成する座標")]
        public Vector3 GenratePosition;
    }
}