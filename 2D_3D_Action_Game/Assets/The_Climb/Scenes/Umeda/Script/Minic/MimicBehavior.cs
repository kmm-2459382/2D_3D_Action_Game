using UnityEngine;

public class MimicBehavior : MonoBehaviour
{
    public float pushForce = 15f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 pushDir = (other.transform.position - transform.position).normalized;
                rb.AddForce(pushDir * pushForce, ForceMode.Impulse);
            }

            Destroy(gameObject);
        }
    }
}
