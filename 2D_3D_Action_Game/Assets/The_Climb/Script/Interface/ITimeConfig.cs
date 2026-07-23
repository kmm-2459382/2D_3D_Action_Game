//    時間設定提供インターフェース
public interface ITimeConfig
{
    //  1日の総実時間プロパティ
    float InitializeTimeProperty { get; }
    //  1日の総実時間プロパティ
    float OneDayTimeProperty { get; }
    //  1日の日中の実時間プロパティ
    float DayTimeProperty { get; }
    //  1日の夜の実時間プロパティ
    float NightTimeProperty { get; }
    //  1秒あたりの進行時間
    float ProgressTimeProperty { get; }
    //  1日の総実時間プロパティ
    int InitializeDateProperty { get; }
}
