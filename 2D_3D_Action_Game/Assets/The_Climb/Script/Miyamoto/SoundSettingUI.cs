using UnityEngine;
using UnityEngine.UI;
public class SoundSettingUI : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;

    private void OnEnable()
    {
        // 保存されている値をUIに反映（非表示から開いた時もOK）
        bgmSlider.SetValueWithoutNotify(SoundManager.Instance.BGMVolume);
        seSlider.SetValueWithoutNotify(SoundManager.Instance.SEVolume);

        // スライダー変更で音量反映
        bgmSlider.onValueChanged.AddListener(SoundManager.Instance.SetBGMVolume);
        seSlider.onValueChanged.AddListener(SoundManager.Instance.SetSEVolume);
    }

    private void OnDisable()
    {
        // リスナー削除（重複防止）
        bgmSlider.onValueChanged.RemoveListener(SoundManager.Instance.SetBGMVolume);
        seSlider.onValueChanged.RemoveListener(SoundManager.Instance.SetSEVolume);
    }
}
