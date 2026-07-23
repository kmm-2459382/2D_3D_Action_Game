using System.Collections;
using System.Xml.Serialization;
using UnityEngine;

//  コルーチンユーティリティ
public static class CoroutineUtility
{
    //  コルーチンを開始
    public static void SafeStartCoroutine(MonoBehaviour mono, ref Coroutine coroutineRef, IEnumerator Routine)
    {
        if(coroutineRef != null)
        {
            mono.StopCoroutine(coroutineRef);
        }

        coroutineRef = mono.StartCoroutine(Routine);
    }

    //  コルーチンを停止
    public static void SafeStopCoroutine(MonoBehaviour mono, ref Coroutine coroutineRef)
    {
        if(coroutineRef != null)
        {
            mono.StopCoroutine(coroutineRef);
            coroutineRef = null;
        }
    }
    //  一定時間待機してから指定のアクションを開始
    public static Coroutine Delay(MonoBehaviour mono, float delaySecongds, System.Action action)
    {
        return mono.StartCoroutine(DelayRoutine(delaySecongds, action));
    }
    //  指定時間待機関数
    private static IEnumerator DelayRoutine(float delaySecondes, System.Action action)
    {
        yield return new WaitForSeconds(delaySecondes);
        action.Invoke();
    }
}
