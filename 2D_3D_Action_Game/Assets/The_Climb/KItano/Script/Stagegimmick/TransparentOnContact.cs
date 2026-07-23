using UnityEngine;

public class TransparentOnContact : MonoBehaviour
{
    private Renderer renderer;

    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // 接触時に透明化
        Color color = renderer.material.color;
        color.a = 0;
        renderer.material.color = color;
    }
}
