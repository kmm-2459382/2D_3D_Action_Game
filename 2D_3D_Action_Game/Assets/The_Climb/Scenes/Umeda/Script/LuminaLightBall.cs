using UnityEngine;
using UnityEngine.Audio;

public class LuminaLightBall : MonoBehaviour
{
    [Header("発射設定")]
    public GameObject lightBallPrefab;
    public Transform shootPoint;
    public Vector3 shootDirection = new Vector3(-1f, 1f, 0f);
    public float shootSpeed = 9.5f;

    [Header("バウンド設定")]
    public int maxBounces = 100;
    public float lifeTime = 5f;

    [Header("再生成設定")]
    public float respawnTime = 3.25f;
    // ★ 追加：最初だけのクールタイム
    public float startupDelay = 0f;

    [Header("サウンド設定")]
    public AudioClip bounceSound;
    [Range(0f, 1f)]
    public float volume = 0.5f; // インスペクターでのRangeに合わせて修正（元は50fになっていました）
    [SerializeField] AudioMixerGroup seMixerGroup;

    [Tooltip("音を鳴らす対象レイヤー（複数選択可）")]
    public LayerMask soundLayers;

    [Header("3Dサウンド距離設定")]
    public float minDistance = 8f;
    public float maxDistance = 30f;
    public AudioRolloffMode rolloffMode = AudioRolloffMode.Linear;

    private float timer;
    private float startupTimer = 0f;
    private bool isStarted = false;

    void Update()
    {
        if (lightBallPrefab == null || shootPoint == null) return;

        // ★ 最初だけの待機処理
        if (!isStarted)
        {
            startupTimer += Time.deltaTime;
            if (startupTimer >= startupDelay)
            {
                isStarted = true;
                SpawnBall(); // 待機明けに1発目を撃つ
                timer = 0f;
            }
            return; // 待機中はこれ以降の処理をしない
        }

        // 通常のループ処理
        timer += Time.deltaTime;
        if (timer >= respawnTime)
        {
            SpawnBall();
            timer = 0f;
        }
    }

    void SpawnBall()
    {
        GameObject ball = Instantiate(lightBallPrefab, shootPoint.position, Quaternion.identity);

        if (ball.TryGetComponent<Rigidbody>(out var rb))
        {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = shootDirection.normalized * shootSpeed;
#else
            rb.velocity = shootDirection.normalized * shootSpeed;
#endif
        }

        BallBehaviour behaviour = ball.AddComponent<BallBehaviour>();
        behaviour.maxBounces = maxBounces;
        behaviour.lifeTime = lifeTime;
        behaviour.bounceSound = bounceSound;
        behaviour.volume = volume;
        behaviour.soundLayers = soundLayers;
        behaviour.minDistance = minDistance;
        behaviour.maxDistance = maxDistance;
        behaviour.rolloffMode = rolloffMode;
        behaviour.seMixerGroup = seMixerGroup;
    }

    private class BallBehaviour : MonoBehaviour
    {
        public int maxBounces;
        public float lifeTime;
        public AudioClip bounceSound;
        public AudioMixerGroup seMixerGroup;
        public float volume;
        public LayerMask soundLayers;
        public float minDistance;
        public float maxDistance;
        public AudioRolloffMode rolloffMode;

        private int bounceCount = 0;
        private Collider ballCollider;

        void Start()
        {
            ballCollider = GetComponentInChildren<Collider>();
            Invoke(nameof(DisableColliderAndDestroy), lifeTime);
        }

        void OnCollisionEnter(Collision collision)
        {
            if (((1 << collision.gameObject.layer) & soundLayers) != 0)
            {
                PlayBounceSound();
            }

            bounceCount++;
            if (bounceCount >= maxBounces)
            {
                DisableColliderAndDestroy();
            }
        }

        void PlayBounceSound()
        {
            if (bounceSound == null) return;

            GameObject audioObj = new GameObject("LightBallSound");
            audioObj.transform.position = transform.position;

            AudioSource source = audioObj.AddComponent<AudioSource>();
            source.clip = bounceSound;
            source.volume = volume;
            source.outputAudioMixerGroup = seMixerGroup;
            source.spatialBlend = 1f;
            source.minDistance = minDistance;
            source.maxDistance = maxDistance;
            source.rolloffMode = rolloffMode;
            source.Play();

            Destroy(audioObj, bounceSound.length + 0.1f);
        }

        private void DisableColliderAndDestroy()
        {
            if (ballCollider != null)
                ballCollider.enabled = false;

            Destroy(gameObject, 0.1f);
        }
    }
}