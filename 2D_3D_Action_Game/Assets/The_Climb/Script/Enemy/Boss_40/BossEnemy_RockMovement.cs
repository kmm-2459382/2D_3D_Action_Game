using UnityEngine;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

[RequireComponent(typeof(Rigidbody))]
public class BossEnemy_RockMovement : MonoBehaviour
{
    public RockStats stats;
    [SerializeField] private Transform player;

    private Rigidbody rb;
    private int meteorHitCount = 0;
    
    private enum RockState
    {
        Crawling,
        PreparingLunge,
        Lunging,
        BitingSuccess,
        BitingFail_Stagger,
        Spinning,
        Throwing,
        Vulnerable,
        Dead
    }

    private RockState currentState = RockState.Crawling;
    private int crawlIndex = 0;
    private float stateTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentState = RockState.Crawling;
        Debug.Log("Rigidbody取得＋初期状態をCrawlingに設定");
    }

    private void Update()
    {
        Debug.Log("現在の状態（state）に応じた処理を呼び出す");
        switch (currentState)
        {
            case RockState.Crawling:
                PerformCrawling();
                break;

            case RockState.PreparingLunge:
                PerformPrepareLunge();
                break;

            case RockState.Lunging:
                PerformLunge();
                break;

            case RockState.BitingSuccess:
                
                break;

            case RockState.BitingFail_Stagger:
                
                break;

            case RockState.Vulnerable:
                // 待機中にメテオドロップを受け付ける
                break;

            case RockState.Dead:
                // 死亡処理
                break;
        }
    }

    void PerformCrawling()
    {
        if (stats.crawlPoints == null || stats.crawlPoints.Length == 0)
        {
            Debug.LogError("RockStats の crawlPoints にデータが入っていません！");
            return;
        }

        // インデックスが配列範囲外に行かないように防止
        crawlIndex = Mathf.Clamp(crawlIndex, 0, stats.crawlPoints.Length - 1);

        Vector3 target = stats.crawlPoints[crawlIndex];
        Vector3 dir = (target - transform.position).normalized;
        rb.linearVelocity = dir * stats.crawlSpeed;

        if (Vector3.Distance(transform.position, target) < 0.2f)
        {
            rb.linearVelocity = Vector3.zero;
            crawlIndex = (crawlIndex + 1) % stats.crawlPoints.Length;
            StartCoroutine(TransitionToLunge());
        }
        Debug.Log("指定された crawlPoints を順に移動。到達したら次は飛びつきへ");

    }

    IEnumerator TransitionToLunge()
    {
        currentState = RockState.PreparingLunge;
        yield return new WaitForSeconds(1f);
        currentState = RockState.Lunging;
        Debug.Log("1秒待って Lunging に移行");
    }

    void PerformPrepareLunge()
    {
        // 予備動作演出があればここで
        rb.linearVelocity = Vector3.zero;
        Debug.Log("（何もしないが将来的な演出枠）");
    }

    void PerformLunge()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * stats.lungeSpeed;
        Debug.Log("プレイヤーへ向かって突進");
        // 当たり判定は別オブジェクトや Trigger で管理推奨
        // 成功 or 失敗は OnTriggerEnter 等で処理する想定
    }

    public void OnBiteSuccess()
    {
        rb.linearVelocity = Vector3.zero;
        currentState = RockState.BitingSuccess;
        StartCoroutine(SpinAndThrow());
        Debug.Log("状態を BitingSuccess に。コルーチンで SpinAndThrow 処理へ");
    }

    public void OnBiteFail()
    {
        rb.linearVelocity = Vector3.zero;
        currentState = RockState.BitingFail_Stagger;
        StartCoroutine(StaggerRoutine());
        Debug.Log("状態を BitingFail_Stagger に。コルーチンで StaggerRoutine 処理へ");
    }

    IEnumerator SpinAndThrow()
    {
        // 回転演出
        yield return new WaitForSeconds(stats.spinDuration);
        // 吹っ飛ばし処理（プレイヤー方向に力を加える）
        currentState = RockState.Crawling;
        Debug.Log("回転 → 投げ → Crawling に戻る");
    }

    IEnumerator StaggerRoutine()
    {
        // よろめき演出
        yield return new WaitForSeconds(stats.staggerDuration);
        currentState = RockState.Vulnerable;
        Debug.Log("よろめき → Vulnerable へ移行");
    }

    public void OnMeteorDropHit()
    {
        if (currentState != RockState.Vulnerable) return;

        meteorHitCount++;
        if (meteorHitCount >= stats.maxMeteorHits)
        {
            currentState = RockState.Dead;
            rb.linearVelocity = Vector3.zero;
            Debug.Log("Rock defeated!");
        }
        else
        {
            currentState = RockState.Crawling;
        }
        Debug.Log("3回当てると Dead 状態に");
    }
}