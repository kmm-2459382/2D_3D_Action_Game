using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public Boss_20_StatusObjectScript status;
    private float speed;
    public float lifeTime = 5f;
    public float hitRadious = 5.0f;
    private Transform playerTransform;
    private Vector3 moveDirection;
    private bool initialized = false;

    public void Initialize(Transform player)
    {
        playerTransform = player;
        speed = status.Attack_Speed;
        moveDirection = (player.position - transform.position).normalized;
        initialized = true;

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (!initialized) return;
        transform.position += moveDirection * speed * Time.deltaTime;
        if(playerTransform != null && Vector3.Distance(transform.position, playerTransform.position) < hitRadious)
        {
            Destroy(gameObject);
        }
    }
}