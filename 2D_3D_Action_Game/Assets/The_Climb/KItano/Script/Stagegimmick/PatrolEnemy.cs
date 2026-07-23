using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool startRight = true;

    [Header("プレイヤー検知")]
    [SerializeField] private float playerRayDistance = 8f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float maxChaseSpeed = 6f;
    [SerializeField] private float chaseKeepTime = 2f;

    [Header("壁検知")]
    [SerializeField] private float wallRayDistance = 0.5f;
    [SerializeField] private LayerMask wallLayer;

    private int direction;
    private float currentSpeed;
    private float chaseTimer;

    private void Start()
    {
        direction = startRight ? 1 : -1;
        currentSpeed = moveSpeed;
    }

    private void Update()
    {
        CheckWall();      // 進行方向のみ壁Ray
        DetectPlayer();   // 進行方向のみプレイヤーRay
        Move();
    }

    private void Move()
    {
        if (chaseTimer > 0)
        {
            chaseTimer -= Time.deltaTime;
        }
        else
        {
            currentSpeed = moveSpeed;
        }

        transform.Translate(Vector2.right * direction * currentSpeed * Time.deltaTime);
    }

    private void DetectPlayer()
    {
        Vector2 rayDir = Vector2.right * direction;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDir, playerRayDistance, playerLayer);
        Debug.DrawRay(transform.position, rayDir * playerRayDistance, Color.red);

        if (hit.collider != null)
        {
            float playerDir = Mathf.Sign(hit.collider.transform.position.x - transform.position.x);
            direction = (int)playerDir;

            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, moveSpeed, maxChaseSpeed);

            chaseTimer = chaseKeepTime;
        }
    }

    private void CheckWall()
    {
        Vector2 rayDir = Vector2.right * direction;

        RaycastHit2D wallHit = Physics2D.Raycast(transform.position, rayDir, wallRayDistance, wallLayer);
        Debug.DrawRay(transform.position, rayDir * wallRayDistance, Color.blue);

        if (wallHit.collider != null)
        {
            direction *= -1;
        }
    }
}
