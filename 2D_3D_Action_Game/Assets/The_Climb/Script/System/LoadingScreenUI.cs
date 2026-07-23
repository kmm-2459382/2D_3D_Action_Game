using UnityEngine;
using UnityEngine.UI;

namespace System.Loading
{
    public class LoadingScreenUI : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private Button startButton; // 遷移開始ボタン

        private void Awake()
        {
            // 初期状態で遷移ボタンを隠しておく
            if (startButton != null)
            {
                startButton.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Sets the background image of the loading screen.
        /// </summary>
        /// <param name="sprite">The sprite to display.</param>
        public void SetBackgroundImage(Sprite sprite)
        {
            if (backgroundImage != null)
            {
                backgroundImage.sprite = sprite;
            }
        }

        /// <summary>
        /// Updates the progress bar value.
        /// </summary>
        /// <param name="progress">Progress value between 0 and 1.</param>
        public void UpdateProgress(float progress)
        {
            if (progressSlider != null)
            {
                progressSlider.value = progress;
            }
        }

        // スライダーを消してボタンを表示する
        public void ShowStartButton(UnityEngine.Events.UnityAction onButtonClick)
        {
            if (progressSlider != null)
            {
                progressSlider.gameObject.SetActive(false);
            }

            if (startButton != null)
            {
                startButton.gameObject.SetActive(true);
                startButton.onClick.RemoveAllListeners();
                startButton.onClick.AddListener(onButtonClick);
            }
        }

        /// <summary>
        /// Shows or hides the loading screen.
        /// </summary>
        /// <param name="isActive">True to show, false to hide.</param>
        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);

            // 表示されるときはスライダーを表示し、ボタンを隠す初期状態に戻す
            if (isActive)
            {
                if (progressSlider != null)
                {
                    progressSlider.gameObject.SetActive(true);
                }
                if (startButton != null)
                {
                    startButton.gameObject.SetActive(false);
                }
            }
        }
    }
}
