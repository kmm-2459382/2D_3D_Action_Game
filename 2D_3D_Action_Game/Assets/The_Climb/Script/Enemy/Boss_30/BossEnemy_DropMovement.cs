using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class BossEnemy_DropMovement : MonoBehaviour
{
    public DropStats stats;
    private Rigidbody rb;
    [SerializeField] private Transform player;

    private enum State
    {
        RushMove,
        FirstHover,
        Aiming,
        MeteorDrop,
        Rising,
        Waiting
    }

    private State currentState;

    private int RushCounter = 0;
    private int MeteorCounter = 0;
    private bool FirstHoverDone = false;
    private int AimMoveCounter = 0;
    private bool AimingToRight = true;
    private bool MovingToB = true;

    private bool hasDropped = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentState = State.RushMove;
        transform.position = new Vector3(stats.PointA_X, stats.GroundY, 0f);
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.RushMove:
                PerformRushMove();
                break;

            case State.FirstHover:
                StartCoroutine(FirstHoverCoroutine());
                break;

            case State.MeteorDrop:
                PerformMeteorDrop();
                break;

            case State.Rising:
                PerformRising();
                break;

            case State.Waiting:
                // コルーチン中の待機
                break;

            case State.Aiming:
                PerformAiming();
                break;
        }
    }

    void PerformRushMove()
    {
        float targetX = MovingToB ? stats.PointB_X : stats.PointA_X;
        float direction = Mathf.Sign(targetX - transform.position.x);
        rb.linearVelocity = new Vector3(direction * stats.RushSpeed, 0f, 0f);

        if (Mathf.Abs(transform.position.x - targetX) < 0.2f)
        {
            RushCounter++;
            rb.linearVelocity = Vector3.zero;

            if (RushCounter >= stats.DiagonalRushCount)
            {
                if (!FirstHoverDone)
                {
                    currentState = State.FirstHover;
                }
                else
                {
                    MeteorCounter = 0;
                    currentState = State.Rising;
                }
            }
            else
            {
                MovingToB = !MovingToB;
            }
        }
    }

    IEnumerator FirstHoverCoroutine()
    {
        currentState = State.Waiting;
        FirstHoverDone = true;

        transform.position = new Vector3(transform.position.x, stats.HoverHeight, 0f);
        yield return new WaitForSeconds(stats.WaitBeforeMeteor);

        AimMoveCounter = 0;
        AimingToRight = true;
        currentState = State.Aiming;
    }

    void PerformAiming()
    {
        float targetX = AimingToRight ? stats.AimMoveRightX : stats.AimMoveLeftX;
        float direction = Mathf.Sign(targetX - transform.position.x);
        rb.linearVelocity = new Vector3(direction * stats.AimMoveSpeed, 0f, 0f);

        if (Mathf.Abs(transform.position.x - targetX) < 0.2f)
        {
            AimMoveCounter++;
            rb.linearVelocity = Vector3.zero;

            if (AimMoveCounter >= stats.AimMoveCount)
            {
                StartCoroutine(WaitAndDrop());
            }
            else
            {
                AimingToRight = !AimingToRight;
            }
        }
    }

    IEnumerator WaitAndDrop()
    {
        currentState = State.Waiting;
        yield return new WaitForSeconds(stats.WaitBeforeMeteor);
        currentState = State.MeteorDrop;
        hasDropped = false; // Drop前にリセット
    }

    void PerformMeteorDrop()
    {
        if (hasDropped) return;

        hasDropped = true;
        Vector3 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * stats.MeteorDropSpeed;
    }

    void PerformRising()
    {
        rb.linearVelocity = new Vector3(0f, stats.RiseSpeed, 0f);
            
        if (transform.position.y >= stats.HoverHeight)
        {
            rb.linearVelocity = Vector3.zero;
            currentState = State.MeteorDrop;
            hasDropped = false; // Rising経由のMeteorDropでも一度だけ落下
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (currentState == State.MeteorDrop && collision.gameObject.CompareTag("Ground"))
        {
            rb.linearVelocity = Vector3.zero;
            hasDropped = false;
            MeteorCounter++;

            if (MeteorCounter >= stats.meteorDropCount)
            {
                RushCounter = 0;
                currentState = State.RushMove;
            }
            else
            {
                StartCoroutine(WaitAndRise());
            }
        }
    }

    IEnumerator WaitAndRise()
    {
        currentState = State.Waiting;
        yield return new WaitForSeconds(stats.WaitBeforeMeteor);
        currentState = State.Rising;
    }
}
