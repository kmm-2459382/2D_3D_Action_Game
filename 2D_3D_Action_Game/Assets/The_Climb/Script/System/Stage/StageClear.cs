using UnityEngine;
using UnityEngine.SceneManagement;

public class StageClear : MonoBehaviour
{
    private bool isClearing = false;
    private bool canClear = false;

    public bool IsClearing { get { return isClearing; } }    //  他クラスからステージclearを判断するために追記(松山)

    [Header("安全用ディレイ")]
    public float enableDelay = 0.5f; // シーン開始後◯秒は無効

    private void Start()
    {
        // 開始直後はクリア不可
        Invoke(nameof(EnableClear), enableDelay);
    }

    private void EnableClear()
    {
        canClear = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canClear) return;
        if (isClearing) return;
        if (!other.CompareTag("Player")) return;

        isClearing = true;


        int currentStageId = PlayerPrefs.GetInt("CurrentStageId", 0);
        int lastClearedStage = PlayerPrefs.GetInt("LastClearedStage", 0);

        Debug.Log($"ステージ {currentStageId} クリア");
        PlayerPrefs.SetInt($"{SceneManager.GetActiveScene().name}", 1);

        // ★ 最大到達ステージのみ更新
        if (currentStageId > lastClearedStage)
        {
            PlayerPrefs.SetInt("LastClearedStage", currentStageId);
        }

        // ★ StageSelectManager への通知用
        PlayerPrefs.SetInt("JustClearedStageId", currentStageId);
        // ★ 永続化用
        PlayerPrefs.SetInt($"StageCleared_{currentStageId}", 1);

        PlayerPrefs.Save();

        if (PlayerPrefs.GetInt($"{SceneManager.GetActiveScene().name}Clear", 0) == 0)
        {
            PlayerPrefs.SetInt($"{SceneManager.GetActiveScene().name}Clear", 1);
            int x = PlayerPrefs.GetInt("ClearStagecount", 0);
            PlayerPrefs.SetInt("ClearStagecount", x + 1);
        }

        // クリア回数の確認
        int clearCount = PlayerPrefs.GetInt("ClearStagecount", 0);

        // 3回クリアした場合は直接ゲームクリア画面へ
        if (clearCount >= 3)
        {
            Debug.Log("3ステージクリア達成！直接 GameClearScene へ遷移します。");
            PlayerPrefs.SetInt("GameCleared", 1); // ゲームクリアフラグをセット
            PlayerPrefs.Save();
            System.Loading.SceneLoader.Instance.LoadScene("GameClearScene");
        }
        else
        {
            // それ以外は通常通りステージセレクトへ
            System.Loading.SceneLoader.Instance.LoadScene("StageSelect");
        }
    }
}
