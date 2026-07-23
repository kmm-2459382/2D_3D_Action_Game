using UnityEngine;
using System.Collections;

namespace TheClimb.Core
{
    public interface ICoroutineRunnerFacade    //  SLに登録されて呼ばれるFacadeClassのI。
    {
        Coroutine StartCoroutine(IEnumerator routine);
        void StopCoroutine(Coroutine coroutine);
    }
}