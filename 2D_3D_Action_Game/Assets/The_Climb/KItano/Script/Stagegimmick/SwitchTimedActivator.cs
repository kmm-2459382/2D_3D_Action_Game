using UnityEngine;
using System.Collections;

public class SwitchTimedActivator : MonoBehaviour
{
    [Header("参照するスイッチ")]
    public Switch targetSwitch;

    [Header("有効化するオブジェクト")]
    public GameObject targetObject;

    [Header("有効時間（秒）")]
    public float activeTime = 3.0f;

    private bool isRunning = false;

    void Start()
    {
        if (targetObject != null)
            targetObject.SetActive(false);
    }

    void Update()
    {
        if (targetSwitch == null) return;

        if (targetSwitch.IsPressed && !isRunning)
        {
            StartCoroutine(ActivateRoutine());
        }
    }

    IEnumerator ActivateRoutine()
    {
        isRunning = true;

        if (targetObject != null)
            targetObject.SetActive(true);

        yield return new WaitForSeconds(activeTime);

        if (targetObject != null)
            targetObject.SetActive(false);

        targetSwitch.ForceReset();

        isRunning = false;
    }
}
