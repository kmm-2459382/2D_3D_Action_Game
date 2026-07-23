using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class BossEnemy_HevvyMovement : MonoBehaviour
{
    private enum BossState
    {
        Idle, Move, ChargeVertical, JumpVertical,
        ChargeArc, JumpArc, Stunned, Defeated
    }
    
    [Header("参照")]
    public Transform player;
    public Animator animator;
    public HevvyStats stats;

    [Header("起動条件")]
    public float activationDistance = 10f;

    private BossState currentState;
    private int hitCount = 0;
    private bool isVulnerable = false;
    private Rigidbody rb;
    private bool hasActivated = false;
    public bool IsVulnerable() => isVulnerable;

    private bool isInvincible = false;
    private Renderer bossRenderer;
    private Coroutine flashCoroutine;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        bossRenderer = GetComponentInChildren<Renderer>(); // モデルの Renderer を取得

        Debug.Log("[Boss] 初期化完了");
        ChangeState(BossState.Idle);
    }

    void Update()
    {
        if (!hasActivated && currentState == BossState.Idle)
        {
            float dist = Vector3.Distance(player.position, transform.position);
            Debug.Log($"[Boss] Idle中：プレイヤーとの距離 = {dist:F2}");

            if (dist <= activationDistance)
            {
                Debug.Log("[Boss] プレイヤーが接近 → 起動");
                ChangeState(BossState.Move);
                hasActivated = true;
            }
        }
    }

    void ChangeState(BossState newState)
    {
        Debug.Log($"[Boss] 状態遷移: {currentState} → {newState}");
        currentState = newState;

        switch (newState)
        {
            case BossState.Idle:
                PlayAnimation("Idle");
                break;
            case BossState.Move:
                StartCoroutine(HopMoveRoutine()); // ← ここに移動させる
                break;
            case BossState.ChargeVertical:
                PlayAnimation("Charge");
                StartCoroutine(ChargeThenJumpVertical());
                break;
            case BossState.ChargeArc:
                PlayAnimation("Charge");
                StartCoroutine(ChargeThenJumpArc());
                break;
            case BossState.Defeated:
                PlayAnimation("Defeat");
                Debug.Log("[Boss] 撃破されました");
                Destroy(gameObject, 2f);
                break;
            case BossState.Stunned:
                PlayAnimation("Stun");
                StartCoroutine(StunRoutine());
                break;
        }
    }

    IEnumerator HopMoveRoutine()
    {
        PlayAnimation("HopMove");

        for (int i = 0; i < stats.hopMoveCount; i++)
        {
            // プレイヤー方向へジャンプベクトルを算出
            Vector3 direction = (player.position - transform.position).normalized;
            Vector3 horizontal = new Vector3(direction.x, 0, direction.z);

            rb.linearVelocity = horizontal * stats.hopMoveForce + Vector3.up * stats.hopMoveHeight;

            Debug.Log($"[Boss] ホップ移動 {i + 1}/{stats.hopMoveCount}");

            // 落下するまで待つ
            yield return new WaitUntil(() => rb.linearVelocity.y <= 0);
            // 着地するまで待つ
            yield return new WaitUntil(() => IsGrounded());

            // 少し待ってから次のホップ（必要に応じて調整）
            yield return new WaitForSeconds(0.2f);
        }

        Debug.Log("[Boss] ホップ移動完了 → 状態遷移チェック");
        CheckForTransition();
    }

    void CheckForTransition()
    {
        float dist = Vector3.Distance(player.position, transform.position);
        Debug.Log($"[Boss] プレイヤーとの距離: {dist:F2}");

        if (dist < stats.nearTriggerDistance)
        {
            Debug.Log("[Boss] プレイヤーが近い → 垂直チャージへ");
            ChangeState(BossState.ChargeVertical);
        }
        else if (dist > stats.farTriggerDistance)
        {
            Debug.Log("[Boss] プレイヤーが遠い → 山なりチャージへ");
            ChangeState(BossState.ChargeArc);
        }
    }

    IEnumerator ChargeThenJumpVertical()
    {
        Debug.Log("[Boss] 垂直ジャンプ チャージ開始");
        yield return new WaitForSeconds(stats.chargeTime);

        PlayAnimation("JumpVertical");

        // プレイヤー方向へのわずかな横移動ベクトルを追加
        Vector3 targetPos = player.position;
        Vector3 direction = (targetPos - transform.position).normalized;

        // 横方向に移動したい力（控えめに）
        Vector3 horizontalForce = new Vector3(direction.x, 0, direction.z) * stats.smallHopForce;

        // 垂直ジャンプ + 横方向へのわずかな力
        rb.linearVelocity = horizontalForce + Vector3.up * stats.verticalJumpForce;

        Debug.Log("[Boss] 垂直ジャンプ 実行");

        // Wait until falling then grounded
        yield return new WaitUntil(() => rb.linearVelocity.y <= 0);
        yield return new WaitUntil(() => IsGrounded());

        Debug.Log("[Boss] 垂直ジャンプ終了 → スタンへ");
        ChangeState(BossState.Stunned); // ← スタン状態へ移行
    }
    IEnumerator StunRoutine()
    {
        Debug.Log("[Boss] スタン状態突入");

        rb.linearVelocity = Vector3.zero;
        isVulnerable = true;

        var weakPoint = GetComponentInChildren<BossWeakPoint>();
        if (weakPoint != null)
        {
            Debug.Log("[Boss] 弱点をアクティブ化");
            weakPoint.ActivateWeakPoint();
        }
        else
        {
            Debug.LogWarning("[Boss] BossWeakPoint が見つかりませんでした！");
        }

        yield return new WaitForSeconds(stats.stunDuration);

        isVulnerable = false;
        if (weakPoint != null)
        {
            Debug.Log("[Boss] 弱点を非アクティブ化");
            weakPoint.DeactivateWeakPoint();
        }

        Debug.Log("[Boss] スタン解除 → 移動へ");
        ChangeState(BossState.Move);
    }

    IEnumerator ChargeThenJumpArc()
    {
        Debug.Log("[Boss] 山なりジャンプ チャージ開始");
        yield return new WaitForSeconds(stats.chargeTime);

        PlayAnimation("JumpArc");

        Vector3 targetPos = player.position;
        Vector3 direction = (targetPos - transform.position).normalized;
        rb.linearVelocity = direction * stats.arcJumpForce + Vector3.up * stats.arcJumpHeight;

        isVulnerable = true;
        GetComponentInChildren<BossWeakPoint>()?.ActivateWeakPoint();

        yield return new WaitUntil(() => rb.linearVelocity.y <= 0);
        yield return new WaitUntil(() => IsGrounded());

        isVulnerable = false;
        GetComponentInChildren<BossWeakPoint>()?.DeactivateWeakPoint();

        Debug.Log("[Boss] 山なりジャンプ終了 → 移動へ");
        ChangeState(BossState.Move);
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    void PlayAnimation(string animName)
    {
        if (animator != null && !string.IsNullOrEmpty(animName))
        {
            animator.Play(animName);
            Debug.Log($"[Boss] アニメーション再生: {animName}");
        }
        else
        {
            Debug.Log($"[Boss] アニメーションスキップ（未設定）: {animName}");
        }
    }
    public void OnHit()
    {
        if (!isVulnerable || isInvincible) return; // 無敵中 or 非スタン中は無視

        hitCount++;
        Debug.Log($"[Boss] 弱点ヒット！残り: {stats.requiredHitsToDefeat - hitCount}");

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(DamageFlash());

        if (hitCount >= stats.requiredHitsToDefeat)
        {
            ChangeState(BossState.Defeated);
        }
    }
    IEnumerator DamageFlash()
    {
        isInvincible = true;

        if (bossRenderer != null)
        {
            Color originalColor = bossRenderer.material.color;
            Color flashColor = Color.red;

            float flashDuration = 0.1f;
            int flashCount = 5;

            for (int i = 0; i < flashCount; i++)
            {
                bossRenderer.material.color = flashColor;
                yield return new WaitForSeconds(flashDuration);
                bossRenderer.material.color = originalColor;
                yield return new WaitForSeconds(flashDuration);
            }
        }

        yield return new WaitForSeconds(1.0f); // 無敵時間
        isInvincible = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (currentState == BossState.JumpArc && collision.contacts[0].normal.y > 0.5f)
        {
            Debug.Log("[Boss] 地面に着地（保険的遷移）");
            ChangeState(BossState.Move);
        }
    }
    
    // Gizmosで可視化
    private void OnDrawGizmosSelected()
    {
        if (stats == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, activationDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stats.nearTriggerDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stats.farTriggerDistance);
    }
}