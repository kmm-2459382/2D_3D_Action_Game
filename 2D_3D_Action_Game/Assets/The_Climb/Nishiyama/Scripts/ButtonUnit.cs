using UnityEngine;

public class ButtonUnit : MonoBehaviour
{
    public bool isPressed { get; private set; } = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isPressed = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isPressed = false;
    }
}
