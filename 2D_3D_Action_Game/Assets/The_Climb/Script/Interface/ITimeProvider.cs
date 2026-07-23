using System;

//  時間系提供インターフェース(設定価以外)
public interface ITimeProvider
{
    //  現在時間取得プロパティ
    float CurrentTimeProperty { get; set; }
    //  現在日数取得プロパティ
    int CurrentDayProperty { get; }
    //  夜かどうかのプロパティ
    bool IsNightProperty { get; set; }
    //  夜になった時の処理
    event Action<bool> OnChangedNight;
    //  時間加速ラッパー関数
    void StartTimeAcceleration(float TargetTime, float Duration);
}
