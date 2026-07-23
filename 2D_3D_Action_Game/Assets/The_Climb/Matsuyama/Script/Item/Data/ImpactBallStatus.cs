using System.Collections.Generic;
using UnityEngine;

namespace TheClimb.Item
{
    [CreateAssetMenu(fileName = "ImpactBallStatus", menuName = "Item/ImpactBall")]
    public class ImpactBallStatus : ScriptableObject    //  インパクトボールのステータスクラス
    {
        [System.Serializable]
        public class ImpactBallModeStatusPair    //  アイテムモードとステータスのペア
        {
            public ItemMode itemMode;    //  アイテムモード
            public ImpactBallStatusBlock statusBlock;    //  ステータス
        }

        [Header("PlanetStatus")]
        public List<ImpactBallModeStatusPair> StatusList = new();    //  ImpactBallのステータスリスト

        Dictionary<ItemMode, ImpactBallStatusBlock> StatusMap;    //  状態とステータスの辞書(処理用)

        void OnEnable()
        {
            BuildStatusMap();    //  辞書構築
        }

        void BuildStatusMap()    //  辞書を構築する
        {
            StatusMap = new();
            foreach (var pair in StatusList)
            {
                if (!StatusMap.ContainsKey(pair.itemMode))
                {
                    StatusMap.Add(pair.itemMode, pair.statusBlock);
                }
            }
        }

        public ImpactBallStatusBlock GetStatus(ItemMode mode)   //  モードに応じたステータスを取得する
        {
            return StatusMap.TryGetValue(mode, out var status) ? status : null;
        }
    }
}