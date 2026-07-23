using UnityEngine;
using System.Collections;

public class RaycastReactiveObject : MonoBehaviour
{
    [Header("初期状態")]
    public bool startVisible = false;

    [Header("フェード設定")]
    public float fadeSpeed = 1f;

    [Header("待機・点滅設定")]
    public float stayTime = 2f;
    public float blinkDuration = 1f;
    public float blinkInterval = 0.15f;

    [Header("Collider")]
    public Collider raycastCollider; // isTrigger = true
    public Collider solidCollider;   // isTrigger = false

    Renderer rend;
    Material mat;

    float currentAlpha;
    bool isVisible;
    bool rayHit;

    bool isBusy; // ★ ループ完全ロック用

    void Start()
    {
        rend = GetComponent<Renderer>();
        mat = rend.material;

        isVisible = startVisible;
        currentAlpha = isVisible ? 1f : 0.5f;

        ApplyState(currentAlpha);

      //  Debug.Log($"[{name}] 初期状態 : {(isVisible ? "表示" : "透明")}");
    }

    void Update()
    {
        if (isBusy) return;

        if (rayHit)
        {
            rayHit = false;

            if (isVisible)
            {
               // Debug.Log($"[{name}] Raycast検知 → 透明化ループ開始");
                isBusy = true;
                StartCoroutine(VisibleStartRoutine());
            }
            else
            {
              //  Debug.Log($"[{name}] Raycast検知 → 出現ループ開始");
                isBusy = true;
                StartCoroutine(InvisibleStartRoutine());
            }
        }
    }

    // ===== Raycastから呼ばれる =====
    public void OnRaycastHit()
    {
        if (isBusy) return;

       // Debug.Log($"[{name}] Raycastが命中しました");
        rayHit = true;
    }

    // ===== 透明スタート専用 =====
    IEnumerator InvisibleStartRoutine()
    {
      //  Debug.Log($"[{name}] フェードイン開始");

        yield return FadeTo(0.8f);
        isVisible = true;

       // Debug.Log($"[{name}] 完全表示");

        yield return new WaitForSeconds(stayTime);

      //  Debug.Log($"[{name}] 点滅開始");
        yield return Blink();

     //   Debug.Log($"[{name}] 点滅終了 → 透明化へ");

        yield return FadeTo(0.2f);
        isVisible = false;

      //  Debug.Log($"[{name}] 完全透明 → 次のRaycast待ち");

        isBusy = false;
    }

    // ===== 出現スタート専用 =====
    IEnumerator VisibleStartRoutine()
    {
      //  Debug.Log($"[{name}] フェードアウト開始");

        yield return FadeTo(0f);
        isVisible = false;

       // Debug.Log($"[{name}] 完全透明");

        yield return new WaitForSeconds(stayTime);

      //  Debug.Log($"[{name}] 自動フェードイン（Raycast不要）");

        yield return FadeTo(1f);
        isVisible = true;

       // Debug.Log($"[{name}] 完全表示 → 次のRaycast待ち");

        isBusy = false;
    }

    // ===== 共通処理 =====
    IEnumerator FadeTo(float target)
    {
        while (!Mathf.Approximately(currentAlpha, target))
        {
            currentAlpha = Mathf.MoveTowards(
                currentAlpha,
                target,
                fadeSpeed * Time.deltaTime
            );

            ApplyState(currentAlpha);
            yield return null;
        }
    }

    IEnumerator Blink()
    {
        int count = Mathf.CeilToInt(blinkDuration / blinkInterval);

        for (int i = 0; i < count; i++)
        {
            rend.enabled = !rend.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }

        rend.enabled = true;
    }

    void ApplyState(float alpha)
    {
        Color c = mat.color;
        c.a = alpha;
        mat.color = c;

        solidCollider.enabled = alpha >= 0.75f;
    }
}
