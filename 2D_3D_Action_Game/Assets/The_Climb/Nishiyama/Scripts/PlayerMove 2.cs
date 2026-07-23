using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerMove2 : MonoBehaviour, IConveyorReceiver
{
    Rigidbody RigidBody;
    PlayerState state;
    PlayerJump jump;
    PlayerSpecialAction special;
    PlayerKnockBack knock;

    [SerializeField] private InputActionReference leftMoveAction;
    [SerializeField] private InputActionReference rightMoveAction;

    private float groundMoveForce = 0.7f;     //プレイヤーの地上移動速度
    public float groundMaxSpeed = 6.459797f;   //プレイヤーの地上最高速度記憶
    public float moveInput = 0f;        //プレイヤーの移動方向
    private float airMoveForce = 50f;    //空中での移動速度
    public float airMaxSpeed = 10f;     //空中での速度制限

    public bool slipping = false;        //着地後勢い止めず滑ってる判定
    public Vector3 slipVelocity;                //滑り時のVelocity

    public float MoveInput => moveInput; // ←読み取り専用プロパティ
    public PlayerAnimation PlayerAnimation;
    public PlayerState State => state;

    private bool OnBelt = false;                 //ベルトコンベアに乗っているか
    private Vector3 BeltVelocity = Vector3.zero; //ベルトコンベアの速度(未接触時はゼロ)

    [SerializeField] private bool reverseHorizontal = false;

    [SerializeField] private bool upsideDown = false; // 天井歩行モード
    [SerializeField] private float customGravity = 9.81f; // 通常重力に近い値

    public bool IsUpsideDown => upsideDown;


    void Start()
    {
        RigidBody = GetComponent<Rigidbody>();
        state = GetComponent<PlayerState>();
        jump = gameObject.GetComponent<PlayerJump>();
        special = gameObject.GetComponent<PlayerSpecialAction>();
        knock = gameObject.GetComponent<PlayerKnockBack>();

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

    public void ToggleUpsideDown()
    {
        upsideDown = !upsideDown; // 状態を反転

        // 見た目の上下反転
        Vector3 scale = transform.localScale;
        scale.y = Mathf.Abs(scale.y) * (upsideDown ? -1 : 1);
        transform.localScale = scale;

        // ← ここで慣性をリセット！（グワン防止）
        RigidBody.linearVelocity = Vector3.zero;

        // 反転直後に少しだけ真上へ押し上げる
        RigidBody.AddForce(Vector3.up * 5f, ForceMode.VelocityChange);

        // プレイヤー位置の微調整（めり込み防止）
        Vector3 pos = transform.position;
        pos.y += upsideDown ? 0.5f : -0.5f;
        transform.position = pos;

        // 地面の判定をリセット
        state.isGrounded = false;

        Debug.Log($"{name} が上下反転！（現在: {(upsideDown ? "天井" : "地面")}）");
    }


    private void Update()
    {
        //移動キー操作
        if (!special.meteorDrop && !knock.knockBacking)　//ノックバック、メテオドロップ中は不可
        {
            MoveOperation();
        }
    }

    void FixedUpdate()
    {
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

        ApplyCustomGravity();
    }

    private void MoveOperation()
    {
        if (state.inputManager.leftHeld && state.inputManager.rightHeld ||
            !state.inputManager.leftHeld && !state.inputManager.rightHeld)  //止まる
        {
            moveInput = 0f;
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

        if (moveInput != 0f)
        {
            // 地上：慣性なし、即応する左右移動
            Vector3 force = new Vector3(moveInput, 0f, 0f) * groundMoveForce;
            RigidBody.AddForce(force);
            RigidBody.linearVelocity = new Vector3(force.x * Time.deltaTime * 1000.0f, RigidBody.linearVelocity.y, 0f);
        }
        else if(!special.meteorHighJump && !jump.jumpCoolActive && RigidBody.linearVelocity.x != 0f)
        {
            RigidBody.linearVelocity = new Vector3(0f, RigidBody.linearVelocity.y, 0f);
        }

        
    }

    private void AirPlayerMove()
    {
        // 空中：左右に力を加える
        Vector3 force = new Vector3(moveInput, 0f, 0f) * airMoveForce;
        RigidBody.AddForce(force, ForceMode.Acceleration);

        // 最大空中速度を制限
        if (!special.quickJumpUsed && !special.highJumpUsed)
        {
            airMaxSpeed = 10f;
        }
        Vector3 horizontalVelocity = new Vector3(RigidBody.linearVelocity.x, 0f, 0f);
        if (horizontalVelocity.magnitude > airMaxSpeed)
        {
            RigidBody.linearVelocity = new Vector3(Mathf.Sign(RigidBody.linearVelocity.x) * airMaxSpeed, RigidBody.linearVelocity.y, RigidBody.linearVelocity.z);
        }

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

    private void ApplyCustomGravity()
    {
        if (upsideDown)
        {
            if (!state.isGrounded)
                RigidBody.AddForce(Vector3.up * customGravity, ForceMode.Acceleration);
        }
        else
        {
            if (!state.isGrounded)
                RigidBody.AddForce(Vector3.down * customGravity, ForceMode.Acceleration);
        }
    }

}