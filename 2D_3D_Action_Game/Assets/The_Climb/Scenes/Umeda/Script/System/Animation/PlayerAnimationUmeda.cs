using UnityEngine;

public class PlayerAnimationUmeda : MonoBehaviour
{
    public Animator animator;
    private int spacePressCount = 0;
    private bool isJumping = false;
    private bool wasGrounded = true;

    private PlayerMove3D playerMove; // PlayerMove3Dに対応
    private PlayerState playerState;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    // 最後に向いた方向を保持する
    private Vector3 lastFacingDirection = Vector3.forward;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerMove = GameObject.Find("PlayerModel").GetComponent<PlayerMove3D>();
        playerState = GameObject.Find("PlayerModel").GetComponent<PlayerState>();

        if (animator == null)
            Debug.LogError("Animator が見つかりません。コンポーネントをアタッチしてください。");
    }

    void Update()
    {
        bool isGrounded = IsGrounded();

        // 着地検出
        if (isGrounded && !wasGrounded)
        {
            isJumping = false; // ジャンプロック解除
        }
        wasGrounded = isGrounded;

        // === PlayerMove3D から入力ベクトルを取得 ===
        Vector2 moveInput = playerMove.GetMoveInput(); // 新メソッドを使う
        Vector3 inputVector = new Vector3(moveInput.x, 0f, moveInput.y);

        // ランアニメーション判定
        bool isRunning = playerState.isGrounded && inputVector.sqrMagnitude > 0f;
        animator.SetBool("IsRunning", isRunning);

        // 向きの反映
        if (inputVector.sqrMagnitude > 0f)
        {
            lastFacingDirection = inputVector.normalized;
        }

        //// 常に最後に向いた方向を向く
        //transform.rotation = Quaternion.LookRotation(lastFacingDirection);

        // ジャンプアニメーション
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping)
        {
            spacePressCount++;
            int animStep = (spacePressCount - 1) % 3 + 1;
            isJumping = true; // ロック

            switch (animStep)
            {
                case 1: animator.SetTrigger("JumpAnimStep1"); break;
                case 2: animator.SetTrigger("JumpAnimStep2"); break;
                case 3: animator.SetTrigger("JumpAnimStep3"); break;
            }
        }
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }
}
