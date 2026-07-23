using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 光や特定のトリガーに反応して、指定ブロック群の表示・当たり判定を切り替える。
/// 複数のトリガーコライダーに対応。少なくとも1つが触れている間は有効化され続ける。
/// </summary>
[DisallowMultipleComponent]
public class TriggerBlockActivator : MonoBehaviour
{
    [Header("表示・消失させるターゲット（複数可）")]
    public GameObject[] targetBlocks;

    [Header("反応させるレイヤー")]
    public LayerMask detectableLayers;

    // 🔸 各ブロックごとに「何個のコライダーが触れているか」を記録
    private Dictionary<GameObject, int> activeTriggerCounts = new Dictionary<GameObject, int>();

    private void Start()
    {
        // ✅ 初期状態：すべて非表示
        if (targetBlocks != null)
        {
            foreach (var block in targetBlocks)
            {
                if (block == null) continue;
                SetBlockVisible(block, false);
                activeTriggerCounts[block] = 0;
            }
        }

        // ✅ Rigidbody確認（Trigger動作用）
        Collider col = GetComponent<Collider>();
        if (col != null && col.isTrigger && GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
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

    // ------------------------------------------------------
    // トリガー接触カウントを更新
    // ------------------------------------------------------
    private void UpdateBlockState(bool isEntering)
    {
        if (targetBlocks == null) return;

        foreach (var block in targetBlocks)
        {
            if (block == null) continue;

            // カウント更新
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

            // カウント結果に応じて切り替え
            bool shouldBeVisible = activeTriggerCounts[block] > 0;
            SetBlockVisible(block, shouldBeVisible);

            // 無効化前に上のRigidbodyを起こす（消える直前のみ）
            if (!shouldBeVisible)
                WakeUpObjectsAbove(block);
        }
    }

    // ------------------------------------------------------
    // Renderer と Collider を切り替える
    // ------------------------------------------------------
    private void SetBlockVisible(GameObject block, bool visible)
    {
        Renderer renderer = block.GetComponent<Renderer>();
        if (renderer != null)
            renderer.enabled = visible;

        Collider collider = block.GetComponent<Collider>();
        if (collider != null)
            collider.enabled = visible;
    }

    // ------------------------------------------------------
    // 上にある Rigidbody を起こす（ブロックが消える時）
    // ------------------------------------------------------
    private void WakeUpObjectsAbove(GameObject block)
    {
        Collider[] hits = Physics.OverlapBox(
            block.transform.position + Vector3.up * 0.5f,
            new Vector3(0.5f, 0.5f, 0.5f),
            Quaternion.identity
        );

        foreach (var hit in hits)
        {
            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null)
            {
                rb.WakeUp();
                rb.AddForce(Vector3.down * 0.01f, ForceMode.VelocityChange);
            }
        }
    }
}
