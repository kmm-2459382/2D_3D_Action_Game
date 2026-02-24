using UnityEngine;
using System.Collections;

public class ShortThornyCudgelController : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource audioSource;
    public AudioClip shortStretchSound;
    public AudioClip longStretchSound_1;
    public AudioClip longStretchSound_2;
    public AudioClip collisionSound;
    public AudioClip shrinkSound_1;
    public AudioClip shrinkSound_2;
    public AudioClip resetSound;

    [Header("初回クールタイム（秒）")]
    public float initialCooldown = 1f;

    [Header("各フェーズ時間（秒）")]
    public float cooldown1 = 4f;
    public float shortStretchDuration = 0.25f;
    public float longStretchDuration = 0.25f;
    public float cooldown3 = 0f;
    public float shrinkDuration = 1.5f;

    [Header("移動距離")]
    public float shortStretchDistance = 2f;
    public float longStretchDistance = 23f;

    [Header("回転速度（度/秒）")]
    public float stretchRotationSpeed = 360f;  // 左回転
    public float shrinkRotationSpeed = 180f;   // 右回転

    private enum State
    {
        InitialCooldown,
        Cooldown1,
        ShortStretch,
        LongStretch,
        Cooldown3,
        Shrinking
    }

    private State currentState = State.InitialCooldown;

    private Vector3 baseLocalPosition;
    private Vector3 shortStretchStart;
    private Vector3 shortStretchEnd;
    private Vector3 longStretchEnd;
    private Vector3 targetLocalPosition;

    private float timer = 0f;
    private float moveSpeed = 0f;
    private Coroutine loopSoundCoroutine;

    void Start()
    {
        baseLocalPosition = transform.localPosition;
        timer = 0f;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        switch (currentState)
        {
            case State.InitialCooldown:
                if (TimerReached(initialCooldown))
                {
                    timer = 0f;
                    currentState = State.Cooldown1;
                }
                break;

            case State.Cooldown1:
                if (TimerReached(cooldown1)) StartShortStretch();
                break;

            case State.ShortStretch:
                float t1 = Mathf.Clamp01(timer / shortStretchDuration);
                float easedT1 = Mathf.SmoothStep(0f, 1f, t1);
                transform.localPosition = Vector3.Lerp(shortStretchStart, shortStretchEnd, easedT1);
                Rotate(stretchRotationSpeed);
                timer += Time.deltaTime;
                if (t1 >= 1f) StartLongStretch();
                break;

            case State.LongStretch:
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetLocalPosition, moveSpeed * Time.deltaTime);
                Rotate(stretchRotationSpeed);
                if (Vector3.Distance(transform.localPosition, targetLocalPosition) < 0.01f)
                {
                    longStretchEnd = transform.localPosition;
                    timer = 0f;
                    currentState = State.Cooldown3;
                    OnLongStretchFinished(); // Cooldown3開始時にCollision再生
                }
                break;

            case State.Cooldown3:
                if (TimerReached(cooldown3)) StartShrinking();
                break;

            case State.Shrinking:
                float t3 = Mathf.Clamp01(timer / shrinkDuration);
                transform.localPosition = Vector3.Lerp(longStretchEnd, baseLocalPosition, t3);
                Rotate(-shrinkRotationSpeed);
                timer += Time.deltaTime;
                if (t3 >= 1f)
                {
                    timer = 0f;
                    transform.localRotation = Quaternion.identity;
                    currentState = State.Cooldown1;
                    OnShrinkFinished(); // Cooldown1開始時にReset再生
                }
                break;
        }
    }

    // --- 各フェーズ開始・終了時の音処理 ---

    void StartShortStretch()
    {
        PlayOneShot(shortStretchSound);
        timer = 0f;
        shortStretchStart = baseLocalPosition;
        shortStretchEnd = baseLocalPosition + Vector3.up * shortStretchDistance;
        currentState = State.ShortStretch;
    }

    void StartLongStretch()
    {
        StartLoopSequence(longStretchSound_1, longStretchSound_2);
        timer = 0f;
        targetLocalPosition = shortStretchEnd + Vector3.up * longStretchDistance;
        moveSpeed = longStretchDistance / longStretchDuration;
        currentState = State.LongStretch;
    }

    void OnLongStretchFinished()
    {
        StopLoopSound();
        PlayOneShot(collisionSound);
    }

    void StartShrinking()
    {
        StartLoopSequence(shrinkSound_1, shrinkSound_2);
        timer = 0f;
        targetLocalPosition = baseLocalPosition;
        currentState = State.Shrinking;
    }

    void OnShrinkFinished()
    {
        StopLoopSound();
        PlayOneShot(resetSound);
    }

    // --- オーディオ制御ヘルパー ---

    void PlayOneShot(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }

    void StartLoopSequence(AudioClip startClip, AudioClip loopClip)
    {
        StopLoopSound();
        loopSoundCoroutine = StartCoroutine(PlaySequenceCoroutine(startClip, loopClip));
    }

    IEnumerator PlaySequenceCoroutine(AudioClip startClip, AudioClip loopClip)
    {
        if (audioSource == null) yield break;

        audioSource.clip = startClip;
        audioSource.loop = false;
        audioSource.Play();

        yield return new WaitUntil(() => !audioSource.isPlaying);

        audioSource.clip = loopClip;
        audioSource.loop = true;
        audioSource.Play();
    }

    void StopLoopSound()
    {
        if (loopSoundCoroutine != null)
            StopCoroutine(loopSoundCoroutine);

        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.loop = false;
        }
    }

    // --- 既存の計算メソッド ---

    void Rotate(float speed)
    {
        transform.Rotate(Vector3.up, speed * Time.deltaTime, Space.Self);
    }

    bool TimerReached(float duration)
    {
        timer += Time.deltaTime;
        return timer >= duration;
    }
}