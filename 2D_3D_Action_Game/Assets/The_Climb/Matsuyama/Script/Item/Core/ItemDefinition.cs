using UnityEngine;

namespace TheClimb.Item
{
    [CreateAssetMenu(fileName = "InpactBallLabelDefinition", menuName = "Item/Label/ImpactBall Definition")]
    public class impactBallLabelDef : ScriptableObject , IItemLabelDef    //  アイテム定義
    {
        [Header("アイテムの種類")]
        [SerializeField] ObjectKind itemKind;    //  アイテムの種類(Inspector用)
        [Header("アイテムの表示位置補正")]
        [SerializeField] Vector3 LabelOffset;    //  ラベル表示位置補正

        //  --  Public Propety
        public ObjectKind ItemKind => itemKind;    //  アイテムの種類
    }
}