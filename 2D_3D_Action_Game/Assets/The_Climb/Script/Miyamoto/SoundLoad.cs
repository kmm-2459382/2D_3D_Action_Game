using UnityEngine;
using UnityEngine.Audio;

public class SoundLoad : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;

    const string BGM_KEY = "BGM_VOLUME";
    const string SE_KEY = "SE_VOLUME";

    void Awake()
    {
        float bgm = PlayerPrefs.GetFloat(BGM_KEY, 1f);
        float se = PlayerPrefs.GetFloat(SE_KEY, 1f);

        SetVolume("BGM_Volume", bgm);
        SetVolume("SE_Volume", se);
    }

    void SetVolume(string name, float value)
    {
        if (value <= 0.0001f)
            audioMixer.SetFloat(name, -80f);
        else
            audioMixer.SetFloat(name, Mathf.Log10(value) * 20);
    }
}
