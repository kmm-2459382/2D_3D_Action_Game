using UnityEngine;

public class LaserKill : MonoBehaviour
{
    [Header("死亡判定するプレイヤー")]
    public GameObject player;

    [Header("レーザー移動設定（往復移動）")]
    public bool moveHorizontal = false;
    public bool moveVertical = false;
    public float speed = 2f;
    public float moveDistance = 3f;

    [Header("ランダム移動設定（中心に戻るタイプ）")]
    public bool moveRandom = false;
    public float randomMoveRadius = 3f;
    public float randomMoveSpeed = 2f;
    public float returnToCenterSpeed = 3f;
    public float randomMoveTime = 1.5f;
    public static System.Action OnPlayerDied;

    [Header("追い詰めレーザー（出現型）")]
    public bool pushLaser = false;
    public Vector3 moveDirection = Vector3.left; // 右→左
    public float pushSpeed = 3f;

    [Header("出現設定")]
    public bool hideOnStart = true;
    [Header("出現型レーザー専用")]
    public bool isSpawnPushLaser = false;
    [Header("レーザー種別")]
    public bool isPushLaser = false;
    [Header("このレーザー専用のトリガー")]
    public PushLaserTrigger myTrigger;
    [Header("追い詰めレーザー加速設定")]
    public bool enableAcceleration = true;
    public float startSpeed = 2f;        // 出現直後の速度
    public float maxSpeed = 6f;          // 最大速度
    public float acceleration = 0.5f;    // 1秒あたりの加速量


    [Header("リセットしたいボタン")]
    public ButtonGimmick buttonGimmick;


    private Vector3 startPos;

    // ランダム移動関連
    private Vector3 randomTargetPos;
    private float moveTimer;
    private bool isReturning = false;

    private PlayerRespawnUmeda playerRespawn; //  Respawnスクリプト参照保持

    private void Start()
    {
        startPos = transform.position;
        PickRandomTarget();

        if (isSpawnPushLaser)
        {
            gameObject.SetActive(false);
        }

        // プレイヤーから PlayerRespawnUmeda を取得
        if (player != null)
        {
            playerRespawn = player.GetComponent<PlayerRespawnUmeda>();
            if (playerRespawn == null)
            {
                Debug.LogWarning(" PlayerRespawnUmeda がプレイヤーに付いていません！");
            }
        }
    }

    private void Update()
    {
        if (pushLaser)
        {
            PushMove();
            return;
        }

        if (moveRandom)
            RandomMoveWithReturn();
        else
            MoveLaser();
    }



    private void MoveLaser()
    {
        Vector3 pos = startPos;

        if (moveHorizontal)
            pos.x += Mathf.Sin(Time.time * speed) * moveDistance;

        if (moveVertical)
            pos.y += Mathf.Sin(Time.time * speed) * moveDistance;

        transform.position = pos;
    }

    private void PushMove()
    {
        // 加速処理（迫ってくるレーザーのみ）
        if (enableAcceleration && isSpawnPushLaser)
        {
            pushSpeed += acceleration * Time.deltaTime;
            pushSpeed = Mathf.Min(pushSpeed, maxSpeed);
        }

        transform.position += moveDirection.normalized * pushSpeed * Time.deltaTime;
    }


    public void AppearAndStartPush()
    {
        if (!isSpawnPushLaser) return;

        Debug.Log("【PushLaser】出現＆加速開始");

        transform.position = startPos;
        gameObject.SetActive(true);

        pushSpeed = startSpeed;
        pushLaser = true;
    }



    private void RandomMoveWithReturn()
    {
        if (!isReturning)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                randomTargetPos,
                randomMoveSpeed * Time.deltaTime
            );

            moveTimer -= Time.deltaTime;

            if (moveTimer <= 0f ||
                Vector3.Distance(transform.position, randomTargetPos) < 0.1f)
            {
                isReturning = true;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                startPos,
                returnToCenterSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, startPos) < 0.1f)
            {
                isReturning = false;
                PickRandomTarget();
            }
        }
    }

    private void PickRandomTarget()
    {
        Vector2 dir = Random.insideUnitCircle.normalized;
        randomTargetPos = startPos + new Vector3(dir.x, dir.y, 0) * randomMoveRadius;
        moveTimer = randomMoveTime;
    }

    //  プレイヤーと当たったら Respawn() を呼ぶ
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player || other.transform.root.gameObject == player)
        {
            // バリアコンポーネントの取得（子オブジェクトも検索）
            var barrier = player.GetComponentInChildren<PlayerBarrier>();

            // バリアで防げるか試行
            if (barrier != null && barrier.TryBlockAttack())
            {
                return; // 防いだのでリスポーンしない
            }

            Debug.Log("レーザーに触れた → Respawn() を実行");

            if (playerRespawn != null)
            {
                playerRespawn.SendMessage("Respawn", SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                Debug.LogWarning(" PlayerRespawnUmeda がプレイヤーに付いていません！");
            }

            // ボタンを戻す処理
            if (buttonGimmick != null)
            {
                buttonGimmick.ForceResetButton();
            }


        }

        if (isPushLaser)
        {
            ResetPushLaser();

            if (myTrigger != null)
            {
                myTrigger.ResetTrigger();
            }
        }


    }

    public void ResetPushLaser()
    {
        pushLaser = false;
        pushSpeed = startSpeed;   // 速度リセット
        gameObject.SetActive(false);
        transform.position = startPos;
    }


}
