using UnityEngine;

public class NewPlayerAnimation : MonoBehaviour
{
    [Header("参照設定")]
    [SerializeField] private Animator _animator;
    [Tooltip("キャラクターの最高速度（MotionSpeedを1にする基準値）")]
    [SerializeField] private float _maxMoveSpeed = 5.0f; // 環境に合わせて調整してください

    private PlayerMove _playerMove;
    private PlayerState _playerState;
    private Rigidbody _rb;

    private int _animIDSpeed;
    private int _animIDMotionSpeed;
    private int _animIDGrounded;
    private int _animIDJump;

    private bool _wasGrounded;

    void Start()
    {
        if (_animator == null) _animator = GetComponent<Animator>();

        _playerMove = GetComponentInParent<PlayerMove>();
        _playerState = GetComponentInParent<PlayerState>();
        _rb = GetComponentInParent<Rigidbody>();

        if (_playerMove == null || _playerState == null || _rb == null)
        {
            Debug.LogError("必須コンポーネントが親に見つかりません。");
        }

        AssignAnimationIDs();

        if (_playerState != null) _wasGrounded = _playerState.isGrounded;
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
    }

    void Update()
    {
        if (_animator == null || _playerState == null || _rb == null) return;

        UpdateMoveAnimation();
        UpdateGroundingAndJumpAnimation();
    }

    private void UpdateMoveAnimation()
    {
        // 1. 現在の速度（絶対値）を取得
        float inputSpeed = Mathf.Abs(_playerMove.MoveInput);
        float physicalSpeed = Mathf.Abs(_rb.linearVelocity.x);
        float currentSpeed = Mathf.Max(inputSpeed, physicalSpeed);

        // 2. MotionSpeed 用に 0〜1 に正規化
        // currentSpeed / _maxMoveSpeed が 1 を超えないように Clamp します
        float normalizedMotionSpeed = Mathf.Clamp01(currentSpeed / _maxMoveSpeed);

        // Animator への反映
        _animator.SetFloat(_animIDSpeed, currentSpeed); // Speed は元の値（BlendTreeの切り替え用）
        _animator.SetFloat(_animIDMotionSpeed, normalizedMotionSpeed); // 0〜1（再生速度用）

        // 3. 向きの制御
        if (_playerState.playerDirectionRight)
            transform.localRotation = Quaternion.Euler(0, 90, 0);
        else
            transform.localRotation = Quaternion.Euler(0, -90, 0);
    }

    private void UpdateGroundingAndJumpAnimation()
    {
        bool isGrounded = _playerState.isGrounded;
        _animator.SetBool(_animIDGrounded, isGrounded);

        if (!isGrounded && _wasGrounded)
        {
            if (_rb.linearVelocity.y > 0.1f)
            {
                _animator.SetBool(_animIDJump, true);
                _animator.CrossFadeInFixedTime("JumpStart", 0.15f);
            }
        }

        if (isGrounded && !_wasGrounded)
        {
            _animator.SetBool(_animIDJump, false);
            _animator.CrossFadeInFixedTime("Grounded", 0.05f);
        }

        _wasGrounded = isGrounded;
    }
}