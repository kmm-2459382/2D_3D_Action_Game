using UnityEngine;
using System.Collections;

//  闇の進行度を調整する
public class FadeContoroller : MonoBehaviour, IDownFading
{
    [SerializeField] FadeSetting fadeSetting;    //  フェード設定
    Material OverLayMaterial;    //  四隅からフェードするのマテリアル
    Coroutine downFadeCoroutine;    //  ダウンフェードコルーチン

    [Header("現在のフェード進行度")]
    [SerializeField, Range(0, 1)] float CurrentProgress;    //  フェード進行度
    float CurrentProgressRate_Sec;    //  秒間のフェード進行速度(割合値)

    void Awake()
    {
        OverLayMaterial = fadeSetting.OverLayMaterial;

        //  初期化
        Initialize();
    }
    //  初期化
    void Initialize()
    {
        CurrentProgress = fadeSetting.Progress;
        CurrentProgressRate_Sec = fadeSetting.FadeProgressRate_Sec;
    }
    //  ダウン時フェードラッパー
    public void StartDownFading()
    {
        CoroutineUtility.SafeStartCoroutine(this, ref downFadeCoroutine, DownFading());
    }
    //  フェードアウト処理
    public IEnumerator DownFading()
    {
#if UNITY_EDITOR
        FadeSetter.ApplyToSceneFadeQuad();
#endif
        while(CurrentProgress > 0)
        {
            CurrentProgress -= CurrentProgressRate_Sec * Time.deltaTime;
            OverLayMaterial.SetFloat("_Progress", CurrentProgress);
            yield return null;
        }
        while (CurrentProgress <= 1)
        {
            CurrentProgress += CurrentProgressRate_Sec * Time.deltaTime;
            OverLayMaterial.SetFloat("_Progress", CurrentProgress);
            yield return null;
        }
        downFadeCoroutine = null;
    }
}