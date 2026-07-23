using System.Collections;
using UnityEngine;

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
        animator = GetComponent<Animator>();
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

        if (recorder.TryGetHistory(frameDelay, out var pos, out var rot, out var anim))
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

            // アニメーション同期
            animator.Play(anim.shortNameHash, 0, anim.normalizedTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Rigidbody playerRb = other.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            // 吹っ飛ばし
            Vector3 pushDir = (other.transform.position - transform.position).normalized;
            playerRb.AddForce(Vector3.up * pushForceUp + pushDir * pushForceForward, ForceMode.Impulse);
        }

        // 操作不能処理
        StartCoroutine(TemporarilyDisablePlayerControl(other.gameObject));
    }

    private IEnumerator TemporarilyDisablePlayerControl(GameObject player)
    {
        var move3D = player.GetComponent<PlayerMove3D>();
        var controller = player.GetComponent<PlayerController>();
        var playerRb = player.GetComponent<Rigidbody>();

        // 操作停止
        if (move3D != null) move3D.enabled = false;
        if (controller != null) controller.enabled = false;

        // 半分の時間後に慣性リセット
        yield return new WaitForSeconds(disableControlTime * 0.7f);

        if (move3D != null)
        {
            move3D.ResetHorizontalVelocity(0.5f); // Yを半分残す
        }
        else if (playerRb != null)
        {
            Vector3 v = playerRb.linearVelocity;
            playerRb.linearVelocity = new Vector3(0f, v.y * 0.5f, 0f);
            playerRb.angularVelocity = Vector3.zero;
        }

        // 残りの操作不能時間待機
        yield return new WaitForSeconds(disableControlTime * 0.3f);

        // 操作再開
        if (move3D != null) move3D.enabled = true;
        if (controller != null) controller.enabled = true;
    }
}
