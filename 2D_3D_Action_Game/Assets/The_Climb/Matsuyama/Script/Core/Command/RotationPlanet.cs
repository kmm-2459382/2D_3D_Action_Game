using TheClimb.Core;
using UnityEngine;

namespace TheClimb.Astral
{
    public class RotationPlanet : PlanetCommadBase   //  天体を回転させるクラス
    {
        Transform _planetTransform;                        //  天体のトランスフォーム
        GravitationStatusBlock _gravitationStatusBlock;    //  万有引力のステータス

        float CurrentRotationSpeed;    //  天体の自転速度

        public RotationPlanet(Transform planetTF, GravitationStatusBlock stat)    //  コンストラクタ
        {
            _planetTransform = planetTF;
            _gravitationStatusBlock = stat;
            CurrentRotationSpeed = stat.RotationSpeed;
        }

        public　override void Execute()    //  処理実行
        {
            RotatePlanet();   //  自転
        }

        void RotatePlanet()    //  天体を回転させる
        {
            _planetTransform.Rotate(Vector3.up * CurrentRotationSpeed * Time.deltaTime);
        }
    }
}