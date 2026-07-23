using System;
using UnityEngine;

//  Notifier系から渡された関数を実行する
public abstract class CollisionNotifier<T> : MonoBehaviour where T : class
{
    protected T handler;    //  渡されたインターフェース型の変数

    protected virtual void Awake()
    {
        handler = GetComponent<T>();
        if (handler == null)
        {
            Debug.LogWarning($"{typeof(T).Name} がアタッチされていません", this);
        }
    }
    //  渡された関数を条件を見て実行する
    protected void NotifyIfTagMatches(Collision collision, string tag, Action<T> action)
    {
        if (collision.gameObject.CompareTag(tag) && handler != null)
        {
            action.Invoke(handler);
        }
    }
}