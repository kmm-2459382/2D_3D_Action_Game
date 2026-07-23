using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BranchRule
{
    public int triggerStage;
    public int[] blockFromStages;
    public int[] blockToStages;
}

[System.Serializable]
public class StageRequirement
{
    public int stage;
    public int[] requiredAny;
}

public class StageSelectManager : MonoBehaviour
{
    [Header("参照")]
    public StageNode[] stages;
    public StagePath[] paths;

    [Header("開始ステージ")]
    public int startStageId = 0;

    [Header("ステージ状態")]
    public bool[] clearedStages;

    [Header("分岐ルール（Path排他）")]
    public BranchRule[] branchRules;

    [Header("ステージ解放条件（OR依存）")]
    public StageRequirement[] stageRequirements;

    private bool[] prevClearedStages;
    private bool isInitialized = false;

    // ===============================
    // Lifecycle
    // ===============================
    private void Awake()
    {
        if (stages == null || stages.Length == 0)
            stages = FindObjectsOfType<StageNode>(true);

        if (paths == null || paths.Length == 0)
            paths = FindObjectsOfType<StagePath>(true);

        if (clearedStages == null || clearedStages.Length != stages.Length)
            clearedStages = new bool[stages.Length];

        prevClearedStages = new bool[clearedStages.Length];
    }

    private void OnEnable()
    {
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        yield return null;
        yield return null;

        // ★ 初期化（完全リセットはしない）
        LoadClearedFromPrefs();

        // ★ Stage0 は常に解放
        clearedStages[startStageId] = true;

        ApplyAllRules();
        Refresh();
        CopyArray(clearedStages, prevClearedStages);
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) return;

        // ★ ゴール直後の反映
        ApplyJustClearedStage();

        if (!HasClearedChanged())
            return;

        if (!clearedStages[startStageId])
            clearedStages[startStageId] = true;

        ApplyAllRules();
        Refresh();
        CopyArray(clearedStages, prevClearedStages);
    }

    // ===============================
    // ゴール反映（最新仕様対応）
    // ===============================
    private void ApplyJustClearedStage()
    {
        if (!PlayerPrefs.HasKey("JustClearedStageId"))
            return;

        int stageId = PlayerPrefs.GetInt("JustClearedStageId");

        if (IsValid(stageId))
            clearedStages[stageId] = true;

        PlayerPrefs.DeleteKey("JustClearedStageId");
    }

    // ===============================
    // 保存データ復元
    // ===============================
    private void LoadClearedFromPrefs()
    {
        for (int i = 0; i < clearedStages.Length; i++)
        {
            clearedStages[i] = PlayerPrefs.GetInt($"StageCleared_{i}", 0) == 1;
        }
    }

    private void SaveClearedToPrefs()
    {
        for (int i = 0; i < clearedStages.Length; i++)
        {
            PlayerPrefs.SetInt($"StageCleared_{i}", clearedStages[i] ? 1 : 0);
        }
    }

    // ===============================
    // ルール総適用
    // ===============================
    private void ApplyAllRules()
    {
        // 以前の複雑なルール（分岐・依存関係）は無視し、クリアデータの保存のみ行う
        SaveClearedToPrefs();
    }

    // クリア回数（スタートステージ以外）を取得
    private int GetClearCount()
    {
        int count = 0;
        for (int i = 0; i < clearedStages.Length; i++)
        {
            if (i == startStageId) continue;
            if (clearedStages[i]) count++;
        }
        return count;
    }

    // ===============================
    // OR依存
    // ===============================
    private bool ApplyStageRequirements()
    {
        bool changed = false;

        foreach (var req in stageRequirements)
        {
            if (req.stage == startStageId) continue;
            if (!IsValid(req.stage)) continue;

            bool satisfied = false;
            foreach (int parent in req.requiredAny)
            {
                if (IsValid(parent) && clearedStages[parent])
                {
                    satisfied = true;
                    break;
                }
            }

            if (!satisfied && clearedStages[req.stage])
            {
                clearedStages[req.stage] = false;
                changed = true;
            }
        }

        return changed;
    }

    // ===============================
    // 分岐排他
    // ===============================
    private bool ApplyBranchExclusionToClearedStages()
    {
        bool changed = false;

        foreach (var rule in branchRules)
        {
            if (!IsValid(rule.triggerStage)) continue;
            if (!clearedStages[rule.triggerStage]) continue;

            for (int i = 0; i < rule.blockFromStages.Length; i++)
            {
                int from = rule.blockFromStages[i];
                int to = rule.blockToStages[i];

                if (!IsValid(from) || !IsValid(to)) continue;
                if (to == startStageId) continue;

                if (clearedStages[from] && clearedStages[to])
                {
                    clearedStages[to] = false;
                    changed = true;
                }
            }
        }

        return changed;
    }

    // ===============================
    // 表示更新
    // ===============================
    private void Refresh()
    {
        int clearCount = GetClearCount();
        Debug.Log($"Current Clear Count: {clearCount}");

        // 3回クリア（Stage 1/2, 3/4, 5/6 から各1つずつ計3つ）でゲームクリア
        if (clearCount >= 3)
        {
            Debug.Log("Game Clear! Transitioning to GameClearScene...");
            PlayerPrefs.SetInt("GameCleared", 1); // ゲームクリアフラグをセット
            PlayerPrefs.Save();
            System.Loading.SceneLoader.Instance.LoadScene("GameClearScene", null);
            return;
        }

        // 全ノードとパスを一旦非表示にする
        foreach (var s in stages)
            if (s != null) s.gameObject.SetActive(false);
        foreach (var p in paths)
            if (p != null) p.gameObject.SetActive(false);

        // クリア回数に応じて表示するステージ ID を決定
        // 0回: 1, 2 / 1回: 3, 4 / 2回: 5, 6
        int idA = -1;
        int idB = -1;

        if (clearCount == 0) { idA = 1; idB = 2; }
        else if (clearCount == 1) { idA = 3; idB = 4; }
        else if (clearCount == 2) { idA = 5; idB = 6; }

        // 対象のステージを表示し、選択可能にする
        ActivateStageNode(idA);
        ActivateStageNode(idB);
    }

    private void ActivateStageNode(int stageId)
    {
        if (stageId < 0 || stageId >= stages.Length) return;
        
        var node = stages[stageId];
        if (node == null) return;

        node.gameObject.SetActive(true);
        node.SetAvailable();
    }

    // ===============================
    // Path排他
    // ===============================
    private HashSet<StagePath> CalculateBlockedPaths()
    {
        HashSet<StagePath> blocked = new HashSet<StagePath>();

        foreach (var rule in branchRules)
        {
            if (!IsValid(rule.triggerStage)) continue;
            if (!clearedStages[rule.triggerStage]) continue;

            for (int i = 0; i < rule.blockFromStages.Length; i++)
            {
                int from = rule.blockFromStages[i];
                int to = rule.blockToStages[i];

                foreach (var path in paths)
                    if (path.fromStage == from && path.toStage == to)
                        blocked.Add(path);
            }
        }
        return blocked;
    }

    // ===============================
    // Utility
    // ===============================
    private bool IsValid(int id) => id >= 0 && id < stages.Length;

    private bool HasClearedChanged()
    {
        for (int i = 0; i < clearedStages.Length; i++)
            if (clearedStages[i] != prevClearedStages[i])
                return true;
        return false;
    }

    private void CopyArray(bool[] src, bool[] dst)
    {
        for (int i = 0; i < src.Length; i++)
            dst[i] = src[i];
    }

    // ===============================
    // Stage選択（StageNodeから呼ばれる）
    // ===============================
    public void OnStageSelected(int stageId)
    {
        // ロード中は何もしない
        if (System.Loading.SceneLoader.IsLoading)
            return;

        // 範囲チェック
        if (stageId < 0 || stageId >= stages.Length)
            return;

        // 進行不可なら無視
        if (!stages[stageId].isInteractable)
            return;

        Debug.Log($"Stage Selected: {stageId}");

        // ★ 現在選択したステージIDを保存
        PlayerPrefs.SetInt("CurrentStageId", stageId);
        PlayerPrefs.Save();

        // ★ StageRandomizer に処理を任せる
        if (stages[stageId].stageRandomizer != null)
        {
            stages[stageId].stageRandomizer.StartStage(stageId);
        }
        else
        {
            Debug.LogError("StageRandomizer が設定されていません");
        }
    }
}
