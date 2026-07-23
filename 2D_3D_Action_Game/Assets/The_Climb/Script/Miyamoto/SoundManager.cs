using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioMixer audioMixer;

    const string BGM_KEY = "BGM_VOLUME";
    const string SE_KEY = "SE_VOLUME";

    public float BGMVolume { get; private set; }
    public float SEVolume { get; private set; }

    private void Awake()
    {
        // Singleton化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 保存値ロード
            BGMVolume = PlayerPrefs.GetFloat(BGM_KEY, 1f);
            SEVolume = PlayerPrefs.GetFloat(SE_KEY, 1f);

            ApplyVolume("BGM_Volume", BGMVolume);
            ApplyVolume("SE_Volume", SEVolume);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // BGM音量設定
    public void SetBGMVolume(float value)
    {
        BGMVolume = value;
        PlayerPrefs.SetFloat(BGM_KEY, value);
        ApplyVolume("BGM_Volume", value);
    }

    // SE音量設定
    public void SetSEVolume(float value)
    {
        SEVolume = value;
        PlayerPrefs.SetFloat(SE_KEY, value);
        ApplyVolume("SE_Volume", value);
    }

    // AudioMixerに適用
    private void ApplyVolume(string name, float value)
    {
        if (value <= 0.0001f)
            audioMixer.SetFloat(name, -80f);
        else
            audioMixer.SetFloat(name, Mathf.Log10(value) * 20);
    }
}
