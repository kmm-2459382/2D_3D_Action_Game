using UnityEngine;
using System.Collections.Generic;

public class StageManager : MonoBehaviour
{
    [Header("ステージ設定")]
    public GameObject[] stagePrefabs;   // 各ステージのプレハブ
    public Transform player;            // プレイヤー参照
    public float stageLength = 50f;     // 1ステージの長さ

    private Dictionary<int, GameObject> activeStages = new Dictionary<int, GameObject>();
    private int currentStageIndex = 0;

    private void Start()
    {
        SpawnStage(0); // 最初のステージ生成
    }

    private void Update()
    {
        float playerX = player.position.x;

        float startX = currentStageIndex * stageLength;
        float endX = startX + stageLength;
        float progress = GetProgress(playerX, startX, endX);

        // --- 前のステージ処理 ---
        if (progress <= 0.2f && currentStageIndex > 0)
        {
            SpawnStage(currentStageIndex - 1);
        }
        else if (progress > 0.2f)
        {
            DestroyStage(currentStageIndex - 1);
        }

        // --- 次のステージ処理 ---
        if (progress >= 0.8f)
        {
            SpawnStage(currentStageIndex + 1);
        }
        else if (progress < 0.8f)
        {
            DestroyStage(currentStageIndex + 1);
        }

        // --- 現在ステージ更新 ---
        if (progress >= 1f) currentStageIndex++;
        if (progress <= 0f && currentStageIndex > 0) currentStageIndex--;
    }

    private void SpawnStage(int stageIndex)
    {
        if (stageIndex < 0) return;
        if (activeStages.ContainsKey(stageIndex)) return;

        int prefabIndex = stageIndex % stagePrefabs.Length;
        Vector3 pos = new Vector3(stageIndex * stageLength, 0, 0);
        GameObject stage = Instantiate(stagePrefabs[prefabIndex], pos, Quaternion.identity);
        activeStages[stageIndex] = stage;
    }

    private void DestroyStage(int stageIndex)
    {
        if (activeStages.ContainsKey(stageIndex))
        {
            Destroy(activeStages[stageIndex]);
            activeStages.Remove(stageIndex);
        }
    }

    // 進行度（Clampしない）
    private float GetProgress(float playerX, float startX, float endX)
    {
        return (playerX - startX) / (endX - startX);
    }
}