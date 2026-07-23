using System.Collections;
using UnityEngine;

namespace TheClimb.Core
{
    public class CoroutineRunnerFacade : ICoroutineRunnerFacade    //  SLに登録して呼ばれるCorouineRunnerFacadeの実体
    {
        CoroutineRunner coroutineRunner;

        //  --  Public API

        public CoroutineRunnerFacade(CoroutineRunner coroutineRunner)
        {
            this.coroutineRunner = coroutineRunner;
        }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return coroutineRunner.StartCoroutine(routine);
        }
        public void StopCoroutine(Coroutine routine)
        {
            coroutineRunner.StopCoroutine(routine);
        }
    }
}