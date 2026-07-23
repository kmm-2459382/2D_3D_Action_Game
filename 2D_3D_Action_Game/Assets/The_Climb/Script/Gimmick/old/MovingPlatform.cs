using UnityEngine;
using UnityEngine.UIElements;

public class MovingPlatform : MonoBehaviour
{
    public enum MoveDirection
    {
        Horizontal,  // 横方向
        Vertical,    // 縦方向
    }

    public enum StartDirection
    {
        Positive,  // 右or上から動く
        Negative,  // 左or下から動く
    }

    [Header("移動設定")]
    public MoveDirection Direction = MoveDirection.Horizontal;
    public StartDirection StartMoveDirection = StartDirection.Positive;
    public float MoveDistance = 3f;
    public float MoveSpeed = 2f;
    public float MovementInfluence = 0.5f;

    private Vector3 StartPosition;
    private Rigidbody Rigidbody;
    private Rigidbody PlayerRigidbody; //プレイヤーのRigidbodyを保持
    private float ElapsedTime = 0f;

    private Vector3 PreviousPosition;
    private bool IsPlayerOnTop = false;

    private void Awake()
    {
        StartPosition = transform.position;
        Rigidbody = GetComponent<Rigidbody>();
        PreviousPosition = transform.position;
    }

    void FixedUpdate()
    {
        ElapsedTime += Time.fixedDeltaTime;

        // 横移動ならx軸、縦移動ならy軸
        Vector3 moveAxis = (Direction == MoveDirection.Horizontal) ? Vector3.right : Vector3.up;

        // 方向がNegativeの場合、-1倍にして逆方向にする
        if (StartMoveDirection == StartDirection.Negative)
        {
            moveAxis *= -1f;
        }

        float offset = Mathf.PingPong(ElapsedTime * MoveSpeed, MoveDistance);
        Vector3 targetPos = StartPosition + moveAxis * offset;

        Vector3 delta = targetPos - PreviousPosition;

        //移動処理
        Rigidbody.MovePosition(targetPos);

        if(PlayerRigidbody != null && IsPlayerOnTop)
        {
            Vector3 adjustedDelta = delta * MovementInfluence;
            PlayerRigidbody.MovePosition(PlayerRigidbody.position + adjustedDelta);
        }

        //次フレームのために保存
        PreviousPosition = targetPos;
        IsPlayerOnTop = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            foreach(ContactPoint contact in collision.contacts)
            {
                //足場にのっているか
                if(Vector3.Dot(contact.normal, Vector3.up) > 0.5f || Vector3.Dot(contact.normal, Vector3.down) > 0.5f)
                {
                    Debug.Log("プレイヤーが動く足場の上に乗った");
                    PlayerRigidbody = collision.gameObject.GetComponent<Rigidbody>();
                    IsPlayerOnTop = true;
                    break;
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(PlayerRigidbody != null && PlayerRigidbody.gameObject == collision.gameObject)
            {
                PlayerRigidbody = null;
                IsPlayerOnTop = false;
            }
        }
    }
}
