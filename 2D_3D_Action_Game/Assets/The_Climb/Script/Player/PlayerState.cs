using UnityEngine;
using TheClimb.Item;
using TMPro;

public class PlayerState : MonoBehaviour, IImpactable
{
    public bool highJumpOn = false;      //ハイジャンプ可能か
    public bool quickJumpOn = false;     //クイックジャンプ可能か
    public bool meteorDropOn = false;    //メテオドロップ叶か

    public bool isFlipped = false;  // ← 反転中なら true
    public bool canFlip => isGrounded && !flipCooldownActive;  // ← 条件付きプロパティ

    [HideInInspector] public Rigidbody RigidBody;
    [HideInInspector] public InputManager inputManager;
    [HideInInspector] public PlayerMove move;
    [HideInInspector] public PlayerJump jump;
    [HideInInspector] public PlayerSpecialAction special;
    [HideInInspector] public PlayerAnimation PlayerAnimation;
    [HideInInspector] public GameObject hukidashi;
    [HideInInspector] public TextMeshPro hukidashiText;

    int lowGroundLayer;

    public bool playerDirectionRight = true;  //プレイヤーの見ている方向が右ならtrue、左ならfalse
    private bool wasGrounded = false;    //前フレームの地面状態
    public bool landing = false;         //着地判定
    private float landingJumpTime = 0.1f;  //着地ジャンプの猶予タイム
    private float landingJumpCounter = 0f;  //着地ジャンプの猶予カウンター
    public bool landingJumpOn = false;   //着地ジャンプのカウントを始める用

    [HideInInspector] public Transform groundCheck;        //プレイヤー足元の地面判定用オブジェクト
    public bool isGrounded;              //地面判定
    [HideInInspector] public Transform jumpMoveOKCheck;    //プレイヤー足元のジャンプ判定用オブジェクト
    public bool isJumpMoveOK;            //ジャンプOK判定
    [HideInInspector] public Transform leftWallCheck;      //プレイヤー足元の左壁判定用オブジェクト
    public bool isLeftWall;              //左壁判定
    [HideInInspector] public Transform rightWallCheck;     //プレイヤー足元の右壁判定用オブジェクト
    public bool isRightWall;             //右壁判定
    [HideInInspector] public LayerMask groundLayer;        //地面レイヤー
    [HideInInspector] public int whiteGround;
    [HideInInspector] public int blackGround;
    public int groundLayerMask;
    private float groundCheckRadius = 0.1f;  //地面判定の半径
    public bool isAir = false;           //空中判定

    private float playerFallSpeed = -30f;  //プレイヤーの落下速度
    private float playerMaxUpSpeed = 30f;  //プレイヤーの最大上昇速度
    private float playerMaxMoveSpeed = 30f;    //プレイヤーの最大移動速度

    public float erosionLevel = 0;       //プレイヤーの侵蝕度
    public int sanityLevel = 100;        //プレイヤーの正気度
    public bool carryingBuddy = false;    //Buddyをおんぶしてる状態か判定
    public bool nearBell = false;       //WhiteBellの近くか判定

    private bool flipCooldownActive = false;  // クールタイム中かどうか
    private float flipCooldownTime = 10f;     // クールタイム（秒）

    public Rigidbody RigidbodyGetter => this.RigidBody ;
    public Transform TransformGetter => this.transform;

    private void Awake()
    {
        // PlayerContext.Instance.RegistPlayerState(this);
        RigidBody = GetComponent<Rigidbody>();
        ImpactableRegistry.Register(this);

        //吹き出しオブジェクトの取得
        hukidashi = transform.Find("Hukidashi").gameObject;
        hukidashiText = hukidashi.transform.Find("HukidashiText").GetComponent<TextMeshPro>();
        hukidashi.SetActive(false);
    }

    void OnEnable()
    {
        ImpactableRegistry.Register(this);
    }

    void OnDisable()
    {
        ImpactableRegistry.Unregister(this);
    }
    void Start()
    {
        inputManager = GameObject.Find("KeyManager").GetComponent<InputManager>();
        RigidBody = GetComponent<Rigidbody>();
        move = GetComponent<PlayerMove>();
        jump = GetComponent<PlayerJump>();
        special = GetComponent<PlayerSpecialAction>();

        PlayerAnimation = transform.Find("pico_chan_chr_pico_00").GetComponent<PlayerAnimation>();

        // Base ground layer
        groundLayer = GameLayer.ToMask(GameLayers.GROUND);
        
        // 追加レイヤーの取得と結合
        int whiteIndex = LayerMask.NameToLayer("WhiteGround");
        int blackIndex = LayerMask.NameToLayer("BlackGround");
        int lowIndex = LayerMask.NameToLayer("LowGround");
        
        if (whiteIndex != -1) groundLayer.value |= (1 << whiteIndex);
        if (blackIndex != -1) groundLayer.value |= (1 << blackIndex);
        if (lowIndex != -1) groundLayer.value |= (1 << lowIndex);

        lowGroundLayer = groundLayer;
        groundLayerMask = groundLayer;
        
        whiteGround = (whiteIndex != -1) ? (1 << whiteIndex) : 0;
        blackGround = (blackIndex != -1) ? (1 << blackIndex) : 0;

        // インスペクターまたはスクリプトで設定
        //RigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        RigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        RigidBody.interpolation = RigidbodyInterpolation.Interpolate;

        Physics.gravity = new Vector3(0, -45F, 0); // Gを倍にする
    }


    public void StartFlipCooldown()
    {
        flipCooldownActive = true;
        Invoke(nameof(ResetFlipCooldown), flipCooldownTime);
    }

    private void ResetFlipCooldown()
    {
        flipCooldownActive = false;
    }

    private void Update()
    {
        // 左壁判定（カプセル形）
        isLeftWall = Physics.CheckCapsule(leftWallCheck.position + Vector3.up * 0.60f, leftWallCheck.position + Vector3.down * 0.60f, 0.001f, groundLayerMask);
        // 右壁判定（カプセル形）
        isRightWall = Physics.CheckCapsule(rightWallCheck.position + Vector3.up * 0.60f, rightWallCheck.position + Vector3.down * 0.60f, 0.001f, groundLayerMask);

        if (jump.jumpCoolActive || jump.jumping)
        {
            isGrounded = false;
        }
        else
        {
            //Rayの始点と方向を設定
            Vector3 rayOrigin = groundCheck.position + (move.IsUpsideDown ? Vector3.down : Vector3.up) * 0.1f;
            Vector3 rayDirection = move.IsUpsideDown ? Vector3.up : Vector3.down;

            //地面との距離は、プレイヤーの乗り高さに少し余裕を持たせる
            float groundedDistance = move.rideHeight + 0.2f; 
            
            //まずは足元の狭い範囲のRay（Edgeに立った時用）
            //isGrounded = Physics.Raycast(rayOrigin, rayDirection, groundedDistance, groundLayerMask);
            
            //安全のため、ある程度厚みを持たせたSphereCastで判定
            isGrounded = Physics.SphereCast(rayOrigin, groundCheckRadius, rayDirection, out RaycastHit hit, groundedDistance, groundLayerMask);

            //念のため以前のカプセル判定も残す
            if (!isGrounded)
            {
                isGrounded = Physics.CheckCapsule(groundCheck.position + Vector3.right * 0.08f, groundCheck.position + Vector3.left * 0.08f, groundCheckRadius, groundLayerMask);
            }

            //空中時、isJumpOKを反応させない
            if (isAir)
            {
                isJumpMoveOK = false;
            }
            else
            {
                // ジャンプOK判定（カプセル形）
                isJumpMoveOK = Physics.CheckSphere(jumpMoveOKCheck.position, 0.19f, groundLayerMask);
            }

            //着地チェック
            if (!jump.jumpCoolActive)
            {
                LandingCheck();
            }

            //空中判定
            if (!isGrounded && !isJumpMoveOK)
            {
                isAir = true;
            }
            else
            {
                isAir = false;
            }

            //落下速度調整
            if (RigidBody.linearVelocity.y < playerFallSpeed && !isGrounded && !jump.jumping && !landing)
            {
                RigidBody.linearVelocity = new Vector3(RigidBody.linearVelocity.x, playerFallSpeed, 0);
            }
            //上昇速度制限
            if (RigidBody.linearVelocity.y > playerMaxUpSpeed)
            {
                RigidBody.linearVelocity = new Vector3(RigidBody.linearVelocity.x, playerMaxUpSpeed, 0);
            }
            //横移動速度制限
            if (Mathf.Abs(RigidBody.linearVelocity.x) > playerMaxMoveSpeed)
            {
                RigidBody.linearVelocity = new Vector3(Mathf.Sign(RigidBody.linearVelocity.x) * playerMaxMoveSpeed, RigidBody.linearVelocity.y, 0);
            }

            //正気度０で死
            if (sanityLevel <= 0)
            {
                PlayerDead();
            }

            //壁に当たるのならば強制停止
            //    if ((isLeftWall && RigidBody.linearVelocity.x < 0) ||
            //(isRightWall && RigidBody.linearVelocity.x > 0))
            //    {
            //        RigidBody.linearVelocity = new Vector3(0, RigidBody.linearVelocity.y, 0);
            //    }

            //前フレームの接地判定
            wasGrounded = isGrounded;
        }
    }

    private void LandingCheck()
    {
        landing = false;
        //着地判定
        if (!wasGrounded && isGrounded)
        {
            landing = true;

            landingJumpCounter = 0f;
            landingJumpOn = true;
            isLeftWall = false;
            isRightWall = false;
            //RigidBody.linearVelocity = new Vector3(RigidBody.linearVelocity.x, 0, 0);

            // 横方向の速度が一定以上ならスリップ開始
            if (Mathf.Abs(RigidBody.linearVelocity.x) > move.groundMaxSpeed && !special.meteorDrop)
            {
                move.slipping = true;
                move.slipVelocity = RigidBody.linearVelocity;
            }
        }

        //着地ジャンプ猶予カウント
        if (landingJumpOn)
        {
            landingJumpCounter += Time.deltaTime;

            if (landingJumpCounter > landingJumpTime)
            {
                special.meteorHighJumpOK = false;
                LandingJumpReset();
            }
        }
    }

    public void LandingJumpReset()
    {
        landingJumpOn = false;
        special.quickJumpUsed = false;
        special.meteorDropUsed = false;
        special.meteorDropCounter = 0;
    }

    private void PlayerDead()
    {
        Debug.Log("栗松、帰国の準備をしろ。～GameOver～");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SearchItem"))
        {
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("WhiteBell"))
        {
            nearBell = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("WhiteBell"))
        {
            nearBell = false;
        }
    }
}