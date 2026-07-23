using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    enum State
    {
        Patrol,
        Chase,
        Search,
        Idle,
        Dash
    }

    private State currentState = State.Patrol;

    [Header("巡回範囲")]
    public float patrolRange = 5f; // 巡回幅
    private float leftLimit;
    private float rightLimit;

    [Header("移動")]
    public float moveSpeed = 2f;

    [Header("検知設定")]
    public Transform player;
    public float detectRange = 5f;   // 横方向検知距離
    public float heightRange = 2f;   // 高さ範囲

    [Header("見失い")]
    public float searchTime = 2f;
    private float searchTimer;

    [Header("待機時間")]
    public float idleTime = 1.5f;
    private float idleTimer;

    [Header("突進設定")]
    public float dashSpeed = 6f;      // 突進速度
    public float dashDuration = 0.5f; // 突進時間
    public float dashCooldown = 1.0f; // クールタイム
    private float dashTimer;
    private bool canDash = true;

    private int moveDir = 1; // 1:右 -1:左

    private Rigidbody rb;
    void Start()
    {
        leftLimit = transform.position.x - patrolRange;
        rightLimit = transform.position.x + patrolRange;
    
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                DetectPlayer();
                break;

            case State.Chase:
                Chase();
                CheckLostPlayer();
                break;

            case State.Search:
                Search();
                DetectPlayer();
                break;

            case State.Idle:
                Idle();
                DetectPlayer();
                break;

            case State.Dash:
                DashMove();
                break;
        }
    }

    // ----------------------
    // 巡回
    // ----------------------
    void Patrol()
    {
        Move(moveSpeed);

        // 巡回範囲内で左右反転
        
        if (transform.position.x > rightLimit)
        {
            moveDir = -1;
            FaceDirection();
        }
        else if (transform.position.x < leftLimit)
        {
            moveDir = 1;
            FaceDirection();
        }
    }

    // ----------------------
    // 横方向＋前方のみ検知
    // ----------------------
    void DetectPlayer()
    {
        float dx = player.position.x - transform.position.x;
        float dy = Mathf.Abs(player.position.y - transform.position.y);

        if (Mathf.Abs(dx) < detectRange && dy < heightRange && dx * moveDir > 0)
        {
            if (canDash)
            {
                currentState = State.Dash;
                dashTimer = dashDuration;
                canDash = false;
            }
            else
            {
                currentState = State.Chase;
                moveDir = (dx > 0) ? 1 : -1;
                FaceDirection();
            }
        }
    }

    // ----------------------
    // 追跡
    // ----------------------
    void Chase()
    {
        float dx = player.position.x - transform.position.x;
        moveDir = (dx > 0) ? 1 : -1;
        FaceDirection();
        Move(moveSpeed);
    }

    // ----------------------
    // 見失いチェック
    // ----------------------
    void CheckLostPlayer()
    {
        float dx = player.position.x - transform.position.x;
        float dy = Mathf.Abs(player.position.y - transform.position.y);

        if (Mathf.Abs(dx) > detectRange || dy > heightRange || dx * moveDir < 0)
        {
            currentState = State.Search;
            searchTimer = searchTime;
        }
    }

    // ----------------------
    // 見失い追跡
    // ----------------------
    void Search()
    {
        Move(moveSpeed);

        searchTimer -= Time.deltaTime;
        if (searchTimer <= 0f)
        {
            currentState = State.Idle;
            idleTimer = idleTime;
        }
    }

    // ----------------------
    // 停止（Idle）
    // ----------------------
    void Idle()
    {
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0f)
        {
            currentState = State.Patrol;
            canDash = true; // クールリセット
        }
    }

    // ----------------------
    // 突進移動
    // ----------------------
    void DashMove()
    {
        Move(dashSpeed);

        dashTimer -= Time.deltaTime;
        if (dashTimer <= 0f)
        {
            currentState = State.Chase; // 突進終了 → 通常追跡
            StartCoroutine(DashCooldownCoroutine());
        }
    }

    // ----------------------
    // クールタイム
    // ----------------------
    IEnumerator DashCooldownCoroutine()
    {
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    // ----------------------
    // 共通移動
    // ----------------------
    void Move(float speed)
    {
        rb.linearVelocity = new Vector3(moveDir * speed, rb.linearVelocity.y, 0);
    }
    void FaceDirection()
    {
        transform.forward = new Vector3(moveDir, 0, 0);
    }
}