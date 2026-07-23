using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimationController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        float speed = agent.velocity.magnitude;
       // Debug.Log(speed);

        animator.SetFloat("Speed", speed);
        animator.SetBool("IsMoving", speed > 0.1f);
    }
}