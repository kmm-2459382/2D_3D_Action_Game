using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TempDisableColliders : MonoBehaviour
{
    [SerializeField] private List<Collider> colliders = new List<Collider>();

    [SerializeField] private float duration = 2f;
    [SerializeField] private float cooldownTime = 3f;

    [Header("スキル使用回数制限")]
    [SerializeField] private int maxUseCount = 3;   // ← 後から自由に変更可能
    private int currentUseCount = 0;                 // ← 今まで使った回数

    public int CurrentUseCount
    {
        get { return currentUseCount; }
        set { currentUseCount = value; }
    }

    private float currentCooldownTimer = 0f;
    public bool isRunning = false;

    void Update()
    {
        // 使用条件
        if (Input.GetKeyDown(KeyCode.Q)
            && !isRunning
            && currentUseCount < maxUseCount
            && PlayerPrefs.GetInt("KitanoAbi") == 1)
        {
            currentUseCount++; // ★ 発動した瞬間にカウント
            StartCoroutine(DisableRoutine());
        }
    }

    private IEnumerator DisableRoutine()
    {
        isRunning = true;

        // Collider 無効化
        foreach (var col in colliders)
        {
            if (col != null)
                col.enabled = false;
        }

        yield return new WaitForSeconds(duration);

        // Collider 有効化
        foreach (var col in colliders)
        {
            if (col != null)
                col.enabled = true;
        }

        // クールタイム
        currentCooldownTimer = 0f;
        while (currentCooldownTimer < cooldownTime)
        {
            currentCooldownTimer += Time.deltaTime;
            yield return null;
        }

        isRunning = false;
        currentCooldownTimer = 0f;
    }

    /// <summary>
    /// クールタイム進行状況 (0.0〜1.0)
    /// </summary>
    public float GetCooldownProgress()
    {
        if (!isRunning) return 0f;

        if (currentCooldownTimer > 0f)
        {
            float progress = 1f - (currentCooldownTimer / cooldownTime);
            return Mathf.Clamp01(progress);
        }

        return 0f;
    }

    /// <summary>
    /// 残り使用回数を取得（UI用）
    /// </summary>
    public int GetRemainingUseCount()
    {
        return maxUseCount - currentUseCount;
    }
}
