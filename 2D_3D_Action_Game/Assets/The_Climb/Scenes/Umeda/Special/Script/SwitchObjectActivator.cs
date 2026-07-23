using UnityEngine;

public class SwitchObjectActivator : MonoBehaviour
{
    [Header("参照するスイッチ（子でもOK）")]
    public Switch targetSwitch;

    [Header("無効化するオブジェクト")]
    public GameObject disableTarget;

    [Header("有効化するオブジェクト")]
    public GameObject enableTarget;

    private bool executed = false;

    void Update()
    {
        if (executed) return;
        if (targetSwitch == null) return;

        if (targetSwitch.IsPressed)
        {
            Execute();
        }
    }

    void Execute()
    {
        executed = true;

        if (disableTarget != null)
            disableTarget.SetActive(false);

        if (enableTarget != null)
            enableTarget.SetActive(true);
    }
}
