using UnityEngine;

//  キャラクター接地判定確認
public class CharacterGroundChecker : MonoBehaviour
{
    [SerializeField] float GroundCheckDis;    //  接地判定に使う光線の長さ

    //  接地判定に使う光線の長さのゲッター
    public float GroundCheckDisProperty => GroundCheckDis;
    //  接地判定
    public bool CheckIsGround(Vector3? position = null)
    {
        //Debug.Log(GroundCheckDis);
        RaycastHit Hit;    //  光線が当たったオブジェクトの変数
        Vector3 CheckPos = position ?? this.transform.position;    //  光線を出すポジション
        if (Physics.Raycast(CheckPos, Vector3.down, out Hit, GroundCheckDis, GameLayer.ToMask(GameLayers.GROUND)))
        {
            if (Hit.collider.CompareTag(TagName.Ground) || Hit.collider.CompareTag(TagName.Platform))
            {
                return true;
            }
        }
        return false;
    }
}
//  以下コード保存所  //
//return Physics.Raycast(CheckPos, Vector3.down, GroundCheckDis, GameLayer.ToMask(GameLayers.GROUND));
