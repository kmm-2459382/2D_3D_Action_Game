using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class LightRange : MonoBehaviour
{
    private new Light light;
    private SphereCollider rangeCollider;

    void Awake()
    {
        light = GetComponent<Light>();
        rangeCollider = GetComponent<SphereCollider>();
        rangeCollider.isTrigger = true;
    }

    void Update()
    {
        if (light != null && light.type == LightType.Point)
        {
            rangeCollider.radius = light.range;
        }
    }

    // Light側に付けるテスト用
    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Light entered: " + other.name);
    }
}