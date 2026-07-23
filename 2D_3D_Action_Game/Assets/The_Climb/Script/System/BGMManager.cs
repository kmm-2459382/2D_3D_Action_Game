using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// TitleとStageSelectシーン間でBGMを持続させるためのマネージャークラス
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance { get; private set; }

    [Header("BGM設定")]
    [Tooltip("TitleとStageSelectで流すBGM")]
    [SerializeField] private AudioClip commonBGM;

    private AudioSource audioSource;

    private void Awake()
    {
        // シングルトンパターンの実装
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
            
            // シーンロード時のイベント登録
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return; // 重要：重複している場合はこれ以上の処理を行わない
        }
    }

    private void Start()
    {
        // 最初のシーンでのBGMチェック
        CheckAndPlayBGM(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckAndPlayBGM(scene.name);
    }

    /// <summary>
    /// 現在のシーン名に基づいてBGMを再生または停止する
    /// </summary>
    /// <param name="sceneName">現在のシーン名</param>
    private void CheckAndPlayBGM(string sceneName)
    {
        // Title または StageSelect の場合
        if (sceneName == "Title" || sceneName == "StageSelect")
        {
            // BGMが設定されており、再生されていない、または違うクリップが再生されている場合
            if (commonBGM != null)
            {
                if (!audioSource.isPlaying || audioSource.clip != commonBGM)
                {
                    audioSource.clip = commonBGM;
                    audioSource.loop = true;
                    audioSource.Play();
                }
            }
        }
        else
        {
            // その他のシーンではBGMを停止（必要に応じてフェードアウトなどを実装可能）
            // もし他のシーンで別のBGM管理がある場合は、ここで干渉しないようにDestroyするか、
            // 単にStopするかは要件次第だが、今回は「持ち越して流したい」が主目的なので、
            // 対象外シーンでは停止する。
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
