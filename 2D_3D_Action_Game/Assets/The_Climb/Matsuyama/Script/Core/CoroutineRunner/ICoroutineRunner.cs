using System.Collections;
using UnityEngine;

namespace TheClimb.Core
{
    public interface ICorutineRunner    //  コルーチンランナー
    {
        Coroutine StartCoroutine(IEnumerator routine);    //  コルーチン開始
        void StopCoroutine(Coroutine coroutine);          //  コルーチン停止
    }
}