
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ThroughWall : MonoBehaviour
{
    [Header("対象設定")]
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private float revealDistance = 4f;

    [Header("色設定")]
    [SerializeField] private Color revealedColor = Color.cyan;
    [SerializeField] private Color hiddenColor = Color.white;
    [SerializeField] private float fadeDuration = 1f;

    [Header("コライダー設定")]
    [SerializeField] private Collider physicsCollider;

    private Renderer objRenderer;
    private Material objMaterial;
    private Coroutine fadeRoutine;
    private Transform player;
    private bool isRevealed = false;

    void Start()
    {
        objRenderer = GetComponent<Renderer>();
        objMaterial = new Material(objRenderer.material); // マテリアルをインスタンス化
        objRenderer.material = objMaterial;

        if (physicsCollider != null)
            physicsCollider.enabled = true;

        GameObject playerObj = GameObject.FindGameObjectWithTag(targetTag);
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= revealDistance && !isRevealed)
        {
            StartFade(0.2f, revealedColor);
            isRevealed = true;
            if (physicsCollider != null) physicsCollider.enabled = false;
        }
        else if (distance > revealDistance && isRevealed)
        {
            StartFade(1f, hiddenColor);
            isRevealed = false;
            if (physicsCollider != null) physicsCollider.enabled = true;
        }
    }

    void StartFade(float targetAlpha, Color targetColor)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeTo(targetAlpha, targetColor));
    }

    private System.Collections.IEnumerator FadeTo(float targetAlpha, Color targetColor)
    {
        Color startColor = objMaterial.color;
        float startAlpha = startColor.a;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            Color lerpedColor = Color.Lerp(startColor, targetColor, t);
            lerpedColor.a = newAlpha;
            objMaterial.color = lerpedColor;
            yield return null;
        }

        // 最終色を明示的に設定
        targetColor.a = targetAlpha;
        objMaterial.color = targetColor;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 1, 0.25f);
        Gizmos.DrawSphere(transform.position, revealDistance);
    }
}
