using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    private enum State
    {
        Idle,
        Patrol,
        Charge,
        Tackle,
        Recovery
    }

    [Header("巡回ポイント")]
    [SerializeField] private Transform[] patrolPoints;

    [Header("設定")]
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private float loseSightTime = 3f;

    [Header("攻撃距離")]
    [SerializeField] private float attackDistance = 3f;

    [Header("攻撃判定")]
    [SerializeField] private Collider attackCollider;

    [Header("タックル設定")]
    [SerializeField] private float tackleSpeed = 10f;
    [SerializeField] private EnemyTackleBreaker breaker;
    private State currentState = State.Patrol;

    private NavMeshAgent agent;
    private Animator animator;
    private EnemyDetection detection;
    private Transform player;

    private int currentIndex = 0;
    private float waitTimer;
    private bool isWaiting = false;

    private Vector3 lastSeenDirection;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        detection = GetComponent<EnemyDetection>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (attackCollider != null)
            attackCollider.enabled = false;

        MoveToNextPoint();
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                UpdateIdle();
                break;
            case State.Patrol:
                UpdatePatrol();
                break;
            case State.Charge:
                UpdateCharge();
                break;
            case State.Tackle:
                UpdateTackle();
                break;
            case State.Recovery:
                UpdateRecovery();
                break;
        }

        // Speed制御修正版
        if (currentState == State.Patrol)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
        else if (currentState == State.Tackle)
        {
            animator.SetFloat("Speed", tackleSpeed);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }
    }
    private bool IsTackling()
    {
        return currentState == State.Tackle;
    }
    void StartIdle()
    {
        currentState = State.Idle;

        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        waitTimer = waitTime;
        animator.SetFloat("Speed", 0f);
    }
    void UpdateIdle()
{
    waitTimer -= Time.deltaTime;

    if (waitTimer <= 0f)
    {
        MoveToNextPoint();
        currentState = State.Patrol;
    }
}
    void UpdatePatrol()
    {
        if (detection.CanSeePlayer)
        {
            StartCharge();
            return;
        }

        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance &&
            agent.velocity.sqrMagnitude < 0.05f)
        {
            StartIdle();
        }

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
                MoveToNextPoint();
        }
    }

    void MoveToNextPoint()
    {
        if (patrolPoints.Length == 0) return;

        agent.isStopped = false;
        agent.updatePosition = true;
        agent.SetDestination(patrolPoints[currentIndex].position);

        currentIndex = (currentIndex + 1) % patrolPoints.Length;
        isWaiting = false;
    }

    void StartCharge()
    {
        currentState = State.Charge;

        agent.isStopped = true;
        agent.ResetPath();
        agent.velocity = Vector3.zero;
        agent.updatePosition = false;   // ★完全停止

        lastSeenDirection = (player.position - transform.position).normalized;
        lastSeenDirection.y = 0f;

        transform.forward = lastSeenDirection;

        animator.SetFloat("Speed", 0f);
        animator.SetTrigger("Charge");
    }

    void UpdateCharge()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (!stateInfo.IsName("Charge"))
        {
            StartTackle();
        }
    }

    void StartTackle()
    {
        breaker.StartTackleBreak();
        currentState = State.Tackle;

        agent.updatePosition = true;
        agent.velocity = Vector3.zero;
        agent.updateRotation = false;

        transform.forward = lastSeenDirection;

        animator.SetTrigger("Tackle");

        EnableAttack();
    }

    void UpdateTackle()
    {
        agent.Move(transform.forward * tackleSpeed * Time.deltaTime);

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (!stateInfo.IsName("Tackle"))
        {
            DisableAttack();
            breaker.StopTackleBreak();
            StartRecovery();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        // タックル中でなければ何もしない
        if (!IsTackling()) return;

        if (collision.gameObject.CompareTag("BreakingWall"))
        {
            var block = collision.gameObject.GetComponent<DestructibleBlock>();
            if (block != null)
            {
                block.BreakBlock();
            }
        }
    }
    void StartRecovery()
    {
        currentState = State.Recovery;

        agent.updateRotation = true;
        agent.velocity = Vector3.zero;

        animator.SetTrigger("Recovery");
    }

    void UpdateRecovery()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (!stateInfo.IsName("Recovery"))
        {
            if (detection.CanSeePlayer)
            {
                StartCharge();
            }
            else
            {
                agent.isStopped = false;
                currentState = State.Patrol;
                MoveToNextPoint();
            }
        }
    }

    void EnableAttack()
    {
        if (attackCollider != null)
            attackCollider.enabled = true;
    }

    void DisableAttack()
    {
        if (attackCollider != null)
            attackCollider.enabled = false;
    }
}