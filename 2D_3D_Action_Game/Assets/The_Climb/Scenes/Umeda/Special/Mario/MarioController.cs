using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class MarioController : MonoBehaviour
{
    [Header("移動")]
    public float moveSpeed = 5f;
    public float dashMultiplier = 1.5f;
    public float jumpForce = 7f;

    [Header("空中操作")]
    [Range(0f, 1f)]
    public float airTurnRate = 0.15f;

    [Header("接地判定")]
    public Transform groundCheck;
    public float groundRadius = 0.25f;
    public LayerMask groundLayer;

    [Header("重力調整")]
    public float fallGravityMultiplier = 2.5f;
    public float lowJumpMultiplier = 2.0f;

    [Header("ダッシュターン")]
    public float turnDuration = 0.05f;
    public float turnSlideForce = 1.2f;
    public float dashTurnMultiplier = 1.5f;

    [Header("モデル")]
    public GameObject idleModel;
    public GameObject walkModelA;
    public GameObject walkModelB;
    public GameObject jumpModel;
    public GameObject turnModel;
    public GameObject gameOverModel;

    [Header("歩行アニメ")]
    public float walkInterval = 0.15f;
    public float dashWalkSpeedMultiplier = 0.6f;

    Rigidbody rb;
    Vector3 baseScale;

    float moveInput;
    int lastDirection = 1;
    bool isGrounded;
    bool isFacingRight = true;
    bool isTurning;
    bool isDead;

    float walkTimer;
    int walkStep;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        baseScale = transform.localScale;
        ShowIdle();
    }

    void Update()
    {
        if (isDead) return;

        HandleInput();
        CheckGround();
        HandleJump();
        HandleFacingAndTurn(); // ★ ここが肝
        UpdateModel();
    }

    void FixedUpdate()
    {
        if (isDead) return;

        HandleMovement();
        ApplyBetterJump();
    }

    // ===== 入力 =====
    void HandleInput()
    {
        bool left = Input.GetKey(KeyCode.A);
        bool right = Input.GetKey(KeyCode.D);

        if (left && !right) lastDirection = -1;
        else if (right && !left) lastDirection = 1;

        moveInput = (left || right) ? lastDirection : 0;
    }

    // ===== 向き & ターン =====
    void HandleFacingAndTurn()
    {
        if (moveInput == 0 || isTurning) return;

        bool wantRight = moveInput > 0;

        // 向きが同じなら何もしない
        if (wantRight == isFacingRight) return;

        bool isDashing = Input.GetKey(KeyCode.LeftShift);
        bool canTurn =
            isDashing &&
            isGrounded &&
            Mathf.Abs(rb.linearVelocity.x) > moveSpeed * 0.8f;

        if (canTurn)
        {
            StartCoroutine(TurnCoroutine(wantRight));
        }
        else
        {
            // 通常の方向転換（即反転）
            isFacingRight = wantRight;
            ApplyFacing();
        }
    }

    IEnumerator TurnCoroutine(bool faceRight)
    {
        isTurning = true;

        DisableAllModels();
        turnModel.SetActive(true);

        float force = turnSlideForce * dashTurnMultiplier;
        rb.AddForce(
            new Vector3(isFacingRight ? force : -force, 0f, 0f),
            ForceMode.Impulse
        );

        yield return new WaitForSeconds(turnDuration);

        isFacingRight = faceRight;
        ApplyFacing();

        isTurning = false;
    }

    // ===== 移動 =====
    void HandleMovement()
    {
        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            speed *= dashMultiplier;

        float targetX = moveInput * speed;

        if (isGrounded)
        {
            rb.linearVelocity = new Vector3(targetX, rb.linearVelocity.y, 0f);
        }
        else
        {
            float newX = Mathf.Lerp(rb.linearVelocity.x, targetX, airTurnRate);
            rb.linearVelocity = new Vector3(newX, rb.linearVelocity.y, 0f);
        }
    }

    // ===== ジャンプ =====
    void HandleJump()
    {
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, 0f);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            ShowJump();
        }
    }

    // ===== 接地 =====
    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundRadius,
            groundLayer
        );
    }

    // ===== モデル =====
    void UpdateModel()
    {
        if (isTurning) return;

        if (!isGrounded)
        {
            ShowJump();
            return;
        }

        if (Mathf.Abs(moveInput) < 0.1f)
        {
            walkTimer = 0;
            walkStep = 0;
            ShowIdle();
            return;
        }

        float interval = walkInterval;
        if (Input.GetKey(KeyCode.LeftShift))
            interval *= dashWalkSpeedMultiplier;

        walkTimer += Time.deltaTime;
        if (walkTimer >= interval)
        {
            walkTimer = 0;
            walkStep = (walkStep + 1) % 4;
        }

        DisableAllModels();
        if (walkStep == 0 || walkStep == 2) idleModel.SetActive(true);
        else if (walkStep == 1) walkModelA.SetActive(true);
        else walkModelB.SetActive(true);
    }

    // ===== ジャンプ補正 =====
    void ApplyBetterJump()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y
                * (fallGravityMultiplier - 1f)
                * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 &&
                !Input.GetKey(KeyCode.Space) &&
                !Input.GetKey(KeyCode.W))
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y
                * (lowJumpMultiplier - 1f)
                * Time.fixedDeltaTime;
        }
    }

    // ===== 向き適用 =====
    void ApplyFacing()
    {
        transform.localScale = new Vector3(
            isFacingRight ? Mathf.Abs(baseScale.x) : -Mathf.Abs(baseScale.x),
            baseScale.y,
            baseScale.z
        );
    }

    // ===== 表示 =====
    void DisableAllModels()
    {
        idleModel.SetActive(false);
        walkModelA.SetActive(false);
        walkModelB.SetActive(false);
        jumpModel.SetActive(false);
        turnModel.SetActive(false);
        gameOverModel.SetActive(false);
    }

    void ShowIdle()
    {
        DisableAllModels();
        idleModel.SetActive(true);
    }

    void ShowJump()
    {
        DisableAllModels();
        jumpModel.SetActive(true);
    }

    public void GameOver()
    {
        isDead = true;
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        DisableAllModels();
        gameOverModel.SetActive(true);
    }
}
