using UnityEngine;

public class HideOnPlayerContact : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            meshRenderer.enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            meshRenderer.enabled = true;
        }
    }
}