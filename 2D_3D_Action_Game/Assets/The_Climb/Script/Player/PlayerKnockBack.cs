using System.Collections;
using UnityEngine;

public class PlayerKnockBack : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerMove move;
    private PlayerJump jump;
    private PlayerMind mind;
    private BuddyCarry buddyCarry;
    [SerializeField] PlayerBarrier barrier;

    public bool knockBacking = false;  //ノックバック中フラグ
    public float knockBackPower = 7f;  //ノックバック中フラグ
    public bool coolTime = false;  //ノックバックのクールタイムONOFF
    private float coolDuration = 1.0f;  //ノックバックのクールタイム
    private float coolTimer = 0f;  //ノックバックのクールタイム計測

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        move = GetComponent<PlayerMove>();
        jump = GetComponent<PlayerJump>();
        mind = GetComponent<PlayerMind>();
        buddyCarry = GetComponent<BuddyCarry>();
        //barrier = this.transform.parent.Find("PlayerAbility").Find("Nakamura").GetComponent<PlayerBarrier>();
    }

    private void Update()
    {
        //ノックバックのクールタイム
        if (coolTime)
        {
            coolTimer += Time.deltaTime;

            if (coolTimer > coolDuration)
            {
                coolTime = false;
                coolTimer = 0f;
            }
        }
    }

    public void DoKnockBack(int direction)
    {
        PlayerActionReset();
        knockBacking = true;

        //横速度をリセット
        rb.linearVelocity = Vector3.zero;

        //横と上にノックバック
        Vector3 knockDir = new Vector3(direction * knockBackPower, 8f, 0f);
        rb.AddForce(knockDir, ForceMode.Impulse);

        //一定時間後に解除（例: 0.3秒）
        Invoke(nameof(EndKnockBack), 0.2f);
    }

    private void EndKnockBack()
    {
        knockBacking = false;
        coolTime = true;
    }

    private void PlayerActionReset()
    {
        move.moveInput = 0f;
        jump.jumping = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!coolTime)
        {
            if (collision.gameObject.tag == "Enemy" && !knockBacking)
            {
                if (!barrier.unlocking)
                {
                    if (barrier.barrierActive)
                    {
                        StartCoroutine(barrier.BarrierFinish());
                    }
                    else
                    {
                        //敵とプレイヤーの位置でノックバックの方向を決める
                        int dir = transform.position.x - collision.gameObject.transform.position.x <= 0 ? -1 : 1;
                        DoKnockBack(dir);  //ノックバック
                        mind.SanityDecreaseEvent(3);  //正気度減少
                    }
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!coolTime)
        {
            if (!knockBacking && other.gameObject.tag == "StalkerHand")
            {
                if (buddyCarry.buddyController.beingKidnapped && other.TryGetComponent(out KidnapBuddy stalker) && stalker.handController.isKidnapping)
                {
                    //Buddy解放
                    stalker.handController.ReleaseBuddy();
                }
            }
            else if (!coolTime)
            {
                if (other.gameObject.tag == "BossStalker" && !knockBacking)
                {
                    if (!barrier.unlocking && barrier.barrierActive)
                    {
                        StartCoroutine(barrier.BarrierFinish());
                    }
                    else
                    {
                        //敵とプレイヤーの位置でノックバックの方向を決める
                        int dir = transform.position.x - other.gameObject.transform.position.x <= 0 ? -1 : 1;
                        DoKnockBack(dir);  //ノックバック
                        mind.SanityDecreaseEvent(5);  //正気度減少
                    }
                }
            }
        }
    }
}
