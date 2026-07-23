using UnityEngine;
using UnityEngine.InputSystem;

public class InputManagerUmeda : MonoBehaviour
{
    [Header("移動アクション")]
    [SerializeField] private InputActionReference leftMoveAction;      // 左移動ボタン/A
    [SerializeField] private InputActionReference rightMoveAction;     // 右移動ボタン/D
    [SerializeField] private InputActionReference forwardMoveAction;   // 前移動ボタン/W
    [SerializeField] private InputActionReference backMoveAction;      // 後移動ボタン/S

    [Header("ジャンプアクション")]
    [SerializeField] private InputActionReference jumpAction;          // ジャンプボタン/Space

    // X軸方向
    public bool leftHeld => leftMoveAction.action.IsPressed();
    public bool rightHeld => rightMoveAction.action.IsPressed();

    // Z軸方向
    public bool forwardHeld => forwardMoveAction != null && forwardMoveAction.action.IsPressed();
    public bool backHeld => backMoveAction != null && backMoveAction.action.IsPressed();

    // ジャンプ
    public bool jumpHeld => jumpAction.action.IsPressed();
    public bool jumpDown => jumpAction.action.WasPerformedThisFrame();
    public bool jumpUp => jumpAction.action.WasReleasedThisFrame();

    private void OnEnable()
    {
        leftMoveAction.action.Enable();
        rightMoveAction.action.Enable();
        jumpAction.action.Enable();
        if (forwardMoveAction != null) forwardMoveAction.action.Enable();
        if (backMoveAction != null) backMoveAction.action.Enable();
    }

    private void OnDisable()
    {
        leftMoveAction.action.Disable();
        rightMoveAction.action.Disable();
        jumpAction.action.Disable();
        if (forwardMoveAction != null) forwardMoveAction.action.Disable();
        if (backMoveAction != null) backMoveAction.action.Disable();
    }
}
