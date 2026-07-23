using UnityEngine;

namespace TheClimb.Core
{
    public class CoreContext : MonoBehaviour    //  Coreの参照を持つコンテキストクラス
    {
        public TimeManager timeManager;    //  タイムマネージャーインスタンス
        public static CoreContext Instance { get; private set; }              //  インスタンスプロパティ

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}