using UnityEngine;
using TheClimb.Core;

namespace TheClimb.Astral
{
    [DefaultExecutionOrder(-100)]
    public class PlanetContext : MonoBehaviour    //  天体コンテキスト
    {
        [SerializeField] Transform PlanetTransform;

        public static PlanetContext Instance { get; private set; }
        public PlanetController _PlanetController { get; private set; }

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

        public void RegistPlanetController(PlanetController planetController)    //  登録メソッド
        {
            _PlanetController = planetController;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                PlanetEventBus.Clear();
            }
        }
    }
}