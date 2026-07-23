using System.Collections.Generic;
using UnityEngine;

namespace TheClimb.Astral
{
    [CreateAssetMenu(fileName = "PlanetStatus", menuName = "Planet/Status")]
    public class PlanetStatus : ScriptableObject    //  天体の大本ステータス
    {
        //  インスペクター表示用のステータスクラス
        [System.Serializable]
        public class PlanetIDStatusPair    //  ステータスブロックが増えてきたらブロックをまとめたクラス作成
        {
            public PlanetIDs CustomerName;                 //  敵の状態(通常時と狂暴化)
            public GravitationStatusBlock gravitationStatus;    //  質量などのステータスクラス
            public OrbitalStatusBlock orbitalStatus;       //  円軌道のステータスクラス
        }

        [Header("PlanetStatus")]
        public List<PlanetIDStatusPair> planetIDStatus = new();    //  インスペクター用ステータスリスト
        Dictionary<PlanetIDs, (GravitationStatusBlock gravitationStatus, OrbitalStatusBlock orbitalStatus) > StatusMap;    //  処理用ステータス辞書

        //  --  UnityLifeCycle

        void OnEnable()
        {
            //  インスペクター用のリストから処理用の辞書を構築
            BuildStatMap();
        }
        
        // --  Private method

        void BuildStatMap()    //  インスペクター用リストからkvpを取り出して処理用の辞書に入れる
        {
            StatusMap = new();
            foreach (var pair in planetIDStatus)
            {
                if (!StatusMap.ContainsKey(pair.CustomerName))
                {
                    StatusMap.Add(pair.CustomerName, (pair.gravitationStatus, pair.orbitalStatus));
                }
            }
        }

        // --  Public method

        public GravitationStatusBlock GetGraviatationStatus(PlanetIDs PlanetID)    //  IDに応じた万有引力ステータスの取得
        {
            return StatusMap.TryGetValue(PlanetID, out var data) ? data.gravitationStatus : null;
        }

        public OrbitalStatusBlock GetOrbitalStatus(PlanetIDs PlanetID)    //  IDに応じた軌道スタータスの取得
        {
            return StatusMap.TryGetValue(PlanetID, out var data) ? data.orbitalStatus : null;
        }

        public (GravitationStatusBlock gravitationStatus, OrbitalStatusBlock orbitalStatus)? GetFullStatus(PlanetIDs id)    //  IDに応じたすべてのステータスを保持
        {
            return StatusMap.TryGetValue(id, out var data) ? data : null;
        }
    }
}