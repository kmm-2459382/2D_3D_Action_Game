using UnityEngine;

public class RespawnGroundCheck : MonoBehaviour
{
    public bool isRespawnGrounded = false; //リスポーンする地点に適しているか判定

    private int groundLayer;

    void Start()
    {
        //Ground レイヤー番号を取得
        groundLayer = LayerMask.NameToLayer("Ground");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == groundLayer)
        {
            isRespawnGrounded = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == groundLayer)
        {
            isRespawnGrounded = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == groundLayer)
        {
            isRespawnGrounded = false;
        }
    }
}
