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
        [Range(0f, 1f)] public float TopFloatMultiplier = 1f; // スライダー化
        [Range(0f, 10f)] public float TopFloatDuration = 4.5f;
        public float FallMultiplier = 2f;
        public float JumpCutMultiplier = 0.5f;

        [Header("Air Control")]
        [Tooltip("0 = 完全慣性（空中で制御不能）, 1 = 地上と同じ制御力")]
        [Range(0f, 1f)]
        public float AirControl = 0.2f; // デフォルトを少し慣性強めに設定

        [Header("Player Grounded")]
        public bool Grounded = true;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.28f;
        public LayerMask GroundLayers;

        [Header("Water Settings")]
        public float WaterMoveSpeed = 4.0f;
        public float WaterSprintSpeed = 7.0f;
        public float WaterVerticalSpeed = 4.0f; // 上昇・下降速度
        private bool _isInWater = false;

        [Header("Water Settings - Physics")]
        public float WaterVerticalAcceleration = 10.0f; // 上下移動の加速・減速の速さ
        public float WaterSurfaceOffset = 0.5f;        // 水面と判定する深さのオフセット

        // アニメーションID
        private int _animIDInWater; // 水中フラグ
                                    // 既存の _animIDSpeed や _animIDMotionSpeed を流用します

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
        }

        private void LateUpdate() => CameraRotation();

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            bool wasGrounded = Grounded;
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
                if (Grounded && !wasGrounded)
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

                // 水面判定：頭付近が水面に出ているか
                bool isNearSurface = !Physics.CheckSphere(transform.position + Vector3.up * 1.5f, 0.2f, LayerMask.GetMask("Water"), QueryTriggerInteraction.Collide);

                if (isJumpPressedNow)
                {
                    if (isNearSurface)
                    {
                        // 【重要】水面に到達した時にSpaceが押されていたら、一旦「押し直し待ち」状態にする
                        // _isJumpInputReady が false の間は、水面ジャンプを許可しない
                        if (_isJumpInputReady)
                        {
                            // 水面ジャンプ実行
                            _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                            _isInWater = false;
                            _isJumpInputReady = false; // 連続ジャンプ防止

                            if (_hasAnimator)
                            {
                                _animator.SetBool(_animIDInWater, false);
                                _animator.SetBool(_animIDJump, true);
                                _animator.Play("JumpStart", 0, 0f);
                            }
                            return;
                        }
                        else
                        {
                            // 長押しで水面に来た場合は、ここで待機（速度を殺す）
                            targetVerticalSpeed = 0f;
                            // 急停止させるために Lerp ではなく直接 0 に近づける
                            _verticalVelocity = Mathf.MoveTowards(_verticalVelocity, 0f, Time.deltaTime * WaterVerticalAcceleration * 2f);
                        }
                    }
                    else
                    {
                        // まだ深い場所なら上昇
                        targetVerticalSpeed = WaterVerticalSpeed;

                        // 深い場所で上昇中に水面ジャンプ準備を「不可」にする
                        // これにより、水中からSpace長押しで水面に来ても、即ジャンプしなくなります
                        _isJumpInputReady = false;
                    }
                }
                else
                {
                    // Spaceを離した瞬間に、ようやく「水面ジャンプができる準備」が整う
                    _isJumpInputReady = true;

#if ENABLE_INPUT_SYSTEM
                    if (Keyboard.current != null && Keyboard.current.ctrlKey.isPressed)
                    {
                        targetVerticalSpeed = -WaterVerticalSpeed;
                    }
#endif
                }

                // 垂直慣性の適用
                _verticalVelocity = Mathf.Lerp(_verticalVelocity, targetVerticalSpeed, Time.deltaTime * WaterVerticalAcceleration);
                _isJumpProcessing = false;
                return;
            }

            // --- 以下、地上/空中ロジック ---
            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;
                if (_hasAnimator) { _animator.SetBool(_animIDJump, false); _animator.SetBool(_animIDFreeFall, false); }

                // 地上でも「離している間だけ準備完了」とする
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

            // --- 重力計算（FallMultiplier等） ---
            float currentGravity = Gravity;
            if (_verticalVelocity < 0) currentGravity *= FallMultiplier;
            else if (_verticalVelocity > 0)
            {
                if (_isJumpProcessing)
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
            }

            if (_verticalVelocity > -_terminalVelocity) _verticalVelocity += currentGravity * Time.deltaTime;
            if (_verticalVelocity < -_terminalVelocity) _verticalVelocity = -_terminalVelocity;

            _input.jump = false;
        }

        private Vector3 _horizontalVelocity;

        private void Move()
        {
            float targetSpeed;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // --- 1. 水中モードの処理 ---
            if (_isInWater)
            {
                // 水中での速度決定（ShiftでWaterSprintSpeedに加速）
                targetSpeed = _input.sprint ? WaterSprintSpeed : WaterMoveSpeed;
                if (_input.move == Vector2.zero) targetSpeed = 0.0f;

                // 水中での向き更新
                if (_input.move != Vector2.zero)
                {
                    Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
                    _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
                }
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

                // 水中での移動ベクトル計算（クイックに反応するようにLerpを使用）
                Vector3 targetInputDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
                _horizontalVelocity = Vector3.Lerp(_horizontalVelocity, targetInputDirection * targetSpeed * inputMagnitude, Time.deltaTime * AccelerationRate);

                // 水中アニメーションのブレンド
                _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * AccelerationRate);
                if (_animationBlend < 0.01f) _animationBlend = 0f;

                if (_hasAnimator)
                {
                    _animator.SetFloat(_animIDSpeed, _animationBlend);
                    // ダッシュ中（Shift押し）はアニメーションを1.5倍速、通常は1.0倍速にする
                    float motionSpeed = (_input.sprint && _input.move != Vector2.zero) ? 1.5f : 1.0f;
                    _animator.SetFloat(_animIDMotionSpeed, motionSpeed);
                }
            }
            // --- 2. 地上・空中モードの処理 ---
            else
            {
                targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
                if (_input.move == Vector2.zero) targetSpeed = 0.0f;

                if (Grounded)
                {
                    // 地上での急旋回ストップ処理
                    if (_input.move != _lastMoveInput && _input.move != Vector2.zero) _turnStopTimer = TurnStopDuration;
                    _lastMoveInput = _input.move;
                    if (_turnStopTimer > 0) { _turnStopTimer -= Time.deltaTime; targetSpeed = 0f; }

                    if (_input.move != Vector2.zero)
                    {
                        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
                        _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
                    }

                    // 地上回転適用
                    float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

                    // 地上速度計算
                    float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
                    float currentRate = (targetSpeed > currentHorizontalSpeed) ? AccelerationRate : DecelerationRate;
                    _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * currentRate);
                    _horizontalVelocity = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward * _speed;
                }
                else
                {
                    // 空中での向き更新
                    if (_input.move != Vector2.zero)
                    {
                        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
                        _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
                    }
                    float rotSpeed = RotationSmoothTime / Mathf.Max(0.1f, AirControl);
                    float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, rotSpeed);
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

                    // 空中移動（AirControlの影響を受ける）
                    Vector3 targetInputDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
                    _horizontalVelocity = Vector3.Lerp(_horizontalVelocity, targetInputDirection * targetSpeed * inputMagnitude, Time.deltaTime * AccelerationRate * AirControl);
                }

                // 地上・空中アニメーション
                _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * AccelerationRate);
                if (_animationBlend < 0.01f) _animationBlend = 0f;

                if (_hasAnimator)
                {
                    _animator.SetFloat(_animIDSpeed, _animationBlend);
                    _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
                }
            }

            // --- 3. 移動の最終実行（共通） ---
            _controller.Move((_horizontalVelocity + new Vector3(0.0f, _verticalVelocity, 0.0f)) * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider foreign)
        {
            if (foreign.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                _isInWater = true;
                if (_hasAnimator) _animator.SetBool(_animIDInWater, true);
            }
        }

        private void OnTriggerExit(Collider foreign)
        {
            if (foreign.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                _isInWater = false;
                if (_hasAnimator) _animator.SetBool(_animIDInWater, false);
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

        // --- Audio Functions (Standard Names) ---
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