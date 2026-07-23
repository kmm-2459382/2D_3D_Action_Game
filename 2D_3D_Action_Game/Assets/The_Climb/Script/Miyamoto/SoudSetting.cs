using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoudSetting : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;

    const string BGM_KEY = "BGM_VOLUME";
    const string SE_KEY = "SE_VOLUME";

    void Start()
    {
        // 保存データ読み込み
        float bgm = PlayerPrefs.GetFloat(BGM_KEY, 1f);
        float se = PlayerPrefs.GetFloat(SE_KEY, 1f);

        // スライダーに値を反映（イベント発火なし）
        bgmSlider.SetValueWithoutNotify(bgm);
        seSlider.SetValueWithoutNotify(se);

        // スライダー操作時の処理登録
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        seSlider.onValueChanged.AddListener(SetSEVolume);

        // AudioMixerに適用
        SetBGMVolume(bgm);
        SetSEVolume(se);
    }

    public void SetBGMVolume(float value)
    {
        PlayerPrefs.SetFloat(BGM_KEY, value);

        if (value <= 0.0001f)
            audioMixer.SetFloat("BGM_Volume", -80f);
        else
            audioMixer.SetFloat("BGM_Volume", Mathf.Log10(value) * 20);
    }

    public void SetSEVolume(float value)
    {
        PlayerPrefs.SetFloat(SE_KEY, value);

        if (value <= 0.0001f)
            audioMixer.SetFloat("SE_Volume", -80f);
        else
            audioMixer.SetFloat("SE_Volume", Mathf.Log10(value) * 20);
    }
}
