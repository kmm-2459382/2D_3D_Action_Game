using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 光や特定のトリガーに反応して、指定ブロック群の表示・当たり判定を切り替える（最適化版）
/// </summary>
[DisallowMultipleComponent]
public class TriggerBlockActivatorOptimized : MonoBehaviour
{
    [Header("表示・消失させるターゲット（複数可）")]
    public GameObject[] targetBlocks;

    [Header("反応させるレイヤー")]
    public LayerMask detectableLayers;

    // 各ブロックごとの接触カウンター
    private Dictionary<GameObject, int> activeTriggerCounts = new Dictionary<GameObject, int>();

    // OverlapBoxNonAlloc用の使い回し配列（メモリ割当をゼロにするため）
    private readonly Collider[] overlapResults = new Collider[10];

    private void Start()
    {
        if (targetBlocks != null)
        {
            foreach (var block in targetBlocks)
            {
                if (block == null) continue;
                SetBlockVisible(block, false);
                activeTriggerCounts[block] = 0;
            }
        }

        // 自身がトリガーの場合のRigidbody確認
        Collider col = GetComponent<Collider>();
        if (col != null && col.isTrigger && GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & detectableLayers) == 0) return;
        UpdateBlockState(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & detectableLayers) == 0) return;
        UpdateBlockState(false);
    }

    private void UpdateBlockState(bool isEntering)
    {
        if (targetBlocks == null) return;

        foreach (var block in targetBlocks)
        {
            if (block == null) continue;

            if (!activeTriggerCounts.ContainsKey(block))
                activeTriggerCounts[block] = 0;

            if (isEntering)
            {
                activeTriggerCounts[block]++;
            }
            else
            {
                activeTriggerCounts[block] = Mathf.Max(0, activeTriggerCounts[block] - 1);
            }

            bool shouldBeVisible = activeTriggerCounts[block] > 0;
            SetBlockVisible(block, shouldBeVisible);

            // ブロックが消える時のみ実行
            if (!shouldBeVisible)
            {
                WakeUpObjectsAbove(block);
            }
        }
    }

    private void SetBlockVisible(GameObject block, bool visible)
    {
        if (block == null) return;

        Renderer renderer = block.GetComponent<Renderer>();
        if (renderer != null)
            renderer.enabled = visible;

        Collider collider = block.GetComponent<Collider>();
        if (collider != null)
            collider.enabled = visible;
    }

    private void WakeUpObjectsAbove(GameObject block)
    {
        if (block == null) return;

        Vector3 center = block.transform.position + Vector3.up * 0.5f;
        Vector3 halfExtents = new Vector3(0.5f, 0.5f, 0.5f);

        // 🔸 NonAlloc版を使用し、メモリの新規割り当て（ガベージ）を完全に防ぐ
        int hitCount = Physics.OverlapBoxNonAlloc(center, halfExtents, overlapResults, Quaternion.identity);

        for (int i = 0; i < hitCount; i++)
        {
            Collider hit = overlapResults[i];
            if (hit == null) continue;

            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null)
            {
                rb.WakeUp();
                rb.AddForce(Vector3.down * 0.01f, ForceMode.VelocityChange);
            }
            // 配列の参照をクリアしてメモリリークを防ぐ
            overlapResults[i] = null;
        }
    }
}