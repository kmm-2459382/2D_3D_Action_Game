using UnityEngine;
using TheClimb.Player;

namespace TheClimb.Astral
{
    public class VectorToPlanetCalculator    //  天体までのベクトルを計算
    {
        private readonly IPlanetDataProvider _planetDataProvider;    //  天体のデータプロバイダー
        private readonly IPlayerDataProvider _playerDataProvider;    //  プレイヤーのデータプロバイダー

        public VectorToPlanetCalculator(IPlanetDataProvider planetDataProvider, IPlayerDataProvider playerDataProvider)    //  コンストラクタ
        {
            _planetDataProvider = planetDataProvider;
            _playerDataProvider = playerDataProvider;            
        }
        public Vector3 CaluclateVaector()    //  天体までのベクトルを返す
        {
            return _planetDataProvider.PositionProperty - _playerDataProvider.PositionProperty;
        }
    }
}