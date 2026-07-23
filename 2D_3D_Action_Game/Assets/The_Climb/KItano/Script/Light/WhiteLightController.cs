using System.Collections;
using UnityEngine;

public class WhiteLightController : MonoBehaviour
{
    private Renderer objRenderer;
    private Collider col;
    private Color originalColor;
    private float currentAlpha = 1f;

    private bool isLit = false;
    private bool isFading = false;

    [Header("設定項目")]
    public float fadeSpeed = 0.5f;        // 透明化／復帰速度
    public float invisibleDuration = 10f;  // 完全に透明のまま維持する時間

    private Coroutine fadeRoutine;

    void Start()
    {
        objRenderer = GetComponent<Renderer>();
        col = GetComponent<Collider>();
        originalColor = objRenderer.material.color;
        SetAlpha(1f);

        Debug.Log($"{name}: 初期化完了");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Light"))
        {
            isLit = true;
            Debug.Log($"{name}: ライトが当たった");

            if (fadeRoutine != null)
                StopCoroutine(fadeRoutine);

            fadeRoutine = StartCoroutine(FadeToTransparent());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Light"))
        {
            isLit = false;
            Debug.Log($"{name}: ライトが離れた");

            if (fadeRoutine != null)
                StopCoroutine(fadeRoutine);

            fadeRoutine = StartCoroutine(FadeToVisible());
        }
    }

    private IEnumerator FadeToTransparent()
    {
        isFading = true;
        Debug.Log($"{name}: 透明化開始");

        while (currentAlpha > 0f && isLit)
        {
            currentAlpha -= Time.deltaTime * fadeSpeed;
            SetAlpha(currentAlpha);

            if (currentAlpha <= 0f)
            {
                currentAlpha = 0f;
                SetAlpha(0f);
                col.enabled = false;
                Debug.Log($"{name}: 完全透明・コライダー無効化");
                yield return new WaitForSeconds(invisibleDuration);
                break;
            }

            yield return null;
        }

        if (currentAlpha <= 0f)
        {
            Debug.Log($"{name}: 透明維持時間終了、復帰開始");
            fadeRoutine = StartCoroutine(FadeToVisible());
        }

        isFading = false;
    }

    private IEnumerator FadeToVisible()
    {
        col.enabled = true;
        Debug.Log($"{name}: 復帰開始");

        while (currentAlpha < 1f && !isLit)
        {
            currentAlpha += Time.deltaTime * fadeSpeed;
            SetAlpha(currentAlpha);
            yield return null;
        }

        currentAlpha = 1f;
        SetAlpha(1f);
        Debug.Log($"{name}: 完全復帰・コライダー有効化");
    }

    private void SetAlpha(float alpha)
    {
        Color c = objRenderer.material.color;
        c.a = Mathf.Clamp01(alpha);
        objRenderer.material.color = c;
    }
}
