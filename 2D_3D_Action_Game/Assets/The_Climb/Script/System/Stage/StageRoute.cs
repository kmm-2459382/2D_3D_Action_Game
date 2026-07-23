using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageRoute : MonoBehaviour
{
    // 各ボタンをインスペクターで登録（1〜8の順）
    public List<StageNode> stageButtons;

    // 通過済みステージを保存
    private HashSet<int> visitedStages = new HashSet<int>();

    // ★追加：直前の分岐情報
    private int lastFromStage = -1;
    private int lastToStage = -1;

    // ステージ分岐辞書
    private Dictionary<int, List<int>> routes = new Dictionary<int, List<int>>()
    {
        { 0, new List<int>{ 1, 2 } }, // 最初の2択を表示
        { 1, new List<int>{ 3, 4 } }, // てんたい → 相棒・アイテム切り替え
        { 2, new List<int>{ 4, 5 } }, // 時間切り替え → アイテム切り替え・操作めちゃくちゃ
        { 3, new List<int>{ 6 } },    // 相棒 → なんか
        { 4, new List<int>{ 6, 7 } }, // アイテム切り替え → なんか・足場消えちゃう
        { 5, new List<int>{ 7 } },    // 操作めちゃくちゃ → 足場消えちゃう
        { 6, new List<int>{ 8 } },    // なんか → ゴール
        { 7, new List<int>{ 8 } },    // 足場消えちゃう → ゴール
        { 8, new List<int>() }        // ゴール → 終点
    };

    void Start()
    {
        int lastStage = PlayerPrefs.GetInt("SelectStage", 0);

        lastFromStage = PlayerPrefs.GetInt("LastFromStage", -1);
        lastToStage = PlayerPrefs.GetInt("LastToStage", -1);

        LoadVisitedStages();

        List<int> actives = new List<int>(routes[lastStage]);

        // ★分岐制御：選ばれなかった方をロック
        if (lastFromStage == lastStage && lastToStage != -1)
        {
            actives.RemoveAll(x => x != lastToStage);
        }

        SetActiveButtons(actives);
    }

    public void OnStageButtonPressed(int buttonNumber)
    {
        Debug.Log("選択されたボタン番号：" + buttonNumber);

        int currentStage = PlayerPrefs.GetInt("SelectStage", 0);

        // ★分岐情報を保存
        PlayerPrefs.SetInt("LastFromStage", currentStage);
        PlayerPrefs.SetInt("LastToStage", buttonNumber);

        visitedStages.Add(buttonNumber);
        SaveVisitedStages();

        PlayerPrefs.SetInt("SelectStage", buttonNumber);

        List<int> actives = new List<int>(routes[buttonNumber]);

        SetActiveButtons(actives);
    }

    // ボタンの有効/無効と色を切り替える
    void SetActiveButtons(List<int> activeNumbers)
    {
        for (int i = 0; i < stageButtons.Count; i++)
        {
            int buttonIndex = i + 1;
            StageNode btn = stageButtons[i];
            Image btnImage = btn.GetComponent<Image>();

            if (visitedStages.Contains(buttonIndex))
            {
                // 通過済み → 無効化＋黄色
                //btn.enabled = false;
                //btnImage.color = Color.yellow;
            }
            else if (activeNumbers.Contains(buttonIndex))
            {
                // 現在選択可能 → 有効化＋白
                //btn.enabled = true;
                //btnImage.color = Color.white;
            }
            else
            {
                // それ以外 → 無効化＋灰色
                //btn.enabled = false;
                //btnImage.color = Color.gray;
            }
        }
    }

    // 通過済みステージを保存
    void SaveVisitedStages()
    {
        string savedData = string.Join(",", visitedStages);
        PlayerPrefs.SetString("VisitedStages", savedData);
        PlayerPrefs.Save();
    }

    // 通過済みステージを読み込み
    void LoadVisitedStages()
    {
        string savedData = PlayerPrefs.GetString("VisitedStages", "");
        if (!string.IsNullOrEmpty(savedData))
        {
            string[] parts = savedData.Split(',');
            foreach (string part in parts)
            {
                if (int.TryParse(part, out int num))
                    visitedStages.Add(num);
            }
        }
    }
}
