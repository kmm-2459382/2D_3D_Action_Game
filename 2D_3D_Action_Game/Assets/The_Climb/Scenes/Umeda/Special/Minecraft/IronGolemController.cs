using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class IronGolemEnemy : MonoBehaviour
{
    [Header("索敵")]
    public float searchRange = 6f;
    public string targetTag = "Player";

    [Header("移動")]
    public float wanderSpeed = 1.5f;   // 未発見時
    public float chaseSpeed = 2.3f;    // 発見時
    public float attackRange = 1.5f;

    [Header("攻撃")]
    public float attackCooldown = 1.0f;
    public float attackForceUp = 30f;

    [Header("歩行アニメ（角度）")]
    public float legSwingAngle = 45f;
    public float armSwingAngle = 45f;

    [Header("歩行アニメ（速度）")]
    public float legSwingSpeed = 5f;
    public float armSwingSpeed = 5f;

    [Header("索敵前の歩行（弱め）")]
    public float idleLegSwingAngle = 15f;
    public float idleArmSwingAngle = 10f;
    public float idleLegSwingSpeed = 2f;
    public float idleArmSwingSpeed = 2f;

    [Header("攻撃アニメ（腕）")]
    public float attackWindupAngle = 0f;
    public float attackSwingAngle = 170f;
    public float windupDuration = 0.25f;
    public float swingDuration = 0.3f;
    public float returnDuration = 0.3f;

    [Header("攻撃時 足リセット")]
    public float legReturnDuration = 0.12f;

    [Header("向き（Y回転）private関数")]
    private float forwardY = 0f;
    private float leftY = 90f;
    private float rightY = -90f;
    private float backY = 180f;

    [Header("参照（Inspector）")]
    public Transform leftLeg;
    public Transform rightLeg;
    public Transform leftArm;
    public Transform rightArm;

    private Rigidbody rb;
    private Transform target;
    private bool isAttacking;

    private float attackTimer;
    private float walkTimer;

    // 徘徊用
    private Vector3 wanderDir;
    private float wanderTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        PickNewWanderDirection();
    }

    void Update()
    {
        attackTimer += Time.deltaTime;

        SearchTarget();

        if (isAttacking) return;

        if (target != null)
        {
            ChaseTarget();
        }
        else
        {
            Wander();
        }

        WalkAnimation();
    }

    // ======================
    // 索敵
    // ======================
    void SearchTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag(targetTag);

        float closestDist = searchRange;
        Transform closest = null;

        foreach (GameObject p in players)
        {
            float d = Vector3.Distance(transform.position, p.transform.position);
            if (d <= closestDist)
            {
                closestDist = d;
                closest = p.transform;
            }
        }

        target = closest;
    }

    // ======================
    // 追跡
    // ======================
    void ChaseTarget()
    {
        Vector3 toTarget = target.position - transform.position;
        float distance = toTarget.magnitude;

        UpdateFacing(toTarget);

        if (distance <= attackRange && attackTimer >= attackCooldown)
        {
            StartCoroutine(Attack());
            return;
        }

        Move(toTarget.normalized, chaseSpeed);
    }

    // ======================
    // 徘徊
    // ======================
    void Wander()
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0f)
        {
            PickNewWanderDirection();
        }

        UpdateFacing(wanderDir);
        Move(wanderDir, wanderSpeed);
    }

    void PickNewWanderDirection()
    {
        wanderDir = Random.value > 0.5f ? Vector3.right : Vector3.left;
        wanderTimer = Random.Range(1.5f, 3f);
    }

    // ======================
    // 移動共通
    // ======================
    void Move(Vector3 dir, float speed)
    {
        transform.position += dir * speed * Time.deltaTime;
    }

    // ======================
    // 向き（Y回転）
    // ======================
    void UpdateFacing(Vector3 dir)
    {
        float absX = Mathf.Abs(dir.x);
        float absZ = Mathf.Abs(dir.z);

        float y;
        if (absX >= absZ)
            y = dir.x < 0 ? leftY : rightY;
        else
            y = dir.z > 0 ? forwardY : backY;

        transform.rotation = Quaternion.Euler(0f, y, 0f);
    }

    // ======================
    // 歩行アニメ
    // ======================
    void WalkAnimation()
    {
        walkTimer += Time.deltaTime;

        bool found = target != null;

        float legAngle = Mathf.Sin(walkTimer * (found ? legSwingSpeed : idleLegSwingSpeed))
                       * (found ? legSwingAngle : idleLegSwingAngle);

        float armAngle = Mathf.Sin(walkTimer * (found ? armSwingSpeed : idleArmSwingSpeed))
                       * (found ? armSwingAngle : idleArmSwingAngle);

        leftLeg.localRotation = Quaternion.Euler(legAngle, 0f, 0f);
        rightLeg.localRotation = Quaternion.Euler(-legAngle, 0f, 0f);

        leftArm.localRotation = Quaternion.Euler(-armAngle, 0f, 0f);
        rightArm.localRotation = Quaternion.Euler(armAngle, 0f, 0f);
    }

    // ======================
    // 攻撃
    // ======================
    IEnumerator Attack()
    {
        isAttacking = true;
        attackTimer = 0f;

        // ★ 攻撃直前に足を素早く揃える
        yield return StartCoroutine(ReturnLegsQuickly());

        ResetLimbsImmediate();

        // 腕の万歳 → 振り下ろし
        yield return StartCoroutine(RaiseArms(attackWindupAngle, windupDuration));
        yield return StartCoroutine(RaiseArms(attackSwingAngle, swingDuration));

        if (target != null)
        {
            Rigidbody targetRb = target.GetComponent<Rigidbody>();
            if (targetRb != null)
            {
                targetRb.linearVelocity = Vector3.zero;
                targetRb.AddForce(Vector3.up * attackForceUp, ForceMode.Impulse);
            }
        }

        // 腕を滑らかに戻す
        yield return StartCoroutine(ReturnArmsSmoothly());

        isAttacking = false;
    }

    IEnumerator ReturnLegsQuickly()
    {
        Quaternion startL = leftLeg.localRotation;
        Quaternion startR = rightLeg.localRotation;
        Quaternion end = Quaternion.identity;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / legReturnDuration;
            leftLeg.localRotation = Quaternion.Slerp(startL, end, t);
            rightLeg.localRotation = Quaternion.Slerp(startR, end, t);
            yield return null;
        }
    }

    IEnumerator RaiseArms(float targetAngle, float duration)
    {
        Quaternion startL = leftArm.localRotation;
        Quaternion startR = rightArm.localRotation;
        Quaternion end = Quaternion.Euler(targetAngle, 0f, 0f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            leftArm.localRotation = Quaternion.Slerp(startL, end, t);
            rightArm.localRotation = Quaternion.Slerp(startR, end, t);
            yield return null;
        }
    }

    IEnumerator ReturnArmsSmoothly()
    {
        Quaternion startL = leftArm.localRotation;
        Quaternion startR = rightArm.localRotation;
        Quaternion end = Quaternion.identity;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / returnDuration;
            leftArm.localRotation = Quaternion.Slerp(startL, end, t);
            rightArm.localRotation = Quaternion.Slerp(startR, end, t);
            yield return null;
        }
    }

    // ======================
    // 補助
    // ======================
    void ResetLimbsImmediate()
    {
        leftLeg.localRotation = Quaternion.identity;
        rightLeg.localRotation = Quaternion.identity;
        leftArm.localRotation = Quaternion.identity;
        rightArm.localRotation = Quaternion.identity;
    }
}
