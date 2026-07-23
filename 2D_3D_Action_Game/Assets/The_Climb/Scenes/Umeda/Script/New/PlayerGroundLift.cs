using UnityEngine;

public class PlayerGroundLift : MonoBehaviour
{
    [SerializeField] private float liftAmount = 0.02f; // 浮かせる高さ
    [SerializeField] private LayerMask groundLayer;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionStay(Collision collision)
    {
        // Ground レイヤーとの接触を確認
        if ((groundLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            // 上方向の法線を含む接触なら
            foreach (ContactPoint contact in collision.contacts)
            {
                if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
                {
                    // 少しだけ上に補正
                    Vector3 pos = transform.position;
                    pos.y += liftAmount;
                    rb.MovePosition(pos);
                    break;
                }
            }
        }
    }
}
