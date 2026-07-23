using UnityEngine;

public class GunShoot : MonoBehaviour
{
    [SerializeField] GameObject sphere;
    private float speed = 300;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject ball = Instantiate(sphere, transform.position, Quaternion.identity);
            Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
            ballRigidbody.AddForce(transform.right * speed);
        }
    }
}
