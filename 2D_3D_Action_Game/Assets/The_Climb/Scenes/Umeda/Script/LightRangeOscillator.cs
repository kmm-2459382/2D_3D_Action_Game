using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightRangeOscillator : MonoBehaviour
{
    [Header("ライトの強弱用スクリプト")]
    [Header("範囲設定")]
    public float minRange = 2f;   // 最小範囲
    public float maxRange = 5f;   // 最大範囲

    [Header("時間設定")]
    public float period = 2f;     // 往復にかかる時間（秒）
    public bool randomizeTime = false; // ランダム時間モード
    public float randomMinPeriod = 1f;
    public float randomMaxPeriod = 3f;

    private Light lightSource;
    private float timer;
    private bool increasing = true; // 範囲増加中かどうか
    private float currentPeriod;

    void Start()
    {
        lightSource = GetComponent<Light>();
        lightSource.range = minRange;

        currentPeriod = period;
        if (randomizeTime)
        {
            currentPeriod = Random.Range(randomMinPeriod, randomMaxPeriod);
        }
    }

    void Update()
    {
        if (currentPeriod <= 0f) return;

        float delta = Time.deltaTime / (currentPeriod / 2f); // 半周期で min→max
        if (increasing)
        {
            lightSource.range += (maxRange - minRange) * delta;
            if (lightSource.range >= maxRange)
            {
                lightSource.range = maxRange;
                increasing = false;
                if (randomizeTime)
                    currentPeriod = Random.Range(randomMinPeriod, randomMaxPeriod);
            }
        }
        else
        {
            lightSource.range -= (maxRange - minRange) * delta;
            if (lightSource.range <= minRange)
            {
                lightSource.range = minRange;
                increasing = true;
                if (randomizeTime)
                    currentPeriod = Random.Range(randomMinPeriod, randomMaxPeriod);
            }
        }
    }
}
