using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class MyPlayerInput : MonoBehaviour
{
    [Header("Input Values")]
    public Vector2 Move;
    public Vector2 Look;
    public bool Jump;
    public bool Sprint;

    [Header("Mouse Settings")]
    public bool LockCursor = true;

    // --- New Input System からのメッセージ受け取り ---
#if ENABLE_INPUT_SYSTEM
    // プレイヤーが移動キー(WASD等)を押したときに実行される
    public void OnMove(InputValue value)
    {
        Move = value.Get<Vector2>();
    }

    // プレイヤーがマウスやスティックを動かしたときに実行される
    public void OnLook(InputValue value)
    {
        Look = value.Get<Vector2>();
    }

    // プレイヤーがジャンプキー(Space等)を押した/離したときに実行される
    public void OnJump(InputValue value)
    {
        Jump = value.isPressed;
    }

    // プレイヤーがスプリントキー(Shift等)を押した/離したときに実行される
    public void OnSprint(InputValue value)
    {
        Sprint = value.isPressed;
    }
#endif

    // --- マウスカーソルの自動制御 ---
    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(LockCursor);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}