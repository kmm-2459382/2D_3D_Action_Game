using System.Collections;
using UnityEngine;

namespace TheClimb.Core
{
    public class CoroutineRunner : MonoBehaviour    //  コルーチンランナー
    {
        public Coroutine StartCoroutine(IEnumerator routine) => base.StartCoroutine(routine);    //  コルーチン開始
        public void StopCoroutine(Coroutine coroutine) => base.StopCoroutine(coroutine);         //  コルーチン停止
        void Awake()
        {
        }
    }
}