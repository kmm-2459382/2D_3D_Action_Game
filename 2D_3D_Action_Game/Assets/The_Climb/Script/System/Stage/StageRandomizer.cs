using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class StagePoolData
{
    [Header("Stage")]
    public string sceneName;
    public string stageName;

    [Header("Images")]
    public Sprite loadingImage;
    public Sprite stagePreviewSprite;
}

public class StageRandomizer : MonoBehaviour
{
    [Header("ステージ設定")]
    public List<StagePoolData> NormalStagePool = new List<StagePoolData>();
    public List<StagePoolData> BossStagePool = new List<StagePoolData>();

    [Header("結果（他スクリプト参照用）")]
    public string[] SceneName = new string[8];
    public string[] StageName = new string[8];
    public Sprite[] LoadingImage = new Sprite[8];

    private int[] _currentSlotIndices = new int[8];

    private void Start()
    {
        if (PlayerPrefs.GetInt("GameStart") == 1 || !PlayerPrefs.HasKey("StageIndexOrder"))
        {
            Shuffle();

            for (int i = 0; i <= 20; i++)
                PlayerPrefs.SetInt($"StageCleared_{i}", 0);

            PlayerPrefs.DeleteKey("JustClearedStageId");
            PlayerPrefs.SetInt("GameStart", 0);
            PlayerPrefs.Save();
        }
        else
        {
            Load();
        }
    }

    private void Shuffle()
    {
        // ランダム化用のインデックスリストを作成
        List<int> pool = new List<int>();
        for (int i = 0; i < NormalStagePool.Count; i++)
            pool.Add(i); // 通常ステージのインデックスを追加

        // フィッシャー・イェーツのシャッフル
        for (int i = 0; i < pool.Count; i++)
        {
            int r = Random.Range(i, pool.Count);
            (pool[i], pool[r]) = (pool[r], pool[i]); // 要素を入れ替え
        }

        // 全スロットを -1 (未割り当て) で初期化
        int[] result = new int[8];
        for (int i = 0; i < result.Length; i++) result[i] = -1;

        // スロット0〜6 (List 1〜7) に重複なしで割り当て
        int[] normalSlots = { 0, 1, 2, 3, 4, 5, 6 };
        for (int i = 0; i < normalSlots.Length && i < pool.Count; i++)
            result[normalSlots[i]] = pool[i]; // シャッフルされたインデックスをスロットに設定

        // ボスステージの設定 (スロット7)
        if (BossStagePool.Count > 0)
            result[7] = Random.Range(0, BossStagePool.Count) + 1000; // ボスは1000以上の値で管理

        _currentSlotIndices = result; // 結果を保存
        Apply(); // 各配列にデータを適用
        PlayerPrefs.SetString("StageIndexOrder", string.Join(",", _currentSlotIndices)); // 保存
    }

    private void Load()
    {
        string[] s = PlayerPrefs.GetString("StageIndexOrder").Split(',');
        for (int i = 0; i < 8; i++)
            _currentSlotIndices[i] = int.Parse(s[i]);

        Apply();
    }

    private void Apply()
    {
        // 各スロット（最大8）をループ。配列のサイズ不足にも対応
        int count = Mathf.Min(8, _currentSlotIndices.Length);

        for (int i = 0; i < count; i++)
        {
            int idx = _currentSlotIndices[i];
            if (idx == -1) continue; // 未割り当てのスロット

            StagePoolData d = null;
            if (idx >= 1000)
            {
                int bIdx = idx - 1000;
                if (bIdx >= 0 && bIdx < BossStagePool.Count)
                    d = BossStagePool[bIdx];
            }
            else
            {
                if (idx >= 0 && idx < NormalStagePool.Count)
                    d = NormalStagePool[idx];
            }

            if (d != null)
            {
                // インスペクターで配列サイズが変更されている可能性があるため、個別にチェック
                if (i < SceneName.Length) SceneName[i] = d.sceneName;
                if (i < StageName.Length) StageName[i] = d.stageName;
                if (i < LoadingImage.Length) LoadingImage[i] = d.loadingImage;
            }
        }

        // ステージの順番をログに表示
        Debug.Log("--- ステージルート順序 ---");
        for (int i = 0; i < StageName.Length; i++)
        {
            if (string.IsNullOrEmpty(StageName[i])) continue;
            Debug.Log($"List {i + 1}: {StageName[i]} ({(i < SceneName.Length ? SceneName[i] : "None")})");
        }
        Debug.Log("--------------------------");
    }

    public Sprite GetStagePreviewSprite(int stageId)
    {
        int slot = stageId - 1;
        if (slot < 0 || slot >= _currentSlotIndices.Length)
            return null;

        int idx = _currentSlotIndices[slot];

        if (idx >= 1000)
        {
            int b = idx - 1000;
            return (b >= 0 && b < BossStagePool.Count)
                ? BossStagePool[b].stagePreviewSprite
                : null;
        }
        else
        {
            return (idx >= 0 && idx < NormalStagePool.Count)
                ? NormalStagePool[idx].stagePreviewSprite
                : null;
        }
    }

    public void StartStage(int id)
    {
        PlayerPrefs.SetInt("CurrentStageId", id);
        PlayerPrefs.Save();

        System.Loading.SceneLoader.Instance.LoadScene(
            SceneName[id - 1],
            LoadingImage[id - 1]
        );
    }
}