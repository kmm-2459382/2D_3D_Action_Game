using UnityEngine;
using TheClimb.Astral;
using TMPro;

namespace TheClimb.Core
{
    public class ObjectLabelContext    //  オブジェクトラベルコンテキスト
    {

        //  --  外部API
        
        public ObjectLabelContext(Transform mainCamTF, Transform impactBallTF, AttractableListenerBase itemController, TextMeshPro label)
        {
            MainCameraTF = mainCamTF;
            ImpactBallTF = impactBallTF;
            ItemController = itemController;
            ObjectLabel = label;
        }

        public Transform MainCameraTF { get; private set; }    //  メインカメラのトランスフォームプロパティ。
        public Transform ImpactBallTF { get; private set; }    //  衝撃球のトランスフォームプロパティ。
        public AttractableListenerBase ItemController { get; private set; }    //  アイテムコントローラー。
        public TextMeshPro ObjectLabel { get; private set; }    //  アイテムコントローラー。
    }
}