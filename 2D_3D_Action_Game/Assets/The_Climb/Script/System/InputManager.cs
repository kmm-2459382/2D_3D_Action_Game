using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private InputActionReference leftMoveAction;   //左移動ボタン/A
    [SerializeField] private InputActionReference rightMoveAction;  //右移動ボタン/D
    [SerializeField] private InputActionReference jumpAction;       //ジャンプボタン/Space
    [SerializeField] private InputActionReference downAction;       //下ボタン/S

    //GetKey 相当
    public bool leftHeld => leftMoveAction.action.IsPressed();
    public bool rightHeld => rightMoveAction.action.IsPressed();
    public bool jumpHeld => jumpAction.action.IsPressed(); 
    public bool downHeld => downAction.action.IsPressed();

    //GetKeyDown 相当
    public bool jumpDown => jumpAction.action.WasPerformedThisFrame();

    //GetKeyUp 相当
    public bool jumpUp => jumpAction.action.WasReleasedThisFrame();

    private void OnEnable() 
    { 
        leftMoveAction.action.Enable();
        rightMoveAction.action.Enable();
        jumpAction.action.Enable();
        downAction.action.Enable();
    }

    private void OnDisable() 
    {
        leftMoveAction.action.Disable();
        rightMoveAction.action.Disable();
        jumpAction.action.Disable();
        downAction.action.Disable();
    }
}
