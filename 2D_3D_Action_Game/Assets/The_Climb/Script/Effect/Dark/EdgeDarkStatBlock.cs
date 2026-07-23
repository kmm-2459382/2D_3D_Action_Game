using UnityEngine;

//  闇のステータスブロック
[System.Serializable]
public class EdgeDarkStatBlock
{
    [Header("闇のマテリアル")]
    public Material OverLayMaterial;    //  四隅から進行してくる闇のマテリアル
    [Header("闇の進行値")]
    [Range(0, 1)]
    public float Progress;    //  闇進行度
    [Header("秒間の進行割合")]
    public float DarkProgressRate_Sec; //  秒間の闇進行速度(割合値)
}
