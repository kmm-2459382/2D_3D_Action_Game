using UnityEngine;

public class PlayerMarker : MonoBehaviour
{
    private float baseY;

    void Start() => baseY = transform.position.y;

    void Update()
    {
        float offset = Mathf.Sin(Time.time * 3f) * 0.1f;
        transform.position = new Vector3(transform.position.x, baseY + offset, transform.position.z);
    }
}
