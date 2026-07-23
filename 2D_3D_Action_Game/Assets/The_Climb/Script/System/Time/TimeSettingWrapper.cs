//  時間設定を提供できるように設定
public class TimeSettingWrapper : ITimeConfig
{
    private readonly TimeSetting timeSetting;    //  時間設定

    //  設定代入コンストラクタ
    public TimeSettingWrapper(TimeSetting _timeSetting)
    {
        timeSetting = _timeSetting;
    }

    //  初期時間プロパティ
    public float InitializeTimeProperty => timeSetting.InitializeTime;
    //  1日の総実時間プロパティ
    public float OneDayTimeProperty => timeSetting.OneDayTime;
    //  1日の日中の実時間プロパティ
    public float DayTimeProperty => timeSetting.DayTime;
    //  1日の夜の実時間プロパティ
    public float NightTimeProperty => timeSetting.NightTime;
    //  1秒あたりの進行時間
    public float ProgressTimeProperty => timeSetting.TimeProgressValue;
    //  初期日付プロパティ
    public int InitializeDateProperty => timeSetting.InitializeDate;
}