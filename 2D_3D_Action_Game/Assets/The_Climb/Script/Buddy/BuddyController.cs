using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;
using Zenject.SpaceFighter;

public class BuddyController : MonoBehaviour
{
    private Rigidbody RigidBody;
    private GameObject player;
    private PlayerState state;
    private BuddyCarry buddyCarry;
    private PlayerMove playerMove;
    private PlayerMove3D playerMove3D;
    private PositionConstraint positionConstraint;
    private ConstraintSource currentSource;
    [SerializeField] private Animator animator;

    private bool buddyDirectionRight;  //バディが右向いてるか判定、falseなら左向き
    public bool moving = false;        //Buddyが動いてるか判定
    private float speed = 4f;          //Buddyの移動スピード
    public float buddyTargetX = 0f;    //Buddyが向かうX座標
    public bool beingKidnapped = false;  //誘拐されてる

    [SerializeField] private bool isLeftWall;          //左壁判定
    [SerializeField] private bool isRightWall;         //右壁判定
    private LayerMask groundLayer;     //地面レイヤー

    private float buddyFallSpeed = -19f;  //プレイヤーの落下速度

    //バディが誘導される地点指定
    public void GuideTo(float x)
    {
        buddyTargetX = x;
        moving = true;

        //バディの向かう方向からどっち向くか決める
        if (buddyTargetX - transform.position.x > 0)
        {
            buddyDirectionRight = true;
        }
        else if (buddyTargetX - transform.position.x < 0)
        {
            buddyDirectionRight = false;
        }
    }

    void Start()
    {
        player = GameObject.Find("PlayerModel");
        state = player.GetComponent<PlayerState>();
        buddyCarry = player.GetComponent<BuddyCarry>();
        playerMove = player.GetComponent<PlayerMove>();
        playerMove3D = player.GetComponent<PlayerMove3D>();
        positionConstraint = GetComponent<PositionConstraint>();
        groundLayer = GameLayer.ToMask(GameLayers.GROUND);
        RigidBody = gameObject.GetComponent<Rigidbody>();

        if (animator == null)
        {
            Debug.LogError("BuddyのAnimatorアタッチされてない");
        }
    }

    void Update()
    {
        //左壁判定（カプセル形）
        isLeftWall = Physics.CheckCapsule(transform.position + Vector3.left * 0.3f + Vector3.up * 0.49f, transform.position + Vector3.left * 0.3f + Vector3.down * 0.49f, 0.001f, groundLayer);
        //右壁判定（カプセル形）
        isRightWall = Physics.CheckCapsule(transform.position + Vector3.right * 0.3f + Vector3.up * 0.49f, transform.position + Vector3.right * 0.3f + Vector3.down * 0.49f, 0.001f, groundLayer);

        //おんぶされてるとき重力働かないように＆物理演算の影響を受けないように
        if (state.carryingBuddy)
        {
            RigidBody.useGravity = false;
            RigidBody.isKinematic = true;
            RigidBody.linearVelocity = Vector3.zero;
            RigidBody.angularVelocity = Vector3.zero;

            //走ってるかの判定
            bool isRunning = false;
            if (playerMove != null)
            {
                isRunning = state.isGrounded && Mathf.Abs(playerMove.MoveInput) > 0f;
            }
            else if (playerMove3D != null)
            {
                Vector2 moveInput = playerMove3D.GetMoveInput();
                isRunning = state.isGrounded && moveInput.sqrMagnitude > 0f;
            }

            //誘拐中の処理（空中判定を強制）
            if (beingKidnapped)
            {
                //空中判定を強制してアニメーション更新
                UpdateAnimation(false, true);
            }
            else
            {
                //通常のアニメーション更新
                UpdateAnimation(isRunning, !state.isGrounded);

                //ジャンプの同期
                if (Input.GetKeyDown(KeyCode.Space) && state.isGrounded)
                {
                    animator.SetTrigger("JumpAnimStep1");
                }
            }
        }
        else  //おんぶされてないときは通常通り重力働かせる＆物理演算の影響受けるように
        {
            RigidBody.useGravity = true;
            RigidBody.isKinematic = false;

            float targetSpeed = moving ? 1f : 0f;
            float currentSpeed = animator.GetFloat(Const.Speed);
            animator.SetFloat(Const.Speed, Mathf.MoveTowards(currentSpeed, targetSpeed, Time.deltaTime * 6f));

            if (HasParameter("MotionSpeed", animator))
            {
                float currentMotionSpeed = animator.GetFloat("MotionSpeed");
                animator.SetFloat("MotionSpeed", Mathf.MoveTowards(currentMotionSpeed, targetSpeed, Time.deltaTime * 6f));
            }
            if (HasParameter("Grounded", animator))
            {
                animator.SetBool("Grounded", true);
            }
            if (HasParameter("isAir", animator))
            {
                animator.SetBool("isAir", false);
            }
            if (HasParameter("IsAir", animator))
            {
                animator.SetBool("IsAir", false);
            }

            //誘導により動く
            if (moving)
            {
                BuddyMove();
            }
        }

        //落下速度調整
        if (RigidBody.linearVelocity.y < buddyFallSpeed)
        {
            RigidBody.linearVelocity = new Vector3(RigidBody.linearVelocity.x, buddyFallSpeed, 0);
        }
    }

    //アニメーション更新
    private void UpdateAnimation(bool isRunning, bool forceAir)
    {
        float targetSpeed = isRunning ? 1f : 0f;
        float currentSpeed = animator.GetFloat(Const.Speed);
        animator.SetFloat(Const.Speed, Mathf.MoveTowards(currentSpeed, targetSpeed, Time.deltaTime * 6f));

        if (HasParameter("MotionSpeed", animator))
        {
            float currentMotionSpeed = animator.GetFloat("MotionSpeed");
            animator.SetFloat("MotionSpeed", Mathf.MoveTowards(currentMotionSpeed, targetSpeed, Time.deltaTime * 6f));
        }

        if (HasParameter("Grounded", animator))
        {
            animator.SetBool("Grounded", !forceAir && state.isGrounded);
        }

        if (HasParameter("isAir", animator))
        {
            animator.SetBool("isAir", forceAir || !state.isGrounded);
        }

        if (HasParameter("IsAir", animator))
        {
            animator.SetBool("IsAir", forceAir || !state.isGrounded);
        }
    }

    //誘導地点に向かって動く
    private void BuddyMove()
    {
        Vector3 pos = transform.position;
        Vector3 target = new Vector3(buddyTargetX, pos.y, pos.z);
        //移動
        transform.position = Vector3.MoveTowards(pos, target, speed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - buddyTargetX) < 0.05f || (isLeftWall && !buddyDirectionRight) || (isRightWall && buddyDirectionRight))
        {
            moving = false;
        }
    }

    //Buddyの追従するオブジェクトを変える
    public void SetConstraintTarget(Transform newTarget)
    {
        //Constraint 評価を一時停止
        positionConstraint.constraintActive = false;

        ConstraintSource playerSource = positionConstraint.GetSource(0);   //プレイヤーのソース
        ConstraintSource stalkerSource = positionConstraint.GetSource(1);  //ストーカーハンドのソース

        //0番目：プレイヤー（固定）
        if (newTarget.name == "PlayerModel")
        {
            playerSource.weight = 1f;
            stalkerSource.weight = 0f;
        }
        else  //1番目：誘拐対象（StalkerHand）
        {
            stalkerSource.sourceTransform = newTarget;
            stalkerSource.weight = 1f;
            playerSource.weight = 0f;
        }
        positionConstraint.SetSource(0, playerSource);
        positionConstraint.SetSource(1, stalkerSource);

        //次フレームでConstraintを再評価
        StartCoroutine(ReenableNextFrame());
    }

    private IEnumerator ReenableNextFrame()
    {
        yield return null;

        positionConstraint.constraintActive = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            buddyCarry.nearBuddy = true;  //Buddyが近くにいる
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            buddyCarry.nearBuddy = false;  //Buddyが近くにいる
        }
    }

    // Animatorに指定した名前のパラメータが存在するかどうかを安全に確認する関数
    private bool HasParameter(string paramName, Animator targetAnimator)
    {
        foreach (AnimatorControllerParameter param in targetAnimator.parameters)
        {
            if (param.name == paramName) return true;
        }
        return false;
    }
}
