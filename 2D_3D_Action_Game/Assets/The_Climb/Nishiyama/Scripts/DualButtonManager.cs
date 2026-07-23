using UnityEngine;
using System.Collections;

public class DualButtonManager : MonoBehaviour
{
    [Header("上プレイヤー用ボタンとドア")]
    [SerializeField] private ButtonUnit upperButton;
    [SerializeField] private Transform upperDoor;

    [Header("下プレイヤー用ボタンとドア")]
    [SerializeField] private ButtonUnit lowerButton;
    [SerializeField] private Transform lowerDoor;

    [Header("ドアの動き設定")]
    [SerializeField] private float slideHeight = 2f;
    [SerializeField] private float openSpeed = 2f;   // 開く速さ
    [SerializeField] private float closeSpeed = 2f;  // 閉じる速さ

    private bool doorOpen = false;
    private Vector3 upperClosedPos, upperOpenPos;
    private Vector3 lowerClosedPos, lowerOpenPos;
    private Coroutine doorRoutine;

    private void Start()
    {
        upperClosedPos = upperDoor.localPosition;
        upperOpenPos = upperClosedPos + new Vector3(0, slideHeight, 0);
        lowerClosedPos = lowerDoor.localPosition;
        lowerOpenPos = lowerClosedPos + new Vector3(0, slideHeight, 0);
    }

    private void Update()
    {
        // 両方のボタンが押されているかチェック
        if (upperButton.isPressed && lowerButton.isPressed)
        {
            if (!doorOpen)
            {
                if (doorRoutine != null) StopCoroutine(doorRoutine);
                doorRoutine = StartCoroutine(OpenDoors());
            }
        }
        else
        {
            if (doorOpen)
            {
                if (doorRoutine != null) StopCoroutine(doorRoutine);
                doorRoutine = StartCoroutine(CloseDoors());
            }
        }
    }

    private IEnumerator OpenDoors()
    {
        doorOpen = true;

        float t = 0f;
        Vector3 startUpper = upperDoor.localPosition;
        Vector3 startLower = lowerDoor.localPosition;

        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            upperDoor.localPosition = Vector3.Lerp(startUpper, upperOpenPos, t);
            lowerDoor.localPosition = Vector3.Lerp(startLower, lowerOpenPos, t);
            yield return null;
        }
    }

    private IEnumerator CloseDoors()
    {
        doorOpen = false;

        float t = 0f;
        Vector3 startUpper = upperDoor.localPosition;
        Vector3 startLower = lowerDoor.localPosition;

        while (t < 1f)
        {
            t += Time.deltaTime * closeSpeed;
            upperDoor.localPosition = Vector3.Lerp(startUpper, upperClosedPos, t);
            lowerDoor.localPosition = Vector3.Lerp(startLower, lowerClosedPos, t);
            yield return null;
        }
    }
}
