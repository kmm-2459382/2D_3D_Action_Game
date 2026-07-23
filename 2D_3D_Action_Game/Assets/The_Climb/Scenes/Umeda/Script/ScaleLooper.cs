using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScaleLooper : MonoBehaviour
{
    [Header("ターゲット設定")]
    public List<Transform> colliderTargets = new List<Transform>();
    public List<Renderer> visualRenderers = new List<Renderer>();

    [Header("タイミング設定")]
    public float shrinkTime = 1f;
    public float waitTime = 7f;
    public float expandTime = 1f;
    public float initialWait = 0f;

    [Header("点滅（待機①後半）設定")]
    [Tooltip("待機①の後半で何回 1->0->1 を繰り返すか")]
    public int blinkCount = 3;

    private List<Vector3> originalScales = new List<Vector3>();
    private Vector3 originalRootPosition;
    private bool isRunning = false;

    private MaterialPropertyBlock propBlock;

    private static readonly int AlphaProp = Shader.PropertyToID("_Alpha");
    private static readonly int IntensityProp = Shader.PropertyToID("_EmissionIntensity");

    private float currentAlpha = 1f;
    private float currentIntensity = 1f;

    IEnumerator Start()
    {
        originalRootPosition = transform.position;
        propBlock = new MaterialPropertyBlock();

        foreach (var t in colliderTargets)
            if (t != null) originalScales.Add(t.localScale);

        while (!SceneReadyManager.SceneReady)
            yield return null;

        StartCoroutine(ScaleLoop());
    }

    private IEnumerator ScaleLoop()
    {
        if (isRunning) yield break;
        isRunning = true;

        if (initialWait > 0f) yield return new WaitForSeconds(initialWait);

        while (true)
        {
            // === 1. 待機① (Alpha=1 固定) ===
            if (waitTime > 0f)
            {
                float halfWait = waitTime * 0.5f;
                SetAlphaAll(1f);

                SetIntensityAll(1f);
                yield return new WaitForSeconds(halfWait);

                float elapsed = 0f;
                while (elapsed < halfWait)
                {
                    float progress = elapsed / halfWait;
                    float blink = (Mathf.Cos(progress * Mathf.PI * 2f * blinkCount) + 1f) * 0.5f;

                    SetIntensityAll(blink);
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                SetIntensityAll(1f);
            }

            // === 2. 縮小 (Alpha: 1 -> 0) ===
            yield return StartCoroutine(TransitionTo(0f, shrinkTime, true));

            // 🔵 縮小完了時にオブジェクトを無効化
            SetCollidersActive(false);

            // === 3. 待機② (Alpha=0 固定) ===
            SetAlphaAll(0f);
            SetIntensityAll(1f);

            Vector3 restorePos = transform.position;
            restorePos.z = originalRootPosition.z;
            transform.position = restorePos;

            if (waitTime > 0f)
                yield return new WaitForSeconds(waitTime);

            // === 4. 拡大 (Alpha: 0 -> 1) ===
            // 🔵 拡大を開始する前にオブジェクトを有効化
            SetCollidersActive(true);
            yield return StartCoroutine(TransitionTo(1f, expandTime, false));
        }
    }

    private IEnumerator TransitionTo(float ratio, float duration, bool isShrinking)
    {
        float elapsed = 0f;
        List<Vector3> startScales = new List<Vector3>();
        foreach (var t in colliderTargets)
            if (t != null) startScales.Add(t.localScale);

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            for (int i = 0; i < colliderTargets.Count; i++)
            {
                if (colliderTargets[i] == null) continue;
                colliderTargets[i].localScale = Vector3.Lerp(startScales[i], originalScales[i] * ratio, t);
            }

            currentAlpha = isShrinking ? (1f - t) : t;
            SetAlphaAll(currentAlpha);

            elapsed += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < colliderTargets.Count; i++)
        {
            if (colliderTargets[i] == null) continue;
            colliderTargets[i].localScale = originalScales[i] * ratio;
        }
        SetAlphaAll(isShrinking ? 0f : 1f);
    }

    // 🟢 オブジェクトの有効・無効を一括切り替えするメソッド
    private void SetCollidersActive(bool active)
    {
        foreach (var t in colliderTargets)
        {
            if (t != null)
                t.gameObject.SetActive(active);
        }
    }

    private void SetAlphaAll(float alpha)
    {
        currentAlpha = alpha;
        UpdateVisuals();
    }

    private void SetIntensityAll(float intensity)
    {
        currentIntensity = intensity;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        for (int i = 0; i < visualRenderers.Count; i++)
        {
            if (visualRenderers[i] == null) continue;
            visualRenderers[i].GetPropertyBlock(propBlock);

            propBlock.SetFloat(AlphaProp, currentAlpha);
            propBlock.SetFloat(IntensityProp, currentIntensity * currentAlpha);

            visualRenderers[i].SetPropertyBlock(propBlock);
        }
    }
}