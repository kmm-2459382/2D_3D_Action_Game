using UnityEngine;
using System.Diagnostics;

public class watch : MonoBehaviour
{
    Vector3 lastRot;

    void Start()
    {
        lastRot = transform.eulerAngles;
    }

    void LateUpdate()
    {
        if (transform.eulerAngles != lastRot)
        {
            StackTrace stack = new StackTrace(true);

            UnityEngine.Debug.Log(
                "Rotation Changed : " + transform.eulerAngles +
                "\nFrame : " + Time.frameCount +
                "\nStackTrace : \n" + stack
            );

            lastRot = transform.eulerAngles;
        }
    }
}