using UnityEngine;
using UnityEngine.InputSystem;

public class MirrorMove : MonoBehaviour
{
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private InputActionReference moveAction; // 一つのキー入力を使う

    private void OnEnable()
    {
        if (moveAction != null)
            moveAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null)
            moveAction.action.Disable();
    }

    private void Update()
    {
        if (moveAction == null) return; 

        float input = moveAction.action.ReadValue<float>(); 

        if (input != 0)
        {
            // Player1は通常方向
            if (player1 != null)
                player1.transform.Translate(Vector3.right * input * moveSpeed * Time.deltaTime);

            // Player2は反転方向
            if (player2 != null)
                player2.transform.Translate(Vector3.right * -input * moveSpeed * Time.deltaTime);
        }
    }
}
