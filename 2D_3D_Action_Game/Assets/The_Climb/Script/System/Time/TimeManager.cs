using System;
using System.Collections;
using UnityEngine;
using Zenject;

//  日付・時間管理スクリプト
public class TimeManager : MonoBehaviour, ITimeProvider
{
    public event Action<bool> OnChangedNight;    //  狂暴化イベント
    public Coroutine defaultTimeCount;    //  時間加速関数
    public Coroutine timeAcceleration;    //  時間加速関数

    [Inject] ITimeConfig TimeConfig;    //  時間設定

    public float CurrentTime;    //  現在の時間
    float TimeProgressValue;    //  １秒当たりの時間進行値
    int CurrentDay;    //  現在の日付
    [SerializeField] bool IsNight;    //  夜かどうかのフラグ
    bool IsStopCount;    //  タイマーを止めるか止めないか
    bool IsPlayerAttacked;    //  プレイヤーが攻撃されたか（仮フラグ）

    //    プレイヤーが攻撃されたかプロパティ
    public bool IsPlayerAttackedProperty
    {
        get => IsPlayerAttacked;
        set => IsPlayerAttacked = value;
    }

    //  現在時間取得プロパティ
    public float CurrentTimeProperty
    {
        get => CurrentTime;
        set => CurrentTime = value;
    }
    //  現在日数取得プロパティ
    public int CurrentDayProperty => CurrentDay;
    //  夜取得プロパティ
    public bool IsNightProperty
    {
        get => IsNight;
        set
        {
            if (IsNight != value)
            {
                IsNight = value;
                OnChangedNight?.Invoke(IsNight); // ← ✅ 安全に呼び出し
            }
        }
    }
    void Awake()
    {
        //  数値初期化
        InitializeValue();
    }
    void Start()
    {
        CoroutineUtility.SafeStartCoroutine(this, ref defaultTimeCount, DefaultTimeCount());
    }
    //  初期化関数
    void InitializeValue()
    {
        CurrentTime = TimeConfig.InitializeTimeProperty;
        TimeProgressValue = TimeConfig.ProgressTimeProperty;
        CurrentDay = TimeConfig.InitializeDateProperty;
        IsStopCount = false;
        IsPlayerAttacked = false;
    }
    //  時間加速ラッパー関数
    public void StartTimeAcceleration(float TargetValue, float Duration)
    {
        CoroutineUtility.SafeStopCoroutine(this, ref defaultTimeCount);
        CoroutineUtility.SafeStartCoroutine(this, ref timeAcceleration, TimeAcceleration(TargetValue, Duration));
    }
    //  デフォルト時間カウント関数
    IEnumerator DefaultTimeCount()
    {
        while (!IsStopCount)
        {
            CurrentTime += TimeProgressValue * Time.deltaTime;
            yield return null;
        }
    }
    //  時間加速関数
    public IEnumerator TimeAcceleration(float TargetTime, float Duration)
    {
        //float TimeDiference = Mathf.Abs(TargetTime - CurrentTime);    //  現在時間と目標時間の差
        float WrappedTargrtTime = TargetTime > CurrentTime ? TargetTime : TargetTime + TimeConfig.OneDayTimeProperty;
        float TimeUntilTarget = WrappedTargrtTime - CurrentTime;
        float SecProgress = TimeUntilTarget / Duration;    //  1秒あたりの進行値
        float Epsilon = 2f;

        //int Direction = (TargetTime >= CurrentTime) ? 1 : -1;    //  加算か減算の方向

        //while ((Direction == 1 && CurrentTime < TargetTime) || (Direction == -1 && CurrentTime > TargetTime))
        //{
        //    CurrentTime += Direction * SecProgress * Time.deltaTime;

        //    if((Direction == 1 && CurrentTime > TargetTime) || (Direction == -1 && CurrentTime > TargetTime))
        //    {
        //        CurrentTime = TargetTime;
        //    }

        //    yield return null;
        //}

        while (Mathf.Abs(WrappedTargrtTime - CurrentTime) > Epsilon)
        {
            CurrentTime += SecProgress * Time.deltaTime;
            yield return null;
        }
        Debug.Log("NightTrout");
        CurrentTime = TargetTime;
        timeAcceleration = null;
        defaultTimeCount = StartCoroutine(DefaultTimeCount());
        IsPlayerAttacked = false;
    }
}