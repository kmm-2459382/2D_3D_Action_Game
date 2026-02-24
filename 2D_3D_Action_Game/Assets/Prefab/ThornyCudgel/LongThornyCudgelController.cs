using UnityEngine;
using System.Collections;

public class LongThornyCudgelController : MonoBehaviour
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
    public float cooldown1 = 1f;
    public float shortStretchDuration = 0.2f;
    public float cooldown2 = 1f;
    public float longStretchDuration = 4f;
    public float cooldown3 = 1f;
    public float shrinkDuration = 6f;

    [Header("移動距離")]
    public float shortStretchDistance = 2f;
    public float longStretchDistance = 150f;

    [Header("回転速度（度/秒）")]
    public float stretchRotationSpeed = 360f;
    public float shrinkRotationSpeed = 180f;

    private enum State
    {
        InitialCooldown,
        Cooldown1,
        ShortStretch,
        Cooldown2,
        LongStretch,
        Cooldown3,
        Shrinking
    }

    private State currentState = State.InitialCooldown;
    private Vector3 baseLocalPosition;
    private Vector3 shortStretchEnd;
    private float timer = 0f;
    private float moveSpeed = 0f;
    private Vector3 targetLocalPosition;
    private float currentYRotation = 0f;
    private Coroutine loopSoundCoroutine;

    void Start()
    {
        baseLocalPosition = transform.localPosition;
        currentYRotation = transform.localEulerAngles.y;
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
                MoveTowardsTarget(() =>
                {
                    shortStretchEnd = transform.localPosition;
                    timer = 0f;
                    currentState = State.Cooldown2;
                });
                RotateContinuous(stretchRotationSpeed);
                break;

            case State.Cooldown2:
                if (TimerReached(cooldown2)) StartLongStretch();
                break;

            case State.LongStretch:
                MoveTowardsTarget(() =>
                {
                    timer = 0f;
                    currentState = State.Cooldown3;
                    OnLongStretchFinished();
                });
                RotateContinuous(stretchRotationSpeed);
                break;

            case State.Cooldown3:
                if (TimerReached(cooldown3)) StartShrinking();
                break;

            case State.Shrinking:
                MoveTowardsTarget(() =>
                {
                    timer = 0f;
                    currentState = State.Cooldown1;
                    currentYRotation = 0f;
                    ApplyRotation();
                    OnShrinkFinished();
                });
                RotateContinuous(-shrinkRotationSpeed);
                break;
        }
    }

    // --- 各フェーズ開始時の処理（音の再生） ---

    void StartShortStretch()
    {
        PlayOneShot(shortStretchSound);
        timer = 0f;
        targetLocalPosition = baseLocalPosition + Vector3.up * shortStretchDistance;
        moveSpeed = shortStretchDistance / shortStretchDuration;
        currentState = State.ShortStretch;
    }

    void StartLongStretch()
    {
        // Sound_1再生後、Sound_2をループ
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
        // Sound_1再生後、Sound_2をループ
        StartLoopSequence(shrinkSound_1, shrinkSound_2);

        timer = 0f;
        targetLocalPosition = baseLocalPosition;
        float totalDistance = Vector3.Distance(transform.localPosition, baseLocalPosition);
        moveSpeed = totalDistance / shrinkDuration;
        currentState = State.Shrinking;
    }

    void OnShrinkFinished()
    {
        StopLoopSound();
        PlayOneShot(resetSound);
    }

    // --- オーディオ制御用ヘルパー ---

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

        // Step 1: 最初の音を再生
        audioSource.clip = startClip;
        audioSource.loop = false;
        audioSource.Play();

        // Step 2: 最初の音が終わるまで待機
        yield return new WaitUntil(() => !audioSource.isPlaying);

        // Step 3: ループ音に切り替えて再生
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

    void MoveTowardsTarget(System.Action onComplete)
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetLocalPosition, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.localPosition, targetLocalPosition) < 0.01f)
            onComplete?.Invoke();
    }

    void RotateContinuous(float speed)
    {
        currentYRotation += speed * Time.deltaTime;
        currentYRotation %= 360f;
        ApplyRotation();
    }

    void ApplyRotation()
    {
        Vector3 euler = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(euler.x, currentYRotation, euler.z);
    }

    bool TimerReached(float duration)
    {
        timer += Time.deltaTime;
        return timer >= duration;
    }
}