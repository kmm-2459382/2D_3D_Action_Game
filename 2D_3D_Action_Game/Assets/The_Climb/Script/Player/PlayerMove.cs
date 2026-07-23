using UnityEngine;
using UnityEngine.InputSystem;
using TheClimb.Item;
using TheClimb.Player;
using TheClimb.Astral;
using System.Collections;

public class PlayerMove : MonoBehaviour, IConveyorReceiver
{
    Rigidbody RigidBody;
    PlayerState state;
    PlayerJump jump;
    PlayerSpecialAction special;
    PlayerKnockBack knock;
    TempDisableColliders tempDisableCollider;
    VectorToPlanetCalculator vectorToPlanetCaluculator;    //  天体までのベクトルを計算するクラス

    [SerializeField] private InputActionReference leftMoveAction;
    [SerializeField] private InputActionReference rightMoveAction;
    [SerializeField] private ImpactBallController inpactBallController;

    [SerializeField] private bool reverseHorizontal = false;

    [SerializeField] private bool upsideDown = false; // 天井歩行モード
    [SerializeField] private float customGravity = 9.81f; // 通常重力に近い値

    IPlayerDataProvider PlayerDataProvider;    //  プレイヤーのデータプロバイダ
    IPlanetDataProvider PlanetDataProvider;    //  天体のデータプロバイダ

    private float groundMoveForce = 50f;     //プレイヤーの地上移動速度
    public float groundMaxSpeed = 6.5f;   //プレイヤーの地上最高速度記憶
    public float moveInput = 0f;        //プレイヤーの移動方向
    private float airMoveForce = 25f;    //空中での移動速度
    public float airMaxSpeed = 7f;     //空中での速度制限
    public float inversionFloat = 1f;    //反転するときに浮かせる高さ
    private Vector3 horizontalVelocity = Vector3.zero;

    public bool slipping = false;        //着地後勢い止めず滑ってる判定
    public Vector3 slipVelocity;                //滑り時のVelocity

    [SerializeField] public float rideHeight = 0.05f;  //目標の浮遊高度
    [SerializeField] public float rideSpringStrength = 1000f;  //バネの強さ
    [SerializeField] public float rideSpringDamper = 100f;  //バネの減衰力
    [SerializeField] public float hoverRayCastLength = 0.2f;  //地面検知のレイの長さ

    public float MoveInput => moveInput; // ←読み取り専用プロパティ
    public PlayerAnimation PlayerAnimation;
    public PlayerState State => state;
    private bool OnBelt = false;                 //ベルトコンベアに乗っているか
    private Vector3 BeltVelocity = Vector3.zero; //ベルトコンベアの速度(未接触時はゼロ)

    private Vector3 platformVelocityOffset = Vector3.zero; // 動く足場に乗っているときの速度オフセット

    public bool IsUpsideDown => upsideDown;

    //void Awake()
    //{
    //    PlayerContext.Instance.RegistPlayerMove(this);

    //    //PlayerDataProvider = PlayerContext.Instance._PlayerDataProvider;
    //    //PlanetDataProvider = PlanetContext.Instance._PlanetDataProvider;

    //    vectorToPlanetCaluculator = new VectorToPlanetCalculator(PlanetDataProvider, PlayerDataProvider);
    //}

    void Start()
    {
        Application.targetFrameRate = 60; // 60FPSに固定
        QualitySettings.vSyncCount = 1;   // VSync有効

        // MSAA設定（可能な場合）
        QualitySettings.antiAliasing = 4;

        RigidBody = GetComponent<Rigidbody>();
        
        // 移動時のジッター（カクつき）を防ぐために補間を有効か
        RigidBody.interpolation = RigidbodyInterpolation.Interpolate;

        state = GetComponent<PlayerState>();
        jump = gameObject.GetComponent<PlayerJump>();
        special = gameObject.GetComponent<PlayerSpecialAction>();
        knock = gameObject.GetComponent<PlayerKnockBack>();
        tempDisableCollider = gameObject.GetComponent<TempDisableColliders>();

        PlayerAnimation = GameObject.Find("pico_chan_chr_pico_00").GetComponent<PlayerAnimation>();

        if (upsideDown)
        {
            // プレイヤーを上下反転表示（天井に張り付くように）
            Vector3 scale = transform.localScale;
            scale.y *= -1;
            transform.localScale = scale;
        }

        RigidBody.useGravity = false;

    }

    private void ApplyCustomGravity()
    {
        if (upsideDown)
        {
            RigidBody.useGravity = false;

            // 天井歩行モードでも、ジャンプ中は重力を加えない
            if (!state.isGrounded && !jump.jumping)
            {
                float gravityScale = 0.8f;
                RigidBody.AddForce(Vector3.up * customGravity * gravityScale, ForceMode.Acceleration);
            }
        }
        else
        {
            // 通常時はUnityの重力を使用
            RigidBody.useGravity = true;
        }
    }

    public void ToggleUpsideDown()
    {
        upsideDown = !upsideDown; // 状態を反転

        // プレイヤーの「中心位置」と「高さ」を考慮して移動量を計算
        // Pivotが足元にある場合：Center.yはHeight/2 -> 移動量はHeight
        // Pivotが中心にある場合：Center.yは0 -> 移動量は0
        // 公式：MoveAmount = 2 * Center.y * ScaleY

        float moveAmountY = 0f;
        float scaleY = Mathf.Abs(transform.localScale.y);
        bool foundValidCollider = false;

        // 1. CharacterController
        var charController = GetComponent<CharacterController>();
        if (charController != null)
        {
            moveAmountY = 2 * charController.center.y * scaleY;
            foundValidCollider = true;
        }
        else
        {
            // 2. CapsuleCollider
            var capsule = GetComponent<CapsuleCollider>();
            if (capsule != null)
            {
                moveAmountY = 2 * capsule.center.y * scaleY;
                foundValidCollider = true;
            }
            else
            {
                // 3. Collider (Boundsから中心オフセットを計算)
                // 小さすぎるCollider（足元のトリガーなど）は無視する
                var cols = GetComponents<Collider>();
                foreach (var col in cols)
                {
                    if (!col.isTrigger && col.bounds.size.y > 0.5f)
                    {
                        float centerOffset = col.bounds.center.y - transform.position.y;
                        moveAmountY = 2 * centerOffset;
                        foundValidCollider = true;
                        break;
                    }
                }
            }
        }

        // 4. まだ有効なColliderが見つかっていない場合、子オブジェクトを探す
        if (!foundValidCollider)
        {
            var childCols = GetComponentsInChildren<Collider>();
            float maxBoundsY = -999f;
            float minBoundsY = 999f;
            bool foundChild = false;

            foreach (var c in childCols)
            {
                if (c.gameObject != gameObject && !c.isTrigger && c.bounds.size.y > 0.1f)
                {
                    maxBoundsY = Mathf.Max(maxBoundsY, c.bounds.max.y);
                    minBoundsY = Mathf.Min(minBoundsY, c.bounds.min.y);
                    foundChild = true;
                }
            }

            if (foundChild)
            {
                float boundsCenterY = (maxBoundsY + minBoundsY) / 2f;
                float centerOffset = boundsCenterY - transform.position.y;
                moveAmountY = 2 * centerOffset;
                foundValidCollider = true;
            }
        }

        // 5. Renderer (Boundsから) - Colliderがない場合の最終手段
        if (!foundValidCollider)
        {
            var rend = GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                float centerOffset = rend.bounds.center.y - transform.position.y;
                moveAmountY = 2 * centerOffset;
                foundValidCollider = true;
            }
        }

        // 6. デフォルト（足元ピボットと仮定して1.8m移動）
        if (!foundValidCollider)
        {
            moveAmountY = 1.8f;
            Debug.LogWarning("[GravityFlip] Center could not be detected. Using default 1.8f.");
        }

        // 見た目の上下反転（遅延させることで位置移動とのズレを目立たなくする）
        StartCoroutine(DelayedVisualFlip(0.1f));

        //// 慣性は横方向だけ維持（縦をリセット）
        RigidBody.linearVelocity = new Vector3(RigidBody.linearVelocity.x, 0f, 0f);

        RigidBody.AddForce(Vector3.up * 5f, ForceMode.VelocityChange);

        // プレイヤー位置の調整
        // 重力が下の時に切り替えたら上にずらす（＝通常→天井で上にずらす）
        if (upsideDown)
        {
            // 通常→天井：上に移動
            transform.position += Vector3.up * moveAmountY;
        }
        else
        {
            // 天井→通常：下に移動
            transform.position -= Vector3.up * moveAmountY;
        }

        //// 地面の判定をリセット
        state.isGrounded = false;

       

        Debug.Log($"{name} が上下反転！（現在: {(upsideDown ? "天井" : "地面")}）");
    }

    public void ResetGravity()
    {
        if (upsideDown)
        {
            upsideDown = false;
            Rigidbody rb = GetComponent<Rigidbody>();
            // 見た目を元に戻す
            // 回転リセット
            Quaternion targetRot = Quaternion.Euler(0f, 0f, 0f);
            rb.MoveRotation(targetRot);

            // 重力設定を戻す（ApplyCustomGravityで処理されるが、念のため）
            RigidBody.useGravity = true;

            Debug.Log("リスポーンに伴い重力をリセットしました");
        }
    }

    private IEnumerator DelayedVisualFlip(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        Vector3 offset = Vector3.up * (upsideDown ? inversionFloat : -inversionFloat);

        // 現在位置を基準に、少し上or下）へ移動
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 targetPosition = rb.position + offset;

        // Rigidbody経由で位置移動
        rb.MovePosition(targetPosition);

        Quaternion targetRot = upsideDown
         ? Quaternion.Euler(180f, 0f, 0f)
         : Quaternion.Euler(0f, 0f, 0f);

        rb.MoveRotation(targetRot);
    }

    private void Update()
    {
        //移動キー操作
        if (!special.meteorDrop && !knock.knockBacking)　//ノックバック、メテオドロップ中は不可
        {
            MoveOperation();
        }
        CheckStuck();
    }

    void FixedUpdate()
    {
        if (inpactBallController != null)
        {
            //移動
            if (special.highJumpChargeCounter == 0f && inpactBallController.currentState is not InpactBallExplosionState)
            {
                if ((state.isGrounded || (!state.isGrounded && state.isJumpMoveOK && !state.isLeftWall && !state.isRightWall)) && !knock.knockBacking)
                {
                    jump.coyoteCounter = 0f;

                    //プレイヤー地上の移動
                    GroundPlayerMove();
                }
                else
                {
                    jump.coyoteCounter += Time.fixedDeltaTime;

                    //プレイヤー空中の移動
                    AirPlayerMove();
                }
            }
        }
        //移動
        if (special.highJumpChargeCounter == 0f)
        {
            if ((state.isGrounded || (!state.isGrounded && state.isJumpMoveOK && !state.isLeftWall && !state.isRightWall)) && !knock.knockBacking)
            {
                jump.coyoteCounter = 0f;

                //プレイヤー地上の移動
                GroundPlayerMove();
            }
            else
            {
                jump.coyoteCounter += Time.fixedDeltaTime;

                //プレイヤー空中の移動
                AirPlayerMove();
            }
        }

        ApplyHoveringForce();
        ApplyCustomGravity();
    }

    private void ApplyHoveringForce()
    {
        //すり抜け発動中はホバーを切る
        if (tempDisableCollider != null && tempDisableCollider.isRunning)
        {
            platformVelocityOffset = Vector3.zero;
            return;
        }

        //地面へのRaycast
        Vector3 rayOrigin = transform.position + (upsideDown ? Vector3.down : Vector3.up) * 0.1f;
        Vector3 rayDirection = upsideDown ? Vector3.up : Vector3.down;

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, hoverRayCastLength, state.groundLayerMask))
        {
            // Rayの方向の速度を取得
            Vector3 vel = RigidBody.linearVelocity;
            Vector3 rayDir = rayDirection;

            float rayDirVelocity = Vector3.Dot(rayDir, vel);

            //理想の高さとの差分（変位）を計算
            float targetDistance = rideHeight + 0.1f;
            float x = hit.distance - targetDistance;

            //バネの計算
            float springForce = (x * rideSpringStrength) - (rayDirVelocity * rideSpringDamper);

            //力を加える
            RigidBody.AddForce(rayDirection * springForce);

            //動く足場用の処理
            if (hit.rigidbody != null)
            {
                //足場の速度を取得
                Vector3 platformVelocity = hit.rigidbody.linearVelocity;
                platformVelocity.y = 0f;
                
                //動く足場に乗っている時はその速度を記録しておく
                platformVelocityOffset = platformVelocity;

                if (platformVelocity.magnitude > 0.1f && moveInput == 0f)
                {
                    //移動入力が無いときは、足場に完全に追従させる強い摩擦力をかける
                    Vector3 currentHorizontalVel = new Vector3(RigidBody.linearVelocity.x, 0f, RigidBody.linearVelocity.z);
                    Vector3 velocityDifference = platformVelocity - currentHorizontalVel;
                    RigidBody.AddForce(velocityDifference * 60f, ForceMode.Acceleration);
                }
            }
            else
            {
                platformVelocityOffset = Vector3.zero;
            }
        }
        else
        {
            platformVelocityOffset = Vector3.zero;
        }
    }

    private void MoveOperation()
    {
        if (state.inputManager.leftHeld && state.inputManager.rightHeld ||
            !state.inputManager.leftHeld && !state.inputManager.rightHeld)  //止まる
        {
            moveInput = 0f;
            horizontalVelocity = new Vector3(0f, 0f, 0f);
        }
        else if (state.inputManager.leftHeld && !state.isLeftWall)  //左移動
        {
            moveInput = -1f;
            state.playerDirectionRight = false;
        }
        else if (state.inputManager.rightHeld && !state.isRightWall)  //右移動
        {
            moveInput = 1f;
            state.playerDirectionRight = true;
        }
        else
        {
            moveInput = 0f;
        }

        if (reverseHorizontal)
        {
            moveInput *= -1f;
        }
    }


    private void GroundPlayerMove()
    {
        //ベルトコンベア上の場合はベルトコンベアの力を加える
        if (OnBelt)
        {
            RigidBody.AddForce(BeltVelocity, ForceMode.Acceleration);
        }

        if (moveInput == 0f && RigidBody.linearVelocity.x != 0f && state.landing)  //着地しているときに移動入力がない場合はぴたっとで止まる
        {
            RigidBody.linearVelocity = new Vector3(0f, RigidBody.linearVelocity.y, 0f);
        }
        else if (moveInput != 0f)
        {
            //地上：慣性なし、即応する左右移動
            Vector3 force = new Vector3(moveInput, 0f, 0f) * groundMoveForce;
            RigidBody.AddForce(force, ForceMode.Acceleration);
            
            //動く足場の速度を考慮した最高速度の計算
            horizontalVelocity = new Vector3(RigidBody.linearVelocity.x, 0f, RigidBody.linearVelocity.z);
            
            //プレイヤー自身が本来出したい最高速度ベクトル
            Vector3 targetMaxVel = new Vector3(Mathf.Sign(moveInput) * groundMaxSpeed, 0f, 0f);
            
            //最終的に許される最高速度 = プレイヤーの最高速度 + 足場の速度
            Vector3 allowedMaxVel = targetMaxVel + platformVelocityOffset;

            //X軸方向のクランプ
            float clampedX = RigidBody.linearVelocity.x;
            if (moveInput > 0 && clampedX > allowedMaxVel.x)
            {
                clampedX = allowedMaxVel.x;
            }
            else if (moveInput < 0 && clampedX < allowedMaxVel.x)
            {
                clampedX = allowedMaxVel.x;
            }

            RigidBody.linearVelocity = new Vector3(clampedX, RigidBody.linearVelocity.y, RigidBody.linearVelocity.z);
        }
        else if (!special.meteorHighJump && !jump.jumpCoolActive && Mathf.Abs(RigidBody.linearVelocity.x - platformVelocityOffset.x) > 0.1f)  //ぴたっと止まる
        {
            //慣性で止まるよ (動く足場の上では足場の速度を目標にして減速する)
            Debug.Log("慣性で止まるよ");
            Vector3 vel = RigidBody.linearVelocity;
            vel.x = Mathf.MoveTowards(vel.x, platformVelocityOffset.x, groundMoveForce * Time.fixedDeltaTime);
            RigidBody.linearVelocity = vel;
        }
    }

    private void AirPlayerMove()
    {
        //空中：左右に力を加える
        bool canAccelerate = false;
        if (moveInput > 0)
        {
            if (RigidBody.linearVelocity.x < airMaxSpeed) canAccelerate = true;
        }
        else if (moveInput < 0)
        {
            if (RigidBody.linearVelocity.x > -airMaxSpeed) canAccelerate = true;
        }

        if (canAccelerate)
        {
            Vector3 force = new Vector3(moveInput, 0f, 0f) * airMoveForce; //プレイヤーの入力による移動力を計算
            RigidBody.AddForce(force, ForceMode.Acceleration); //力を加える
        }

        horizontalVelocity = new Vector3(RigidBody.linearVelocity.x, 0f, 0f); //現在の水平速度を更新

        //ハイジャンプ後徐々に早くするよ
        if (special.highJumpUsed)
        {
            if (airMaxSpeed > 10f)
            {
                airMaxSpeed = 10f;
            }
            else if (airMaxSpeed < 0.1f)
            {
                airMaxSpeed += airMaxSpeed;
            }
            else if (airMaxSpeed < 10f)
            {
                airMaxSpeed += 0.3f;
            }
        }
        else if (special.quickJumpUsed) //クイックジャンプ後徐々に遅くするよ
        {
            if (airMaxSpeed > 10f)
            {
                //Debug.Log(maxAirSpeed);
                airMaxSpeed -= 0.14f;
            }
        }
    }

    public void SetOnBelt(bool OnBelt, Vector3 velocity)
    {
        this.OnBelt = OnBelt;
        this.BeltVelocity = velocity;
    }

    [Header("埋まり判定設定")]
    public float stuckCheckDelay = 0.5f; // 埋まり判定が確定するまでの時間
    public CapsuleCollider stuckDetectionCollider; // 埋まり判定専用のコライダー（未設定なら自動検出）
    public Collider[] collidersToDisableOnUnstuck; // 脱出時に一時的に無効化するコライダー（足元のSphereColliderなど）
    private float stuckTimer = 0f;

    private void CheckStuck()
{
    // カプセルコライダーを取得
    Vector3 point1, point2;
    float radius;

    CapsuleCollider capsule = stuckDetectionCollider != null ? stuckDetectionCollider : GetComponent<CapsuleCollider>();
    if (capsule != null)
    {
        float shrink = 0.1f;
        float maxScale = Mathf.Max(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
        radius = capsule.radius * maxScale - shrink;
        float height = capsule.height * maxScale - (shrink * 2);

        Vector3 center = capsule.transform.TransformPoint(capsule.center);
        point1 = center + Vector3.up * (height / 2f - radius);
        point2 = center - Vector3.up * (height / 2f - radius);
    }
    else
    {
        return;
    }

    // コライダー取得
    Collider[] hitColliders = Physics.OverlapCapsule(point1, point2, radius);

    bool isStuck = false;
    foreach (var col in hitColliders)
    {
        if (col.gameObject != gameObject && !col.isTrigger)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Ground") && !col.CompareTag("Nosink"))
            {
                isStuck = true;
                break;
            }
        }
    }

    if (isStuck)
    {
        stuckTimer += Time.deltaTime;
        if (stuckTimer > stuckCheckDelay)
        {
            Debug.LogWarning("プレイヤーの埋まりを検知しました。位置を修正します。");

            float direction = upsideDown ? 1f : -1f;
            transform.position += Vector3.up * direction * 3.0f;

            if (collidersToDisableOnUnstuck != null && collidersToDisableOnUnstuck.Length > 0)
            {
                StartCoroutine(TemporarilyDisableColliders());
            }

            stuckTimer = 0f;
        }
    }
    else
    {
        stuckTimer = 0f;
    }
}

    private IEnumerator TemporarilyDisableColliders()
    {
        // 指定されたコライダーを無効化
        foreach (var col in collidersToDisableOnUnstuck)
        {
            if (col != null)
            {
                col.enabled = false;
            }
        }

        // 0.1秒待機
        yield return new WaitForSeconds(0.1f);

        // 再有効化
        foreach (var col in collidersToDisableOnUnstuck)
        {
            if (col != null)
            {
                col.enabled = true;
            }
        }
    }
}
