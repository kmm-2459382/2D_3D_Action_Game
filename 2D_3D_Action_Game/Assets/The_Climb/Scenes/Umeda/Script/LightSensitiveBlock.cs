using UnityEngine;

/// <summary>
/// 光に反応して出現・消失する足場ブロック（軽量化版）
/// DetectionTrigger統合済み。
/// </summary>
[DisallowMultipleComponent]
public class LightSensitiveBlock : MonoBehaviour
{
    [Header("感知用Collider（子オブジェクトなどに配置）")]
    [Tooltip("光を検知するためのTrigger Colliderを設定します。")]
    public Collider detectionCollider;

    [Header("反応するレイヤー設定")]
    [Tooltip("光として反応させたいレイヤーを指定します。")]
    public LayerMask detectableLayers;

    [Header("更新間隔（秒）")]
    [Tooltip("SetActiveやRenderer制御を行う間隔（小さいほど即時反応）")]
    [Range(0f, 0.2f)] public float updateInterval = 0.05f;

    private Renderer blockRenderer;
    private Collider blockCollider;
    private int lightCount = 0;
    private bool isActive = false;
    private float timer = 0f;

    void Start()
    {
        blockRenderer = GetComponent<Renderer>();
        blockCollider = GetComponent<Collider>();
        ActivateBlock(false);

        if (detectionCollider == null)
        {
            Debug.LogWarning($"[LuminaBlock] 感知用Colliderが設定されていません: {name}");
            return;
        }

        // Rigidbody（Triggerが動作するため必要）
        Rigidbody rb = detectionCollider.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = detectionCollider.gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        // トリガー転送用クラスを付与
        TriggerForwarder forwarder = detectionCollider.gameObject.AddComponent<TriggerForwarder>();
        forwarder.targetBlock = this;
        forwarder.detectableLayers = detectableLayers;
    }

    private void Update()
    {
        // 一定間隔ごとに可視状態を更新
        timer += Time.deltaTime;
        if (timer < updateInterval) return;
        timer = 0f;

        bool shouldBeActive = lightCount > 0;
        if (shouldBeActive != isActive)
        {
            ActivateBlock(shouldBeActive);
            isActive = shouldBeActive;
        }
    }

    private void ActivateBlock(bool active)
    {
        if (blockRenderer != null)
            blockRenderer.enabled = active;

        if (blockCollider != null)
            blockCollider.enabled = active;
    }

    public void AddLight()
    {
        lightCount++;
    }

    public void RemoveLight()
    {
        lightCount = Mathf.Max(0, lightCount - 1);
    }

    public void ForceDeactivate()
    {
        lightCount = 0;
        ActivateBlock(false);
        isActive = false;
    }

    // -----------------------------
    // 内部クラス: Trigger転送専用
    // -----------------------------
    private class TriggerForwarder : MonoBehaviour
    {
        [HideInInspector] public LightSensitiveBlock targetBlock;
        [HideInInspector] public LayerMask detectableLayers;

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & detectableLayers) == 0) return;
            targetBlock?.AddLight();
        }

        private void OnTriggerExit(Collider other)
        {
            if (((1 << other.gameObject.layer) & detectableLayers) == 0) return;
            targetBlock?.RemoveLight();
        }
    }
}