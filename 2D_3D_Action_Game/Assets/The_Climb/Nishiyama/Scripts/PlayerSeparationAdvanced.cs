using UnityEngine;

public class PlayerSeparationAdvanced : MonoBehaviour
{
    [SerializeField] private Transform otherPlayer;
    [SerializeField] private float minDistance = 0.6f;
    [SerializeField] private float pushSpeed = 3f;
    [SerializeField] private LayerMask wallLayer; // 壁のレイヤー指定

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (otherPlayer == null) return;

        Vector3 diff = transform.position - otherPlayer.position;
        diff.y = 0f;
        float distance = diff.magnitude;

        if (distance < minDistance)
        {
            Vector3 pushDir = diff.normalized;

            //  壁があるかをチェック（壁側には押さない）
            bool wallAhead = Physics.Raycast(transform.position, pushDir, 0.4f, wallLayer);
            if (!wallAhead)
            {
                // 壁がなければ押し戻す
                Vector3 newPos = transform.position + pushDir * (minDistance - distance) * Time.fixedDeltaTime * pushSpeed;
                rb.MovePosition(newPos);
            }
        }
    }

}
