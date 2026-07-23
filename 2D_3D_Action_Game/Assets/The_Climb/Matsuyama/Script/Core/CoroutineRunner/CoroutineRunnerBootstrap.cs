using UnityEngine;

namespace TheClimb.Core
{
    public class CoroutineRunnerBootstrap : MonoBehaviour    //  コルーチンランナーを初期化してServiceLocatorに登録する
    {
        [SerializeField] CoroutineRunner coroutineRunner;    //  コルーチン実行役
        ICoroutineRunnerFacade coroutineRunnerFacade;

        private void Awake()
        {
            coroutineRunnerFacade = new CoroutineRunnerFacade(coroutineRunner) as ICoroutineRunnerFacade;
            ServiceLocator.Register<ICoroutineRunnerFacade>(coroutineRunnerFacade);
        }
    }
}