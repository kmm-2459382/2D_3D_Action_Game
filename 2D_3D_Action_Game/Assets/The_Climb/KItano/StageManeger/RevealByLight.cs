using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class RevealByLight : MonoBehaviour
{
    public float fadeDuration = 1f;
    public bool enableColliderOnReveal = true; // 出現時に Collider を有効化するか
    public bool tryMakeMaterialTransparent = true; // ランタイムで透明設定を試す

    Renderer rend;
    Material mat;
    Color originalColor;
    float originalAlpha;
    bool isRevealed = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend == null) { Debug.LogError("RevealByLight: Renderer required."); return; }

        // material をインスタンス化して保持（個別に変えるため）
        mat = rend.material;
        originalColor = mat.color;
        originalAlpha = originalColor.a;

        if (tryMakeMaterialTransparent)
            TrySetMaterialToTransparent(mat);

        // 最初は透明
        SetAlpha(0f);

        // 初期はコライダー無効化（必要なら true/false を切り替えて）
        if (enableColliderOnReveal)
        {
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }
    }

    public void Reveal()
    {
        if (isRevealed) return;
        isRevealed = true;
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        float start = mat.color.a;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(start, originalAlpha, t / fadeDuration);
            SetAlpha(a);
            yield return null;
        }
        SetAlpha(originalAlpha);

        if (enableColliderOnReveal)
        {
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = true;
        }
    }

    void SetAlpha(float a)
    {
        Color c = mat.color;
        c.a = a;
        mat.color = c;
    }

    // Standard/URP の簡易対応を試みる（完璧ではないので Inspector での手動確認を推奨）
    void TrySetMaterialToTransparent(Material m)
    {
        if (m == null) return;

        // Standard シェーダの _Mode を切り替え
        if (m.HasProperty("_Mode"))
        {
            m.SetFloat("_Mode", 2f); // 0:Opaque, 1:Cutout, 2:Fade, 3:Transparent
            m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            m.SetInt("_ZWrite", 0);
            m.DisableKeyword("_ALPHATEST_ON");
            m.EnableKeyword("_ALPHABLEND_ON");
            m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            m.renderQueue = 3000;
            return;
        }

        // URP Lit shader の Surface を切り替える場合（プロパティ名はプロジェクトによる）
        if (m.HasProperty("_Surface"))
        {
            m.SetFloat("_Surface", 1f); // 0 opaque, 1 transparent
            m.renderQueue = 3000;
            return;
        }

        Debug.LogWarning($"RevealByLight: material '{m.name}' may not support runtime transparency. Set material's rendering mode to Transparent in Inspector.");
    }
}