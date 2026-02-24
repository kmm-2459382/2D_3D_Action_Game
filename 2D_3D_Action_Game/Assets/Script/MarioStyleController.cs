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
        public float JumpHeight = 3.0f;
        public float Gravity = -30.0f;
        public float MaxJumpHeldTime = 0.4f;
        [Range(0f, 1f)] public float TopFloatMultiplier = 0.1f; // スライダー化
        [Range(0f, 10f)] public float TopFloatDuration = 4.5f;
        public float FallMultiplier = 2.5f;
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

            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;
                if (_hasAnimator) { _animator.SetBool(_animIDJump, false); _animator.SetBool(_animIDFreeFall, false); }
                if (!isJumpPressedNow) _isJumpInputReady = true;

                if (isJumpPressedNow && _isJumpInputReady && _jumpTimeoutDelta <= 0.0f)
                {
                    // ジャンプ開始：初速を与える
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                    if (_hasAnimator) { _animator.SetBool(_animIDJump, true); _animator.Play("JumpStart", 0, 0f); }

                    _isJumpProcessing = true;
                    _isJumpInputReady = false;
                    _jumpButtonHeldTime = 0.0f;
                    _jumpTimeoutDelta = JumpTimeout;
                }

                if (_jumpTimeoutDelta >= 0.0f) _jumpTimeoutDelta -= Time.deltaTime;
                // 地面にいる時はわずかに押し下げる力を維持
                if (_verticalVelocity < 0.0f) _verticalVelocity = -2f;
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;
                if (_fallTimeoutDelta >= 0.0f) _fallTimeoutDelta -= Time.deltaTime;
                else if (_hasAnimator) _animator.SetBool(_animIDFreeFall, true);
                if (!isJumpPressedNow) _isJumpInputReady = true;
            }

            // --- 重力計算ロジック ---
            float currentGravity = Gravity;

            if (_verticalVelocity < 0)
            {
                // 1. 落下中：FallMultiplierをかけて素早く落とす
                currentGravity *= FallMultiplier;
            }
            else if (_verticalVelocity > 0)
            {
                // 2. 上昇中
                if (_isJumpProcessing)
                {
                    if (isJumpPressedNow)
                    {
                        // 長押し中：上昇重力を軽減して高く飛ぶ
                        _jumpButtonHeldTime += Time.deltaTime;
                        currentGravity *= 0.6f;
                        if (_jumpButtonHeldTime > MaxJumpHeldTime) _isJumpProcessing = false;
                    }
                    else
                    {
                        // --- 最小ジャンプの「すとん」感を作る ---
                        // ボタンを離した瞬間、JumpCutMultiplier (0.1など) で速度を削る
                        _verticalVelocity *= JumpCutMultiplier;
                        // 上昇処理を強制終了して、即座に落下重力がかかる状態にする
                        _isJumpProcessing = false;
                    }
                }

                // --- 頂点付近の「ふわっと」処理 ---
                // 最大まで溜めて、かつボタンを押し続けている時のみ発動
                if (!_isJumpProcessing && isJumpPressedNow && _verticalVelocity < TopFloatDuration)
                {
                    currentGravity *= TopFloatMultiplier;
                }
            }

            // 【重要】ここで重力を速度に加算する（これが消えていたため、ずっと上昇していました）
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += currentGravity * Time.deltaTime;
            }

            _input.jump = false;
        }

        private Vector3 _horizontalVelocity;

        private void Move()
        {
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            if (Grounded)
            {
                // 地上での回転処理
                if (_input.move != _lastMoveInput && _input.move != Vector2.zero) _turnStopTimer = TurnStopDuration;
                _lastMoveInput = _input.move;
                if (_turnStopTimer > 0) { _turnStopTimer -= Time.deltaTime; targetSpeed = 0f; }

                if (_input.move != Vector2.zero)
                {
                    Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
                    _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
                }

                // 地上での回転適用
                float currentRotation = transform.eulerAngles.y;
                float angleDiff = Mathf.Abs(Mathf.DeltaAngle(currentRotation, _targetRotation));
                if (angleDiff > 0.1f)
                {
                    float rotation = Mathf.SmoothDampAngle(currentRotation, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                }

                // 地上での速度計算（Lerpで加速・減速）
                float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
                float currentRate = (targetSpeed > currentHorizontalSpeed) ? AccelerationRate : DecelerationRate;
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * currentRate);

                // 地上では現在の向きに向かって進む
                _horizontalVelocity = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward * _speed;
            }
            else
            {
                // --- 空中での処理（ここが重要） ---

                // 1. 空中での向き更新（AirControlが0でも向きだけは変えられるようにする場合）
                if (_input.move != Vector2.zero)
                {
                    Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
                    _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
                }
                float currentRotation = transform.eulerAngles.y;
                float rotSpeed = RotationSmoothTime / Mathf.Max(0.1f, AirControl); // 回転もAirControlの影響を受ける
                float rotation = Mathf.SmoothDampAngle(currentRotation, _targetRotation, ref _rotationVelocity, rotSpeed);
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

                // 2. 空中での移動ベクトルの計算
                // 入力方向への目標ベクトル
                Vector3 targetInputDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

                // 現在の慣性ベクトルに対して、入力方向への力を AirControl 越しに加える
                // AirControl が 0 なら、この Lerp は現在の速度を維持し続ける
                _horizontalVelocity = Vector3.Lerp(_horizontalVelocity, targetInputDirection * targetSpeed * inputMagnitude, Time.deltaTime * AccelerationRate * AirControl);
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * AccelerationRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // 最終的な移動実行（水平ベクトルの _horizontalVelocity + 垂直ベクトルの _verticalVelocity）
            _controller.Move(_horizontalVelocity * Time.deltaTime + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            if (_hasAnimator) { _animator.SetFloat(_animIDSpeed, _animationBlend); _animator.SetFloat(_animIDMotionSpeed, inputMagnitude); }
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