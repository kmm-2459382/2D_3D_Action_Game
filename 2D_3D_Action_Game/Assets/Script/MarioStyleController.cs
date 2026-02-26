using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class MarioStyleController : MonoBehaviour
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
        [Range(0f, 1f)] public float TopFloatMultiplier = 1f;
        [Range(0f, 10f)] public float TopFloatDuration = 4.5f;
        public float FallMultiplier = 2f;
        public float JumpCutMultiplier = 0.5f;

        [Header("Air Control")]
        [Tooltip("0 = 完全慣性（空中で制御不能）, 1 = 地上と同じ制御力")]
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
        [Tooltip("水面検知の高さ（足元からのオフセット）")]
        public float WaterSurfaceDetectionHeight = 0.98f;

        [Header("Water Settings - Physics")]
        public float WaterVerticalAcceleration = 10.0f;
        public float WaterSurfaceOffset = 0.5f;

        [Header("Collider Settings")]
        public float DefaultColliderHeight = 1.5f;
        public Vector3 DefaultColliderCenter = new Vector3(0, 0.4f, 0);

        [Header("Water Collider Settings")]
        // 水中Idle（立ち泳ぎ）用
        public float WaterIdleHeight = 1.5f;
        public Vector3 WaterIdleCenter = new Vector3(0, 0.4f, 0);
        // 水中Swimming（水平）用
        public float WaterSwimHeight = 0.6f;
        public Vector3 WaterSwimCenter = new Vector3(0, 0.4f, 0);

        private int _animIDInWater;
        [Header("Settings & Audio")]
        public float JumpTimeout = 0.1f;
        public float FallTimeout = 0.15f;
        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Header("Cinemachine")]
        public GameObject CinemachineCameraTarget;
        public float TopClamp = 70.0f;
        public float BottomClamp = -30.0f;
        public float CameraAngleOverride = 0.0f;
        public bool LockCameraPosition = false;

        private float _cinemachineTargetYaw, _cinemachineTargetPitch, _verticalVelocity, _speed, _animationBlend, _targetRotation, _rotationVelocity, _jumpTimeoutDelta, _fallTimeoutDelta, _jumpButtonHeldTime;
        private float _terminalVelocity = 53.0f;
        private bool _isJumpProcessing, _isJumpInputReady = true, _hasAnimator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private Animator _animator;
        private GameObject _mainCamera;
        private int _animIDSpeed, _animIDGrounded, _animIDJump, _animIDFreeFall, _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
        private InputAction _jumpAction;
#endif

        private bool IsCurrentDeviceMouse => _playerInput.currentControlScheme == "KeyboardMouse";
        private void Awake() { if (_mainCamera == null) _mainCamera = GameObject.FindGameObjectWithTag("MainCamera"); }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
            _jumpAction = _playerInput.actions["Jump"];
#endif
            AssignAnimationIDs();
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);
            GroundedCheck();
            JumpAndGravity();
            Move();
            UpdateCollider();
        }

        private void UpdateCollider()
        {
            if (!_isInWater)
            {
                // 地上ではデフォルトサイズに滑らかに戻す
                _controller.height = Mathf.Lerp(_controller.height, DefaultColliderHeight, Time.deltaTime * 5f);
                _controller.center = Vector3.Lerp(_controller.center, DefaultColliderCenter, Time.deltaTime * 5f);
                return;
            }

            // 水中では移動速度（_animationBlend）に応じてIdle用とSwim用をブレンドする
            // _animationBlend は 0(Idle) から WaterMoveSpeed(Swim) の値をとる
            float swimWeight = Mathf.Clamp01(_animationBlend / WaterMoveSpeed);

            float targetHeight = Mathf.Lerp(WaterIdleHeight, WaterSwimHeight, swimWeight);
            Vector3 targetCenter = Vector3.Lerp(WaterIdleCenter, WaterSwimCenter, swimWeight);

            _controller.height = Mathf.Lerp(_controller.height, targetHeight, Time.deltaTime * 5f);
            _controller.center = Vector3.Lerp(_controller.center, targetCenter, Time.deltaTime * 5f);
        }

        private void LateUpdate() => CameraRotation();

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
                    if (!_jumpAction.IsPressed()) _animator.Play("JumpLand", 0, 0f);
                }
            }
        }

        private void JumpAndGravity()
        {
            bool isJumpPressedNow = _jumpAction.IsPressed();
            if (_isInWater)
            {
                _fallTimeoutDelta = FallTimeout;
                float targetVerticalSpeed = 0;
                //水面の高さ上限の設定
                // 修正前：Vector3.up * 1.25f
                // 修正後：
                bool isNearSurface = !Physics.CheckSphere(transform.position + Vector3.up * WaterSurfaceDetectionHeight, 0.3f, LayerMask.GetMask("Water"), QueryTriggerInteraction.Collide);
                if (!isJumpPressedNow) _isJumpInputReady = true;

                if (isJumpPressedNow)
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
                _input.jump = false;
                return;
            }

            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;
                if (_hasAnimator) { _animator.SetBool(_animIDJump, false); _animator.SetBool(_animIDFreeFall, false); }
                if (!isJumpPressedNow) _isJumpInputReady = true;
                if (isJumpPressedNow && _isJumpInputReady && _jumpTimeoutDelta <= 0.0f)
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
                if (!isJumpPressedNow) _isJumpInputReady = true;
            }

            float currentGravity = Gravity;
            if (_verticalVelocity < 0) currentGravity *= FallMultiplier;
            else if (_verticalVelocity > 0 && _isJumpProcessing)
            {
                if (isJumpPressedNow)
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
            _input.jump = false;
        }

        private Vector3 _horizontalVelocity;
        private void Move()
        {
            float targetSpeed;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;
            if (_isInWater)
            {
                targetSpeed = _input.sprint ? WaterSprintSpeed : WaterMoveSpeed;
                if (_input.move == Vector2.zero) targetSpeed = 0.0f;
                if (_input.move != Vector2.zero)
                {
                    Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
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
                    float motionSpeed = (_input.sprint && _input.move != Vector2.zero) ? 1.5f : 1.0f;
                    _animator.SetFloat(_animIDMotionSpeed, motionSpeed);
                }
            }
            else
            {
                targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
                if (_input.move == Vector2.zero) targetSpeed = 0.0f;
                if (Grounded)
                {
                    if (_input.move != _lastMoveInput && _input.move != Vector2.zero) _turnStopTimer = TurnStopDuration;
                    _lastMoveInput = _input.move;
                    if (_turnStopTimer > 0) { _turnStopTimer -= Time.deltaTime; targetSpeed = 0f; }
                    if (_input.move != Vector2.zero)
                    {
                        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
                        _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
                    }
                    float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                    float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
                    float currentRate = (targetSpeed > currentHorizontalSpeed) ? AccelerationRate : DecelerationRate;
                    _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * currentRate);
                    _horizontalVelocity = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward * _speed;
                }
                else
                {
                    if (_input.move != Vector2.zero)
                    {
                        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
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
            _controller.Move((_horizontalVelocity + new Vector3(0.0f, _verticalVelocity, 0.0f)) * Time.deltaTime);
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
                // 接地状態に関わらず、水から出たらモードを終了する
                ExitWaterMode();
            }
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= 0.01f && !LockCameraPosition)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
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
}