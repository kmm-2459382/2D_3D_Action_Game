
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class TransparentOnPlayerNear_URP : MonoBehaviour
{
    [Header("参照")]
    public Transform player;  // プレイヤーのTransformをInspectorで指定

    [Header("設定")]
    public float transparentDistance = 5f; // この距離以内に入ったら透明化
    [Range(0f, 1f)]
    public float transparentAlpha = 0.3f;  // 透明度（0＝完全透明、1＝不透明）

    private Renderer objRenderer;
    private Material objMaterial;
    private Color originalColor;
    private bool isTransparent = false;

    void Start()
    {
        objRenderer = GetComponent<Renderer>();

        if (objRenderer == null)
        {
            Debug.LogError("[TransparentOnPlayerNear_URP] Rendererが見つかりません！");
            enabled = false;
            return;
        }

        // マテリアルをインスタンス化（共有マテリアルへの影響を防ぐ）
        objMaterial = new Material(objRenderer.material);
        objRenderer.material = objMaterial;

        originalColor = objMaterial.color;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(player.position, transform.position);

        if (dist <= transparentDistance && !isTransparent)
        {
            SetTransparency(true);
        }
        else if (dist > transparentDistance && isTransparent)
        {
            SetTransparency(false);
        }
    }

    void SetTransparency(bool makeTransparent)
    {
        Color color = objMaterial.color;

        if (makeTransparent)
        {
            color.a = transparentAlpha;
            objMaterial.color = color;
            isTransparent = true;
        }
        else
        {
            color.a = 1f;
            objMaterial.color = color;
            isTransparent = false;
        }
    }

    // Gizmosで範囲を可視化
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 1f, 0.3f); // 半透明の水色
        Gizmos.DrawWireSphere(transform.position, transparentDistance);
        Gizmos.DrawSphere(transform.position, 0.1f); // 中心マーカー
    }
}
