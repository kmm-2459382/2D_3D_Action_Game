using UnityEngine;
using UnityEngine.EventSystems;

public enum GameState
{
    Playing,
    Paused
}

public class PauseUI : MonoBehaviour
{

    [Header("UI")]
    [SerializeField] private GameObject UI;                //オプションUI
    [SerializeField] private GameObject BGMUI;　　　　　　 //BGMUI
    [Header("Audio")]
    [SerializeField] private AudioSource bgmSource;        //BGM

    public GameState CurrentState { get; private set; }    //ゲーム中かオプション中か


    void Start()
    {  
        UI.SetActive(false);
        ResumeGame();

        // BGMはポーズ中も再生
        bgmSource.ignoreListenerPause = true;

    }
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CurrentState == GameState.Playing)
                PauseGame();
            else
                ResumeGame();
        }
    }

    //オプション表示中
    public void PauseGame()
    {
        CurrentState = GameState.Paused;

        Time.timeScale = 0f;
        AudioListener.pause = true;
        BGMUI.SetActive(false);
        UI.SetActive(true);

    }

    //ゲームシーン
    public void ResumeGame()
    {
        CurrentState = GameState.Playing;

        Time.timeScale = 1f;
        AudioListener.pause = false;

        UI.SetActive(false);
      

    }

}
