using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI; // Buttonコンポーネント用

public class StartGame : MonoBehaviour
{
    [FormerlySerializedAs("ScenePool")]
    [FormerlySerializedAs("StageName")]
    public string[] SceneName; // リセット対象の全シーン名

    public TextMeshProUGUI UITextMeshPro;
    [SerializeField] public UnityEngine.UI.Button continueButton; // 「つづきから」ボタンの参照

    public void Start()
    {
        int ClearStagecount = PlayerPrefs.GetInt("ClearStagecount", 0);
        UITextMeshPro.text = ($"読破数 {ClearStagecount}/6"); 

        // 「つづきから」ボタンの有効化判定
        // 1. 過去に一度でもGameStartを押した（プレイ実績がある）場合は有効
        // 2. 既にクリア済み (GameCleared == 1) の場合は無効
        // ※GameStartフラグはランダム生成のためにリセットされるため、判定にはHasSaveDataを使用
        bool hasSaveData = PlayerPrefs.GetInt("HasSaveData", 0) == 1;
        bool hasCleared = PlayerPrefs.GetInt("GameCleared", 0) == 1;

        if (continueButton != null)
        {
            continueButton.interactable = hasSaveData && !hasCleared;
        }
    }
    public void GameStart()
    {
        PlayerPrefs.SetInt("GameStart", 1); 
        PlayerPrefs.SetInt("HasSaveData", 1); // プレイ実績フラグをセット
        PlayerPrefs.SetInt("SelectStage", 0); 

        PlayerPrefs.DeleteKey("VisitedStages"); 
        PlayerPrefs.DeleteKey("ClearedStages"); 
        
        // シーン名ベースのフラグリセット
        if (SceneName != null)
        {
            foreach (string scene in SceneName)
            {
                PlayerPrefs.SetInt(scene, 0);
                PlayerPrefs.SetInt(scene + "Clear", 0); // 個別クリアフラグもリセット
            }
        }

        // ステージIDベースのフラグリセット (0〜20)
        for (int i = 0; i < 20; i++)
        {
            PlayerPrefs.SetInt($"StageCleared_{i}", 0);
        }
        PlayerPrefs.DeleteKey("JustClearedStageId");
        PlayerPrefs.SetInt("GameCleared", 0); // クリアフラグをリセット
        PlayerPrefs.SetInt("ClearStagecount", 0); // 読破数をリセット
        
        PlayerPrefs.Save();
        SceneManager.LoadScene("StageSelect"); 
    }

    // 「つづきから」ボタンから呼ばれるメソッド
    public void ContinueGame()
    {
        SceneManager.LoadScene("StageSelect");
    }
    public void EndGame()
    {
        SceneManager.LoadScene("Title");
    }
}
