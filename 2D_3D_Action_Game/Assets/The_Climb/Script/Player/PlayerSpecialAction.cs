using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpecialAction : MonoBehaviour
{
    Rigidbody RigidBody;
    PlayerState state;
    PlayerMove move;
    PlayerJump jump;
    PlayerKnockBack knock;

    public GameObject headingAttack;
    public GameObject meteorDropAttack;
    public GameObject quickJumpAttack;

    public float highJumpChargeTime = 0.8f;  //ハイジャンプのチャージ時間
    public float highJumpChargeCounter = 0f;  //ハイジャンプのチャージカウンター
    private bool highJump = false;       //ハイジャンプする判定
    private float highJumpPower = 30f;   //ハイジャンプのパワー
    public bool highJumpUsed = false;   //ハイジャンプを使用したか判定

    private bool quickJump = false;      //クイックジャンプする判定
    public bool isGroundNear = false;   //地面が近いとクイックジャンプ発動させない用
    private float quickJumpPowerX = 10f;  //クイックジャンプの横のパワー
    private float quickJumpPowerY = 10f;  //クイックジャンプの縦のパワー
    public bool quickJumpUsed = false;   //クイックジャンプを使用したか判定

    public bool meteorDrop = false;      //メテオドロップする判定
    public bool meteorDropUsed = false;   //メテオドロップを使用したか判定
    public bool meteorHighJumpOK = false;  //メテオドロップからのハイジャンプへの移行ができるか
    private float meteorDropPower = 30f;  //メテオドロップのパワー
    private float meteorDropAngle = 135f;  //メテオドロップの角度
    private float meteorDropXMove;        //メテオドロップのX軸移動
    private float meteorDropYMove;        //メテオドロップのY軸移動
    public bool meteorHighJump = false;  //メテオドロップ後のハイジャンプ
    public float meteorDropTime = 0.37f;  //メテオドロップからのハイジャンプに移行できるまでの時間
    public float meteorDropCounter = 0f;  //メテオドロップのカウンター

    void Start()
    {
        RigidBody = GetComponent<Rigidbody>();
        state = GetComponent<PlayerState>();
        move = gameObject.GetComponent<PlayerMove>();
        jump = gameObject.GetComponent<PlayerJump>();
        knock = gameObject.GetComponent<PlayerKnockBack>();

        headingAttack = transform.Find("HeadingAttack").gameObject;
        meteorDropAttack = transform.Find("MeteorDropAttack").gameObject;
        quickJumpAttack = transform.Find("QuickJumpAttack").gameObject;

        float meteorDropDirection = meteorDropAngle * Mathf.Deg2Rad;

        meteorDropXMove = Mathf.Sin(meteorDropDirection);
        meteorDropYMove = Mathf.Cos(meteorDropDirection);
    }

    void Update()
    {
        if (!knock.knockBacking && !state.carryingBuddy)  //ノックバック中は不可
        {
            if (state.highJumpOn)
            {
                //チャージジャンプのチャージキー操作
                HighJumpChargeOperation();
            }

            if (state.meteorDropOn)
            {
                //メテオドロップキー操作
                MeteorDropOperation();
            }

            if (state.quickJumpOn)
            {
                //クイックジャンプキー操作
                QuickJumpOperation();
            }
        }

        //地面が近いか（クイックジャンプ用判定）
        if (!isGroundNear && RigidBody.linearVelocity.y < 0)
        {
            isGroundNear = Physics.CheckSphere(state.jumpMoveOKCheck.position + Vector3.down * 0.4f, 0.19f, state.groundLayer);
        }
        else if (state.isAir && isGroundNear)
        {
            isGroundNear = false;
        }
    }

    private void FixedUpdate()
    {
        //ハイジャンプ実行
        if (highJump)
        {
            HighJumpUse();
        }

        //メテオドロップ実行
        if (meteorDrop)
        {
            MeteorDropUse();
        }

        //メテオハイジャンプ実行
        if (meteorHighJump)
        {
            MeteorHighJumpUse();
        }

        //クイックジャンプ実行
        if (quickJump)
        {
            QuickJumpUse();
        }
    }

    private void HighJumpChargeOperation()
    {
        if (jump.jumpCoolActive || state.isAir || (state.inputManager.jumpUp && highJumpChargeCounter <= 0.2f)) //ハイジャンプ不可
        {
            highJumpChargeCounter = 0f;
        }
        else if (state.inputManager.jumpUp)　//ハイジャンプ放す
        {
            if (highJumpChargeCounter >= highJumpChargeTime)  //チャージできてればOK
            {
                highJump = true;
                highJumpUsed = true;
                headingAttack.SetActive(true);
            }
            //else
            //{
            //    jump.jumping = true;
            //}

            jump.jumpCoolActive = true;
            highJumpChargeCounter = 0f;
        }
        else if (state.isGrounded && state.inputManager.jumpHeld && !state.landingJumpOn && !state.inputManager.jumpDown) //ハイジャンプおしっぱの状態
        {
            RigidBody.linearVelocity = new Vector3(0, RigidBody.linearVelocity.y, 0);
            highJumpChargeCounter += Time.deltaTime;
            move.slipping = false;
        }
    }

    private void MeteorDropOperation()
    {
        if (state.isAir && state.inputManager.downHeld && state.inputManager.jumpDown && !meteorDropUsed)  //空中でまだメテオドロップ使ってないか
        {
            meteorDrop = true;
            meteorDropUsed = true;
            meteorHighJumpOK = true;
            meteorDropAttack.SetActive(true);

            if (state.playerDirectionRight)  //向いてる方向によって進む方向変化
            {
                meteorDropXMove = Mathf.Abs(meteorDropXMove);
            }
            else
            {
                meteorDropXMove = Mathf.Abs(meteorDropXMove) * -1f;
            }
        }
        
    }

    private void QuickJumpOperation()
    {
        if (state.isAir && state.inputManager.jumpDown && !quickJumpUsed && !meteorDropUsed && !isGroundNear)  //空中で地面に近くなく、メテオもクイックもまだ使ってないか
        {
            quickJump = true;
            quickJumpUsed = true;
            quickJumpAttack.SetActive(true);
        }
        
        //横移動入力中ならジャンプ力低下
        if (move.MoveInput == 1f || move.MoveInput == -1f)
        {
            quickJumpPowerY = 7f;
        }
        else
        {
            quickJumpPowerY = 12f;
        }
    }

    public void HighJumpUse()
    {
        RigidBody.AddForce(new Vector3(0, highJumpPower, 0), ForceMode.Impulse);
        jump.jumpCoolActive = true;
        move.airMaxSpeed = 0.005f;
        highJump = false;
    }

    private void MeteorDropUse()
    {
        RigidBody.useGravity = false;
        RigidBody.linearVelocity = new Vector3(0, 0, 0);
        meteorDropCounter += Time.fixedDeltaTime;

        //壁にぶつかったらメテオハイジャンプ終わり
        if (((state.isLeftWall && !state.playerDirectionRight) || (state.isRightWall && state.playerDirectionRight)) && !state.isGrounded)
        {
            meteorHighJumpOK = false;
            RigidBody.linearVelocity = Vector3.zero;
        }
        else
        {
            //斜め下に移動させる
            RigidBody.AddForce(meteorDropPower * new Vector3(meteorDropXMove, meteorDropYMove, 0), ForceMode.Impulse);
        }

        //メテオドロップ終わり
        if ((state.isLeftWall && !state.playerDirectionRight) || (state.isRightWall && state.playerDirectionRight) || state.isGrounded)
        {
            RigidBody.useGravity = true;
            meteorDrop = false;
            RigidBody.linearVelocity = Vector3.zero;
        }
    }

    private void QuickJumpUse()
    {
        RigidBody.linearVelocity = new Vector3(RigidBody.linearVelocity.x, 0, 0);  //ジャンプ力重複させないようにリセット
        RigidBody.AddForce(new Vector3(quickJumpPowerX * move.MoveInput, quickJumpPowerY, 0), ForceMode.Impulse);
        
        //クイックジャンプの横移動速度制限
        if (move.MoveInput != 0f)
        {
            move.airMaxSpeed = 15f;
        }
        quickJump = false;
    }

    private void MeteorHighJumpUse()
    {
        headingAttack.SetActive(true);

        RigidBody.linearVelocity = Vector3.zero;
        RigidBody.AddForce(new Vector3(meteorDropPower * meteorDropXMove, highJumpPower, 0), ForceMode.VelocityChange);

        meteorHighJump = false;
        jump.jumpCoolActive = true;
    }
}
