using UnityEngine;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    private bool isFading = false; // フェード中かどうか
    public bool IsFading => isFading; // ← 外部から読み取り用

    private void Awake()
    {
        if (fadeImage != null)
        {
            // 最初は透明にしておく
            fadeImage.color = new Color(0, 0, 0, 0);
        }
    }

    /// <summary>
    /// フェードアウト → 処理実行 → フェードイン
    /// </summary>
    public void FadeAndDo(System.Action onMiddle)
    {
        if (!isFading) // フェード中でなければ実行
        {
            StartCoroutine(FadeRoutine(onMiddle));
        }
    }

    private IEnumerator FadeRoutine(System.Action onMiddle)
    {
        isFading = true; // フェード開始

        // フェードアウト
        yield return StartCoroutine(Fade(0, 1));

        // 中間処理（マップ切り替えなど）
        onMiddle?.Invoke();

        // フェードイン
        yield return StartCoroutine(Fade(1, 0));

        isFading = false; // フェード終了
    }

    private IEnumerator Fade(float start, float end)
    {
        float t = 0f;
        Color baseColor = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(start, end, t / fadeDuration);
            fadeImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, end);
    }
}