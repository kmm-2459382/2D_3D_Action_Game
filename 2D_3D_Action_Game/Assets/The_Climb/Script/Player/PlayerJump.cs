using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    Rigidbody RigidBody;
    PlayerState state;
    PlayerMove move;
    PlayerSpecialAction special;
    PlayerKnockBack knock;

    [Header("ジャンプ設定")]
    public bool jumping = false; 　//ジャンプ入力中判定
    private float coyoteTime = 0.13f;　//コヨーテタイム
    public float coyoteCounter = 0f;　//コヨーテタイムカウント
    private float jumpCoolTime = 0.2f;　//ジャンプのクールタイム
    private float jumpCoolCounter = 0f;　//ジャンプのクールタイムカウント
    public bool jumpCoolActive = false;　//ジャンクールタイムを始める用判定
    private bool isJumpQueued = false;　//ジャンプキーが押された判定
    private float jumpQueueTime = 0.2f;　//ジャンプ選考入力猶予時間
    private float jumpQueueCounter = 0f;　//ジャンプ選考入力カウンター
    private float jumpTime;　　　　　　　//ジャンプ入力時間　
    private float jumpTimeMax;　         //最大ジャンプ入力時間
    private float jumpTimeMaxSaving = 0.22f;　//最大図アンプ入力時間を保持
    private float groundJumpPower = 13f;　//ジャンプでプレイヤーにかかる上方向の力
    private float maxJumpSpeed = 12f;　//空中での速度制限
    [SerializeField] AnimationCurve jumpCurve = new();　//ジャンプの速度カーブ

    [Header("トランポリン")]
    public bool isOnTrampoline = false;　//トランポリンに乗ってるかの判定
    public float TrampolinePower = 1.5f;　//トランポリンの倍率
    private float TrampolineGraceTime = 0.15f;　//トランポリンの効果を維持する時間
    private float TrampolineTimer = 0f;　//トランポリンの効果を管理するタイマー
    private bool TrampolineJumping = false;　//トランポリンのジャンプ中判定

    [Header("重力設定")]
    public float gravityPower = 20f; // 自前重力
    private Vector3 gravityDirection = Vector3.down;

    void Start()
    {
        RigidBody = GetComponent<Rigidbody>();
        state = GetComponent<PlayerState>();
        move = GetComponent<PlayerMove>();
        special = GetComponent<PlayerSpecialAction>();
        knock = GetComponent<PlayerKnockBack>();
        jumpTimeMax = jumpTimeMaxSaving;
    }

    void Update()
    {
        //ジャンプ入力受付
        if (!knock.knockBacking)
            JumpOperation();

        //ジャンプのクールタイム
        if (jumpCoolActive)
        {
            jumpCoolCounter += Time.deltaTime;
            state.isGrounded = false;
            state.isJumpMoveOK = false;
            if (jumpCoolCounter > jumpCoolTime)
            {
                jumpCoolActive = false;
                jumpCoolCounter = 0f;
            }
        }
    }

    private void FixedUpdate()
    {
        　//反転してる時だけ自前重力
        if (move.IsUpsideDown)
        {
            RigidBody.useGravity = false;

            // 反転時のカスタム重力
            Vector3 gravityDirection = Vector3.up;
            RigidBody.AddForce(gravityDirection * gravityPower, ForceMode.Acceleration);
        }
        else
        {
            // 通常時はUnityの重力に完全任せる
            RigidBody.useGravity = true;
        }

        //ジャンプ
        if (jumping)
        {
            jumpTime += Time.fixedDeltaTime;
            float JumpPower = groundJumpPower;

            special.headingAttack.SetActive(true);

            if (isOnTrampoline)
            {
                TrampolineJumping = true;
                TrampolineTimer = TrampolineGraceTime;
            }

            Jump(JumpPower);
        }

        //トランポリンの効果
        if (TrampolineJumping)
        {
            TrampolineTimer -= Time.fixedDeltaTime;
            if (TrampolineTimer <= 0)
                TrampolineJumping = false;
        }
    }

    // ジャンプの入力処理
    private void JumpOperation()
    {
        //ジャンプ入力の受付
        if (state.inputManager.jumpDown && !special.meteorHighJumpOK && !isJumpQueued)
        {
            isJumpQueued = true;
            jumpQueueCounter = 0f;
        }

        //ジャンプ可能かの判定
        if (((coyoteCounter <= coyoteTime || state.isJumpMoveOK) || (move.IsUpsideDown && state.isGrounded))
             && !jumpCoolActive
             && special.highJumpChargeCounter < special.highJumpChargeTime)
        {
            //ジャンプ開始
            if (isJumpQueued)
            {
                jumping = true;
                jumpCoolActive = true;
                jumpTime = 0f;
                jumpTimeMax = jumpTimeMaxSaving;
                isJumpQueued = false;

                //着地ジャンプ
                if (state.landingJumpOn)
                {
                    state.LandingJumpReset();
                }
            }
            else if (state.inputManager.jumpHeld && special.meteorHighJumpOK && state.landingJumpOn)  //メテオジャンプ（今は使われてない）
            {
                //メテオハイジャンプ
                if (special.meteorDropCounter >= special.meteorDropTime)
                {
                    jumpCoolActive = true;
                    special.meteorHighJump = true;
                    special.headingAttack.SetActive(true);
                }
                special.meteorHighJumpOK = false;
                state.LandingJumpReset();
            }
        }

        //ジャンプ入力やめ
        if (jumping && !state.inputManager.jumpHeld)
        {
            //最低ジャンプ時間
            float minJumpTime = jumpTimeMaxSaving * 0.3f;

            if (jumpTime < minJumpTime)
            {
                //最低時間まではジャンプが続くように上限時間を上書き
                jumpTimeMax = minJumpTime;
            }
            else
            {
                jumping = false;

                //離した瞬間の勢い殺すぞ禿
                //if (!move.IsUpsideDown && RigidBody.linearVelocity.y > 0f)
                //{
                //    RigidBody.linearVelocity = new Vector3(RigidBody.linearVelocity.x, RigidBody.linearVelocity.y * 0.5f, RigidBody.linearVelocity.z);
                //}
                //else if (move.IsUpsideDown && RigidBody.linearVelocity.y < 0f)  //天井反転時の上昇を殺す
                //{
                //    RigidBody.linearVelocity = new Vector3(RigidBody.linearVelocity.x, RigidBody.linearVelocity.y * 0.5f, RigidBody.linearVelocity.z);
                //}
            }
        }

        //ジャンプ先行入力
        if (isJumpQueued)
        {
            Debug.Log("ジャンプ先行入力中");
            jumpQueueCounter += Time.deltaTime;
            if (jumpQueueCounter > jumpQueueTime)
                isJumpQueued = false;
        }
    }

    public void Jump(float jumpPower)
    {
        //Y方向速度をリセット
        Vector3 vel = RigidBody.linearVelocity;
        vel.y = 0f;
        RigidBody.linearVelocity = vel;

        if (TrampolineJumping)
            jumpPower *= TrampolinePower;

        float time = jumpTime / jumpTimeMaxSaving;
        float power = jumpPower * jumpCurve.Evaluate(time);

        if (jumpTime >= jumpTimeMax)
            jumping = false;

        // 上下反転ジャンプ
        Vector3 jumpDirection = move.IsUpsideDown ? Vector3.down : Vector3.up;

        RigidBody.AddForce(power * jumpDirection, ForceMode.Impulse);

        //横速度制限
        Vector3 horizontalVelocity = new Vector3(RigidBody.linearVelocity.x, 0f, RigidBody.linearVelocity.z);
        if (horizontalVelocity.magnitude > maxJumpSpeed)
            RigidBody.linearVelocity = new Vector3(Mathf.Sign(RigidBody.linearVelocity.x) * maxJumpSpeed, RigidBody.linearVelocity.y, RigidBody.linearVelocity.z);
    }
}
