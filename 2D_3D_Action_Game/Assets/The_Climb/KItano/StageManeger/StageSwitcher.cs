using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StageSwitcher : MonoBehaviour
{
    [Header("ステージPrefab")]
    public GameObject lightStagePrefab;
    public GameObject darkStagePrefab;

    [Header("フェード用")]
    public CanvasGroup fadeCanvas;
    public float fadeDuration = 1f;

    private GameObject currentStage;
    private bool isLightWorld = true;
    private bool isSwitching = false; // ← 切り替え中フラグ

    void Start()
    {
        // 最初は光のステージをロード
        currentStage = Instantiate(lightStagePrefab);
        if (fadeCanvas != null) fadeCanvas.alpha = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SwitchStage();
        }
    }
    public void SwitchStage()
    {
        if (!isSwitching) // ← 切り替え中じゃなければ実行
        {
            StartCoroutine(SwitchRoutine());
        }
    }

    IEnumerator SwitchRoutine()
    {
        isSwitching = true; // ← 切り替え開始

        // フェードアウト
        yield return Fade(1f);

        // ステージ削除と生成
        if (currentStage != null) Destroy(currentStage);
        currentStage = Instantiate(isLightWorld ? darkStagePrefab : lightStagePrefab);
        isLightWorld = !isLightWorld;

        // フェードイン
        yield return Fade(0f);

        isSwitching = false; // ← 切り替え完了
    }

    IEnumerator Fade(float targetAlpha)
    {
        if (fadeCanvas == null) yield break;

        float start = fadeCanvas.alpha;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(start, targetAlpha, time / fadeDuration);
            yield return null;
        }
        fadeCanvas.alpha = targetAlpha;
    }
}