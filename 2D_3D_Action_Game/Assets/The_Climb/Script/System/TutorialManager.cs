using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("チュートリアル画像")]
    [SerializeField] private Sprite[] tutorialImages; // 表示する画像の配列

    [Header("UI参照")]
    [SerializeField] private Image displayImage;      // 画像を表示するImageコンポーネント
    [SerializeField] private Button nextButton;       // 次へボタン
    [SerializeField] private Button prevButton;       // 前へボタン
    [SerializeField] private Button closeButton;      // 閉じるボタン
    [SerializeField] private GameObject tutorialPanel; // チュートリアル全体のパネル
    [SerializeField] private GameObject canvasToHide;  // チュートリアル中に非表示にするCanvas（任意）

    private int currentIndex = 0; // 現在表示している画像のインデックス

    void Start()
    {
        // ボタンのイベントリスナー登録
        // ボタンのイベントリスナー登録
        if (nextButton != null) nextButton.onClick.AddListener(OnNextButtonClicked);
        if (prevButton != null) prevButton.onClick.AddListener(OnPrevButtonClicked);
        if (closeButton != null) closeButton.onClick.AddListener(OnCloseButtonClicked);

        // 画像が設定されていればチュートリアルを開始
        if (tutorialImages != null && tutorialImages.Length > 0)
        {
            ShowTutorial();
        }
        else
        {
            // 画像がない場合はパネルを非表示にしておく
            if (tutorialPanel != null) tutorialPanel.SetActive(false);
        }

        // 画像がクリックを遮らないようにRaycastTargetを無効化
        if (displayImage != null)
        {
            displayImage.raycastTarget = false;
        }
    }

    // チュートリアルを表示する
    private void ShowTutorial()
    {
        if (tutorialPanel != null) tutorialPanel.SetActive(true);
        
        // 指定されたCanvasがあれば非表示にする
        if (canvasToHide != null)
        {
            canvasToHide.SetActive(false);
        }

        currentIndex = 0;
        UpdateUI();
        // ゲーム時間を停止（BGMはTime.timeScaleの影響を受けない設定であれば止まらない）
        Time.timeScale = 0f;
    }

    // UIの更新（画像とボタンの表示切り替え）
    private void UpdateUI()
    {
        if (tutorialImages == null || tutorialImages.Length == 0) return;

        // 画像の更新
        if (displayImage != null)
        {
            displayImage.sprite = tutorialImages[currentIndex];
        }

        // ボタンの表示制御
        // 画像が1枚以下の場合は矢印ボタンは不要（あるいは両方非表示）
        if (tutorialImages.Length <= 1)
        {
            if (nextButton != null) nextButton.gameObject.SetActive(false);
            if (prevButton != null) prevButton.gameObject.SetActive(false);
        }
        else
        {
            // 次へボタン: 最後の画像でなければ表示
            if (nextButton != null)
            {
                nextButton.gameObject.SetActive(currentIndex < tutorialImages.Length - 1);
            }

            // 前へボタン: 最初の画像でなければ表示
            if (prevButton != null)
            {
                bool shouldShow = currentIndex > 0;
                prevButton.gameObject.SetActive(shouldShow);
            }
            
            // 他のボタンも念のため最前面へ
            if (nextButton != null && nextButton.gameObject.activeSelf) nextButton.transform.SetAsLastSibling();
            if (closeButton != null && closeButton.gameObject.activeSelf) closeButton.transform.SetAsLastSibling();
        }
    }

    // 次へボタンが押されたとき
    private void OnNextButtonClicked()
    {
        if (currentIndex < tutorialImages.Length - 1)
        {
            currentIndex++;
            UpdateUI();
        }
    }

    // 前へボタンが押されたとき
    private void OnPrevButtonClicked()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateUI();
        }
    }

    // 閉じるボタンが押されたとき
    private void OnCloseButtonClicked()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }

        // 非表示にしていたCanvasがあれば再表示する
        if (canvasToHide != null)
        {
            canvasToHide.SetActive(true);
        }

        // ゲーム時間を再開
        Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        // シーン遷移などで破棄された場合に時間を戻す（安全策）
        Time.timeScale = 1f;
    }
}
