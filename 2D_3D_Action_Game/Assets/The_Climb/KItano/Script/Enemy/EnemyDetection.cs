using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    [Header("検知設定")]
    [SerializeField] private float viewDistance = 10f;
    [SerializeField] private float viewAngle = 60f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Transform eyePoint;

    private Transform player;

    public bool CanSeePlayer { get; private set; }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        DetectPlayer();

    }

    void DetectPlayer()
    {
        CanSeePlayer = false;

        Vector3 dirToPlayer = player.position - eyePoint.position;
        float distance = dirToPlayer.magnitude;

        if (distance > viewDistance) return;

        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        if (angle > viewAngle * 0.5f) return;

        if (Physics.Raycast(eyePoint.position, dirToPlayer.normalized, out RaycastHit hit, viewDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                CanSeePlayer = true;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (eyePoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(eyePoint.position, viewDistance);
    }
}