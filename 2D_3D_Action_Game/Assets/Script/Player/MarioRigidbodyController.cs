using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(MyPlayerInput))] // 自作インプットを必須にする
public class MarioRigidbodyController : MonoBehaviour
{
    [Header("Player - Movement")]
    public float MoveSpeed = 8.0f;
    public float SprintSpeed = 14.0f;
    public float AccelerationRate = 5.0f;
    public float DecelerationRate = 10.0f;
    [Range(0.0f, 0.3f)] public float RotationSmoothTime = 0.12f;

    [Header("Turn Settings")]
    public float TurnStopDuration = 0.02f;
    private float _turnStopTimer;
    private Vector2 _lastMoveInput;

    [Header("Mario Jump Physics")]
    public float JumpHeight = 2.5f;
    public float Gravity = -30.0f;
    public float MaxJumpHeldTime = 0.4f;
    public float FallMultiplier = 2f;
    public float JumpCutMultiplier = 0.5f;

    [Header("Air Control")]
    [Range(0f, 1f)] public float AirControl = 0.2f;

    [Header("Player Grounded")]
    public bool Grounded = true;
    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.28f;
    public LayerMask GroundLayers;

    [Header("Water Settings")]
    public float WaterMoveSpeed = 4.0f;
    public float WaterSprintSpeed = 7.0f;
    public float WaterVerticalSpeed = 4.0f;
    private bool _isInWater = false;
    public float WaterSurfaceDetectionHeight = 0.98f;

    [Header("Water Settings - Physics")]
    public float WaterVerticalAcceleration = 10.0f;

    [Header("Collider Settings (Capsule)")]
    public float DefaultColliderHeight = 1.5f;
    public Vector3 DefaultColliderCenter = new Vector3(0, 0.75f, 0);

    [Header("Water Collider Settings")]
    public float WaterIdleHeight = 1.5f;
    public Vector3 WaterIdleCenter = new Vector3(0, 0.75f, 0);
    public float WaterSwimHeight = 0.6f;
    public Vector3 WaterSwimCenter = new Vector3(0, 0.3f, 0);

    [Header("Camera Settings (No Cinemachine)")]
    public Transform CameraPivot;
    public float CameraDistance = 5.0f;
    public float TopClamp = 70.0f;
    public float BottomClamp = -30.0f;
    public float CameraAngleOverride = 0.0f;

    [Header("Settings & Audio")]
    public float JumpTimeout = 0.1f;
    public float FallTimeout = 0.15f;
    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    // 内部参照
    private Rigidbody _rigidbody;
    private CapsuleCollider _collider;
    private GameObject _mainCamera;
    private Animator _animator;
    private MyPlayerInput _input; // 自作インプットの参照
    private bool _hasAnimator;

    private float _cameraYaw, _cameraPitch;
    private float _verticalVelocity, _speed, _animationBlend, _targetRotation, _rotationVelocity, _jumpTimeoutDelta, _fallTimeoutDelta, _jumpButtonHeldTime;
    private float _terminalVelocity = 53.0f;
    private bool _isJumpProcessing, _isJumpInputReady = true;
    private Vector3 _horizontalVelocity;

    private int _animIDSpeed, _animIDGrounded, _animIDJump, _animIDFreeFall, _animIDMotionSpeed, _animIDInWater;

    private void Awake()
    {
        if (_mainCamera == null) _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        _input = GetComponent<MyPlayerInput>(); // インプットスクリプトを取得

        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        _rigidbody.useGravity = false;
    }

    private void Start()
    {
        _hasAnimator = TryGetComponent(out _animator);

        if (CameraPivot != null)
        {
            _cameraYaw = CameraPivot.rotation.eulerAngles.y;
            _cameraPitch = CameraPivot.rotation.eulerAngles.x;
        }

        AssignAnimationIDs();
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }

    private void Update()
    {
        GroundedCheck();
        JumpAndGravity();
        MoveCalculation();
        UpdateCollider();
    }

    private void FixedUpdate()
    {
        Vector3 targetVelocity = _horizontalVelocity + Vector3.up * _verticalVelocity;

#if UNITY_2023_1_OR_NEWER
        _rigidbody.linearVelocity = targetVelocity;
#else
        _rigidbody.velocity = targetVelocity;
#endif
    }

    private void LateUpdate()
    {
        CameraRotationAndPosition();
    }

    private void UpdateCollider()
    {
        if (!_isInWater)
        {
            _collider.height = Mathf.Lerp(_collider.height, DefaultColliderHeight, Time.deltaTime * 5f);
            _collider.center = Vector3.Lerp(_collider.center, DefaultColliderCenter, Time.deltaTime * 5f);
            return;
        }

        float swimWeight = Mathf.Clamp01(_animationBlend / WaterMoveSpeed);
        float targetHeight = Mathf.Lerp(WaterIdleHeight, WaterSwimHeight, swimWeight);
        Vector3 targetCenter = Vector3.Lerp(WaterIdleCenter, WaterSwimCenter, swimWeight);

        _collider.height = Mathf.Lerp(_collider.height, targetHeight, Time.deltaTime * 5f);
        _collider.center = Vector3.Lerp(_collider.center, targetCenter, Time.deltaTime * 5f);
    }

    private void GroundedCheck()
    {
        float surfaceHeightOffset = 1.35f;
        bool isAboveSurface = !Physics.CheckSphere(transform.position + Vector3.up * surfaceHeightOffset, 0.1f, LayerMask.GetMask("Water"), QueryTriggerInteraction.Collide);
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        bool wasGrounded = Grounded;
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

        if (_hasAnimator)
        {
            bool animatorGrounded = (_isInWater && !isAboveSurface) ? false : Grounded;
            _animator.SetBool(_animIDGrounded, animatorGrounded);
            if (Grounded && !wasGrounded && !_isInWater)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
                if (!_input.Jump) _animator.Play("JumpLand", 0, 0f);
            }
        }
    }

    private void JumpAndGravity()
    {
        if (_isInWater)
        {
            _fallTimeoutDelta = FallTimeout;
            float targetVerticalSpeed = 0;
            bool isNearSurface = !Physics.CheckSphere(transform.position + Vector3.up * WaterSurfaceDetectionHeight, 0.3f, LayerMask.GetMask("Water"), QueryTriggerInteraction.Collide);
            if (!_input.Jump) _isJumpInputReady = true;

            if (_input.Jump)
            {
                if (isNearSurface && _isJumpInputReady)
                {
                    _isInWater = false;
                    _isJumpInputReady = false;
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                    _jumpTimeoutDelta = JumpTimeout;
                    _isJumpProcessing = true;
                    _jumpButtonHeldTime = 0.0f;
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDInWater, false);
                        _animator.SetBool(_animIDJump, true);
                        _animator.Play("JumpStart", 0, 0f);
                    }
                    return;
                }
                else if (isNearSurface && !_isJumpInputReady)
                {
                    targetVerticalSpeed = 0f;
                    if (_verticalVelocity > 0) _verticalVelocity = 0;
                }
                else
                {
                    targetVerticalSpeed = WaterVerticalSpeed;
                    _isJumpInputReady = false;
                }
            }
            else
            {
                targetVerticalSpeed = Grounded ? -1.0f : 0f;
#if ENABLE_INPUT_SYSTEM
                if (Keyboard.current != null && Keyboard.current.ctrlKey.isPressed)
                    targetVerticalSpeed = -WaterVerticalSpeed;
#endif
            }
            _verticalVelocity = Mathf.Lerp(_verticalVelocity, targetVerticalSpeed, Time.deltaTime * WaterVerticalAcceleration);
            return;
        }

        if (Grounded)
        {
            _fallTimeoutDelta = FallTimeout;
            if (_hasAnimator) { _animator.SetBool(_animIDJump, false); _animator.SetBool(_animIDFreeFall, false); }
            if (!_input.Jump) _isJumpInputReady = true;
            if (_input.Jump && _isJumpInputReady && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                if (_hasAnimator) { _animator.SetBool(_animIDJump, true); _animator.Play("JumpStart", 0, 0f); }
                _isJumpProcessing = true;
                _isJumpInputReady = false;
                _jumpButtonHeldTime = 0.0f;
                _jumpTimeoutDelta = JumpTimeout;
            }
            if (_jumpTimeoutDelta >= 0.0f) _jumpTimeoutDelta -= Time.deltaTime;
            if (_verticalVelocity < 0.0f) _verticalVelocity = -2f;
        }
        else
        {
            _jumpTimeoutDelta = JumpTimeout;
            if (_fallTimeoutDelta >= 0.0f) _fallTimeoutDelta -= Time.deltaTime;
            else if (_hasAnimator) _animator.SetBool(_animIDFreeFall, true);
            if (!_input.Jump) _isJumpInputReady = true;
        }

        float currentGravity = Gravity;
        if (_verticalVelocity < 0) currentGravity *= FallMultiplier;
        else if (_verticalVelocity > 0 && _isJumpProcessing)
        {
            if (_input.Jump)
            {
                _jumpButtonHeldTime += Time.deltaTime;
                currentGravity *= 0.6f;
                if (_jumpButtonHeldTime > MaxJumpHeldTime) _isJumpProcessing = false;
            }
            else
            {
                _verticalVelocity *= JumpCutMultiplier;
                _isJumpProcessing = false;
            }
        }
        if (_verticalVelocity > -_terminalVelocity) _verticalVelocity += currentGravity * Time.deltaTime;
    }

    private void MoveCalculation()
    {
        // 安全対策：もしインプット参照がNullなら、エラーで止まらずに処理を抜ける
        if (_input == null) return;

        float targetSpeed;
        float inputMagnitude = _input.Move.magnitude;

        if (_isInWater)
        {
            targetSpeed = _input.Sprint ? WaterSprintSpeed : WaterMoveSpeed;
            if (_input.Move == Vector2.zero) targetSpeed = 0.0f;
            if (_input.Move != Vector2.zero)
            {
                Vector3 inputDirection = new Vector3(_input.Move.x, 0.0f, _input.Move.y).normalized;
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            }
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            Vector3 targetInputDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            _horizontalVelocity = Vector3.Lerp(_horizontalVelocity, targetInputDirection * targetSpeed * inputMagnitude, Time.deltaTime * AccelerationRate);
            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * AccelerationRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                float motionSpeed = (_input.Sprint && _input.Move != Vector2.zero) ? 1.5f : 1.0f;
                _animator.SetFloat(_animIDMotionSpeed, motionSpeed);
            }
        }
        else
        {
            targetSpeed = _input.Sprint ? SprintSpeed : MoveSpeed;
            if (_input.Move == Vector2.zero) targetSpeed = 0.0f;
            if (Grounded)
            {
                if (_input.Move != _lastMoveInput && _input.Move != Vector2.zero) _turnStopTimer = TurnStopDuration;
                _lastMoveInput = _input.Move;
                if (_turnStopTimer > 0) { _turnStopTimer -= Time.deltaTime; targetSpeed = 0f; }
                if (_input.Move != Vector2.zero)
                {
                    Vector3 inputDirection = new Vector3(_input.Move.x, 0.0f, _input.Move.y).normalized;
                    _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
                }
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

#if UNITY_2023_1_OR_NEWER
                Vector3 currentVel = _rigidbody.linearVelocity;
#else
                Vector3 currentVel = _rigidbody.velocity;
#endif
                float currentHorizontalSpeed = new Vector3(currentVel.x, 0.0f, currentVel.z).magnitude;
                float currentRate = (targetSpeed > currentHorizontalSpeed) ? AccelerationRate : DecelerationRate;
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * currentRate);
                _horizontalVelocity = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward * _speed;
            }
            else
            {
                if (_input.Move != Vector2.zero)
                {
                    Vector3 inputDirection = new Vector3(_input.Move.x, 0.0f, _input.Move.y).normalized;
                    _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
                }
                float rotSpeed = RotationSmoothTime / Mathf.Max(0.1f, AirControl);
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, rotSpeed);
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                Vector3 targetInputDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
                _horizontalVelocity = Vector3.Lerp(_horizontalVelocity, targetInputDirection * targetSpeed * inputMagnitude, Time.deltaTime * AccelerationRate * AirControl);
            }
            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * AccelerationRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }
    }

    private void ExitWaterMode()
    {
        _isInWater = false;
        _isJumpInputReady = false;
        if (_hasAnimator) _animator.SetBool(_animIDInWater, false);
    }

    private void OnTriggerStay(Collider foreign)
    {
        if (foreign.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            if (_verticalVelocity > 0 && !_isInWater) return;
            if (!_isInWater)
            {
                _isInWater = true;
                if (_hasAnimator) _animator.SetBool(_animIDInWater, true);
            }
        }
    }

    private void OnTriggerEnter(Collider foreign)
    {
        if (foreign.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            _isInWater = true;
            _isJumpInputReady = false;
            if (_hasAnimator) _animator.SetBool(_animIDInWater, true);
        }
    }

    private void OnTriggerExit(Collider foreign)
    {
        if (foreign.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            ExitWaterMode();
        }
    }

    private void CameraRotationAndPosition()
    {
        if (CameraPivot == null || _mainCamera == null) return;

        // 自作インプットのLookを使用
        if (_input.Look.sqrMagnitude >= 0.01f)
        {
            // マウス使用時は感度を少しマイルドに調整
            bool isMouse = false;
#if ENABLE_INPUT_SYSTEM
            if (PlayerInput.all.Count > 0)
            {
                isMouse = PlayerInput.all[0].currentControlScheme == "KeyboardMouse";
            }
#endif
            float deltaTimeMultiplier = isMouse ? 0.1f : Time.deltaTime * 50f;
            _cameraYaw += _input.Look.x * deltaTimeMultiplier;
            _cameraPitch += _input.Look.y * deltaTimeMultiplier;
        }

        _cameraYaw = ClampAngle(_cameraYaw, float.MinValue, float.MaxValue);
        _cameraPitch = ClampAngle(_cameraPitch, BottomClamp, TopClamp);

        CameraPivot.position = transform.position;
        CameraPivot.rotation = Quaternion.Euler(_cameraPitch + CameraAngleOverride, _cameraYaw, 0.0f);

        Vector3 targetCamPosition = CameraPivot.position - (CameraPivot.forward * CameraDistance);
        _mainCamera.transform.position = targetCamPosition;
        _mainCamera.transform.LookAt(CameraPivot.position + Vector3.up * 1.0f);
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        _animIDInWater = Animator.StringToHash("InWater");
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Grounded ? new Color(0, 1, 0, 0.35f) : new Color(1, 0, 0, 0.35f);
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f && FootstepAudioClips.Length > 0)
        {
            var index = Random.Range(0, FootstepAudioClips.Length);
            AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.position, FootstepAudioVolume);
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f && LandingAudioClip != null)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.position, FootstepAudioVolume);
        }
    }
}