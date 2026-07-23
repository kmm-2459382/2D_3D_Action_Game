using System.Collections.Generic;
using UnityEngine;

public class SkilTransparent : MonoBehaviour
{
    [Header("透明化の対象コライダー（後から増減可能）")]
    public List<Collider> triggerColliders = new List<Collider>();

    [Header("透明化の長さ(秒)")]
    public float transparentDuration = 2f;

    private Renderer rend;
    private bool isTransparent = false;
    private float transparentTimer = 0f;

    private bool isTouchingGround = false;

    // ★ リスポーン直後に無限ループするのを防ぐためのフラグ
    private bool canRespawnAfterTransparent = false;

    private PlayerRespawnUmeda respawn;

    void Start()
    {
        rend = GetComponent<Renderer>();
        respawn = GetComponent<PlayerRespawnUmeda>();
    }

    void Update()
    {
        // 透明化タイマーの更新
        if (isTransparent)
        {
            transparentTimer -= Time.deltaTime;
            if (transparentTimer <= 0f)
            {
                EndTransparent();
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isTouchingGround = true;

            // ★透明解除直後の1回だけリスポーンを許可
            if (canRespawnAfterTransparent)
            {
                TriggerRespawn();
            }
        }

        // 危険コライダーに触れた
        if (triggerColliders.Contains(collision.collider))
        {
            StartTransparent();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isTouchingGround = false;
        }
    }

    void StartTransparent()
    {
        if (isTransparent) return;
        
        isTransparent = true;
        transparentTimer = transparentDuration;

        // 再透明化したのでリスポーン許可フラグをリセット
        canRespawnAfterTransparent = false;

        Color c = rend.material.color;
        c.a = 0f;
        rend.material.color = c;
    }

    void EndTransparent()
    {
        isTransparent = false;

        // ★透明が終わった「直後の1回だけ」リスポーンできるようにする
        canRespawnAfterTransparent = true;

        Color c = rend.material.color;
        c.a = 1f;
        rend.material.color = c;

        // 透明解除した瞬間にすでに地面の上なら即リスポーン
        if (isTouchingGround)
        {
            TriggerRespawn();
        }
    }

    void TriggerRespawn()
    {
        if (respawn != null)
        {
            respawn.Respawn();
        }

        // ★リスポーンが1回行われたら、もう二度と連続で発動しない
        canRespawnAfterTransparent = false;
    }

}
