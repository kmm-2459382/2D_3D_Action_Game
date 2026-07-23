// RestartButtonUI.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButtonUI : MonoBehaviour
{
    // ボタンから直接呼ぶ用
    public void RestartGame()
    {
        // 現在のシーンを再読み込み
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
