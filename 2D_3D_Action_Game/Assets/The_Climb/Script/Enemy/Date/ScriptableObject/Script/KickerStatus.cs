using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "KickerStat", menuName = "GameDate/Enemy/KickerStat")]
//  キッカーのステータス
public class KickerStatus : ScriptableObject
{
    //  敵の状態とステータスをもつクラス
    [System.Serializable]
    public class StateStatPair
    {
        public EnemyMode enemyMode;    //  敵の状態(通常時と狂暴化)
        public KickerStatBlock Stats;    //  ステータスを持つクラス
    }

    [Header("Kicker Status")]
    public List<StateStatPair> StateStats = new();    //  状態とステータスを持つクラスのリスト(データ入力用)

    Dictionary<EnemyMode, KickerStatBlock> StatMap;    //  状態とステータスの辞書(処理用)

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
            if (!StatMap.ContainsKey(pair.enemyMode))
            {
                StatMap.Add(pair.enemyMode, pair.Stats);
            }
        }
    }
    //  状態に応じたステータスの取得
    public KickerStatBlock GetStats(EnemyMode State)
    {
        return StatMap.TryGetValue(State, out KickerStatBlock stats) ? stats : null;
    }
}