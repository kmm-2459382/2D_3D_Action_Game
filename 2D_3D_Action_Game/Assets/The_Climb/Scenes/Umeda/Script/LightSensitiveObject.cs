using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Collider))]
public class LightSensitiveObject : MonoBehaviour
{
    private Renderer rend;
    private int lightCount = 0;

    void Start()
    {
        rend = GetComponent<Renderer>();
        GetComponent<Collider>().isTrigger = false; // é©ìÆê›íË
        gameObject.layer = LayerMask.NameToLayer("Default"); // é©ìÆê›íË
        rend.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Light")
        {
            lightCount++;
            rend.enabled = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Light")
        {
            lightCount = Mathf.Max(0, lightCount - 1);
            if (lightCount == 0) rend.enabled = false;
        }
    }
}
