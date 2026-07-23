using UnityEngine;

//  太陽のスクリプタブルオブジェクト設定スクリプト
[CreateAssetMenu(fileName = "SunStat", menuName = "GameData/SunStat")]
public class SunStat : ScriptableObject
{
    [Header("太陽の角度設定(X)")]
    public float InitializeAngle;    //  初期位置
    public float DayTimeStartAngle;    //  太陽が昇るときの最初のアングル
    public float DayTimeLastAngle;    //  太陽が沈むときの最後のアングル
    public float NightTimeStartAngle;    //  太陽が沈んているときの最初のアングル
    public float NightTimeLastAngle;    //  太陽が昇る前の最後のアングル
}