using UnityEngine;

//  闇のステータスブロック
[System.Serializable]
[CreateAssetMenu(fileName = "FadeSetting", menuName = "GameDate/Effect/FadeSetting")]
public class FadeSetting : ScriptableObject
{
    [Header("フェードするマテリアル")]
    public Material OverLayMaterial;    //  四隅から進行してくる闇のマテリアル
    [Header("フェード進行値")]
    [Range(0, 1)]
    public float Progress;    //  闇進行度
    [Header("秒間の進行割合")]
    public float FadeProgressRate_Sec; //  秒間の闇進行速度(割合値)
}
