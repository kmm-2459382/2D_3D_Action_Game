using System.Collections;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

[RequireComponent(typeof(Rigidbody))]
public class MimicFollower : MonoBehaviour
{
    [Header("模倣設定")]
    public float delay = 1.0f; // 遅延時間（秒）
    public float pushForceUp = 10f;
    public float pushForceForward = 5f;

    [Header("プレイヤー制御")]
    public float disableControlTime = 0.8f; // 操作不能時間（秒）

    private Rigidbody rb;
    private Animator animator;
    private int frameDelay;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.isKinematic = false;
    }

    private void Start()
    {
        frameDelay = Mathf.RoundToInt(delay / Time.fixedDeltaTime);
    }

    private void FixedUpdate()
    {
        var recorder = PlayerMimicRecorder.Instance;
        if (recorder == null) return;
        if (recorder.HistoryCount <= frameDelay) return;

        if (recorder.TryGetHistory(frameDelay, out var pos, out var rot, out var speed, out var motionSpeed, out var grounded, out var jump, out var freeFall, out var inWater))
        {
            // 移動
            rb.MovePosition(pos);

            // 3D向き修正（水平のみ）
            Vector3 moveDir = pos - transform.position;
            moveDir.y = 0f;
            if (moveDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                rb.MoveRotation(targetRot);
            }
            else
            {
                rb.MoveRotation(rot);
            }

            // アニメーションパラメータの同期
            if (animator != null)
            {
                animator.SetFloat("Speed", speed, 0.1f, Time.fixedDeltaTime);
                animator.SetFloat("MotionSpeed", motionSpeed, 0.1f, Time.fixedDeltaTime);
                animator.SetBool("Grounded", grounded);
                animator.SetBool("Jump", jump);
                animator.SetBool("FreeFall", freeFall);
                animator.SetBool("InWater", inWater);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        CharacterController playerController = other.GetComponent<CharacterController>();
        if (playerController != null)
        {
            StartCoroutine(PushCharacterControllerPlayer(other.gameObject, playerController));
        }
    }

    // 吹っ飛ばした瞬間に自然な放物線を描かせるコルーチン
    private IEnumerator PushCharacterControllerPlayer(GameObject playerObj, CharacterController controller)
    {
        var marioController = playerObj.GetComponent<StarterAssets.MarioStyleController>();
        var move3D = playerObj.GetComponent<PlayerMove3D>();
        var playerScript = playerObj.GetComponent<PlayerController>();

        // 1. 操作系スクリプトを一時停止
        if (marioController != null) marioController.enabled = false;
        if (move3D != null) move3D.enabled = false;
        if (playerScript != null) playerScript.enabled = false;

        Vector3 pushDirection = (controller.transform.position - transform.position).normalized;
        pushDirection.y = 0f; // 水平方向の押し出し

        // 初速の設定
        float verticalVel = pushForceUp;
        float currentHorizontalSpeed = pushForceForward;

        float elapsedTime = 0f;

        while (elapsedTime < disableControlTime)
        {
            // 空中判定にする
            if (marioController != null)
            {
                marioController.Grounded = false;
            }

            // 重力による垂直速度の減衰（verticalVel を正しく使用）
            verticalVel += -30f * Time.deltaTime;

            // 水平方向は少しずつ減速させる（空気抵抗）
            currentHorizontalSpeed = Mathf.Lerp(currentHorizontalSpeed, 0f, Time.deltaTime * 2f);

            // 移動ベクトルの合成
            Vector3 moveVector = (pushDirection * currentHorizontalSpeed + Vector3.up * verticalVel) * Time.deltaTime;

            // CharacterControllerで移動
            controller.Move(moveVector);

            // もし途中で地面に接地したらループを抜ける
            if (controller.isGrounded && verticalVel < 0f)
            {
                break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 2. 操作を復帰させる
        if (marioController != null)
        {
            marioController.Grounded = true;
            marioController.enabled = true;
        }
        if (move3D != null) move3D.enabled = true;
        if (playerScript != null) playerScript.enabled = true;
    }
}