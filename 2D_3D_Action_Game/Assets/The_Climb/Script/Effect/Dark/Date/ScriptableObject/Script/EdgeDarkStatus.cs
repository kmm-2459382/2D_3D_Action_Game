using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DarkStat", menuName = "GameDate/Dark/EdgeDarkStat")]
public class EdgeDarkStatus : ScriptableObject
{
    //  階層状態とステータスを持つクラス
    [System.Serializable]
    public class StateStatPair
    {
        public FloorType floorType;    //  フロアの状態
        public EdgeDarkStatBlock EdgeDarkStats;    //  ステータスを持つクラス
    }

    [Header("EdgeDark Status")]
    public List<StateStatPair> StateStats = new();    //  フロア状態とステータスを持つクラスのリスト(データ入力用)

    Dictionary<FloorType, EdgeDarkStatBlock> StatMap;    //  フロア状態とステータスの辞書(処理用)

    void OnEnable()
    {
        //  スーテータスマップの初期化
        BuildStatMap();
    }
    //  ステータスマップ初期化
    void BuildStatMap()
    {
        StatMap = new();
        foreach (var pair in StateStats)
        {
            if (!StatMap.ContainsKey(pair.floorType))
            {
                StatMap.Add(pair.floorType, pair.EdgeDarkStats);
            }
        }
    }
    //  状態に応じたステータスの取得
    public EdgeDarkStatBlock GetStats(FloorType State)
    {
        return StatMap.TryGetValue(State, out EdgeDarkStatBlock stats) ? stats : null;
    }
}
