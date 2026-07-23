using UnityEngine;

public class Trampoline : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerJump jump = other.GetComponent<PlayerJump>();
            if (jump != null)
            {
                jump.isOnTrampoline = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerJump jump = other.GetComponent<PlayerJump>();
            if (jump != null)
            {
                jump.isOnTrampoline = false;
            }
        }
    }
}