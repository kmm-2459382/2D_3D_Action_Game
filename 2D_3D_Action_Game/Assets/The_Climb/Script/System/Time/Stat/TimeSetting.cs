//  時間のデータ
using UnityEngine;

[CreateAssetMenu(fileName = "TimeSetting", menuName = "GameData/TimeSetting")]
public class TimeSetting : ScriptableObject
{
    [Header("初期時間")]
    public float InitializeTime;    //  初期時間
    public int InitializeDate;    //  初期日
    [Header("1日の時間")]
    public float OneDayTime;    //  1日の合計時間
    public float DayTime;    //  昼の実時間
    public float NightTime;    //  夜の実時間
    [HideInInspector] public readonly float TimeProgressValue = 10.0f;  //  一秒あたりの時間新行内
}