using UnityEngine;
using UnityEngine.Audio;

public class Player_Bomb : MonoBehaviour
{
    [Header("Explosion")]
    [SerializeField] GameObject explosion;

    [Header("Force")]
    [SerializeField] float b_force = 10f;
    [SerializeField] float b_radius = 5f;
    [SerializeField] float b_upward = 0f;

    [Header("Timer")]
    [SerializeField] float b_time = 3f;

    [Header("Sound")]
    [SerializeField] AudioClip explosionSound;
    [SerializeField] AudioMixerGroup seMixerGroup;
    [SerializeField, Range(0f, 1f)] float explosionVolume = 1f;

    public int b_damage = 5;

    private float b_explosion = 0f;
    private bool exploded = false;
    private Vector3 b_pos;

    private System.Action onExploded;

    public void SetOnExplodedCallback(System.Action callback)
    {
        onExploded = callback;
    }

    void Update()
    {
        b_explosion += Time.deltaTime;

        if (b_explosion >= b_time && !exploded)
        {
            Explosion();
        }
    }

    public void ForceExplosion()
    {
        if (!exploded)
        {
            Explosion();
        }
    }

    void Explosion()
    {
        exploded = true;
        b_pos = transform.position;

        PlayParticle();
        PlayExplosionSound();
        ApplyExplosionForce();

        onExploded?.Invoke();

        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(0.15f, 0.11f);
        }

        Destroy(gameObject);
    }

    void PlayParticle()
    {
        if (explosion != null)
        {
            Instantiate(explosion, b_pos, Quaternion.identity);
        }
    }

    void PlayExplosionSound()
    {
        if (explosionSound == null) return;

        GameObject audioObj = new GameObject("ExplosionAudio");
        audioObj.transform.position = b_pos;

        AudioSource source = audioObj.AddComponent<AudioSource>();

        source.clip = explosionSound;
        source.volume = explosionVolume;
        source.spatialBlend = 0f;
        source.outputAudioMixerGroup = seMixerGroup;

        source.Play();

        Destroy(audioObj, explosionSound.length);
    }

    void ApplyExplosionForce()
    {
        Collider[] hitColliders = Physics.OverlapSphere(b_pos, b_radius);

        foreach (var hit in hitColliders)
        {
            GameObject obj = hit.gameObject;

            Rigidbody rb = obj.GetComponent<Rigidbody>();

            if (rb == null) continue;

            rb.AddExplosionForce(b_force, b_pos, b_radius, b_upward, ForceMode.Impulse);

            ObjExplosionTarget(obj);
        }
    }

    void ObjExplosionTarget(GameObject obj)
    {
        switch (obj.tag)
        {
            case "BreakingWall":

                Debug.Log("壁を発見");

                DestructibleBlock block = obj.GetComponent<DestructibleBlock>();

                if (block != null)
                    block.BreakBlock();

                break;

            case "Enemy":

                Enemy enemy = obj.GetComponent<Enemy>();

                if (enemy != null)
                {
                    enemy.TakeDamage(b_damage);
                    Debug.Log("爆風ヒット");
                }

                break;

            case "StalkerHand":

                obj.GetComponent<KidnapBuddy>()?.handController.ReleaseBuddy();

                break;

            case "BossStalker":

                BossStalkerHandController boss =
                    obj.transform.parent.GetComponent<BossStalkerHandController>();

                if (boss != null)
                    boss.BossStalkerSlow();

                break;
        }
    }
}