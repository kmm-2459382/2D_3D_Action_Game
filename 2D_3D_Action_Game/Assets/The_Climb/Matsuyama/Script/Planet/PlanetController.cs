using UnityEngine;
using TheClimb.Core;
using TheClimb.Player;

namespace TheClimb.Astral
{
    [RequireComponent(typeof(PlanetBootstrap))]
    [DisallowMultipleComponent]
    public class PlanetController : MonoBehaviour    //  天体を包括的にコントロールする
    {
        [SerializeField] PlanetStatus planetStatus;           //  天体のステータス
        GravitationStatusBlock currentGravitationStat;        //  天体の万有引力ステータスブロック
        OrbitalStatusBlock currentOrbitalStat;                //  天体の円軌道追従ステータスブロック
        PlanetStateMachine planetStateMachine;                //  天体のステートマシーン
        PlanetStateFactory planetStateFactory;                //  天体ステートファクトリー
        PlanetCommandProvider planetCommandProvider;          //  天体関数提供クラス
        OrbitalContext orbitalContext;                        //  天体円軌道コンテキスト

        void Awake()
        {
            PlanetContext.Instance.RegistPlanetController(this);
            currentGravitationStat = planetStatus.GetGraviatationStatus(PlanetIDs.Earth);
            currentOrbitalStat = planetStatus.GetOrbitalStatus(PlanetIDs.Earth);
        }

        private void Start()
        {
        }
        public void Initialize(IPlayerDataProvider playerDataProvider, StageClear stageClear)    //  初期化
        {
            planetStateMachine = new PlanetStateMachine();
            orbitalContext = new OrbitalContext(this.transform, currentOrbitalStat, playerDataProvider.TransformProperty, stageClear);
            planetCommandProvider = new PlanetCommandProvider(this.transform, playerDataProvider.TransformProperty, currentGravitationStat, currentOrbitalStat, orbitalContext);
            planetStateFactory = new PlanetStateFactory(this, planetStateMachine, planetCommandProvider);
            PlanetEventBus.ActivePlanet(playerDataProvider.TransformProperty, currentOrbitalStat.OrbitRadius, currentOrbitalStat.OrbitalSamples);    //  半円を表示

            planetStateMachine.ChangeState(planetStateFactory.CreateIdleState());
        }

        void Update()
        {
            planetStateMachine.Update();
        }

        public void StopPlanet()
        {
            planetStateMachine.ChangeState(planetStateFactory.CreateStopState());
        }
    }
}