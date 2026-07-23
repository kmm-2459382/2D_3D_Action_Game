using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;
    private int spacePressCount = 0;
    private bool isJumping = false;
    private bool wasGrounded = true;

    private PlayerMove PlayerMove;
    private PlayerState PlayerState;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask lowGroundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    void Start()
    {
        animator = GetComponent<Animator>();
        PlayerMove = GameObject.Find("PlayerModel").GetComponent<PlayerMove>();
        PlayerState = GameObject.Find("PlayerModel").GetComponent<PlayerState>();

        if (animator == null)
            Debug.LogError("Animator が見つかりません。コンポーネントをアタッチしてください。");
    }

    void Update()
    {
        bool isGrounded = IsGrounded();

        // 着地検出
        if (isGrounded && !wasGrounded)
        {
            //Debug.Log("着地しました。次のジャンプが可能になります。");
            isJumping = false; // 再解禁
        }
        wasGrounded = isGrounded;

        // ランアニメ
        bool isRunning = PlayerState.isGrounded && Mathf.Abs(PlayerMove.MoveInput) > 0f;
        animator.SetBool("IsRunning", isRunning);

        // 向き
        if (PlayerMove.MoveInput > 0f)
            transform.rotation = Quaternion.Euler(0, 90, 0);
        else if (PlayerMove.MoveInput < 0f)
            transform.rotation = Quaternion.Euler(0, -90, 0);

        // ジャンプアニメ：空中ではロック
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping)
        {
            spacePressCount++;
            int animStep = (spacePressCount - 1) % 3 + 1;
            //Debug.Log("ジャンプ Step " + animStep);
            isJumping = true; // ロックをかける

            animator.SetTrigger("JumpAnimStep1");
        }
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer) || Physics.CheckSphere(groundCheck.position, groundCheckRadius, lowGroundLayer);
    }
}