using TheClimb.Astral;
using UnityEngine;

namespace TheClimb.Core
{
    public class InputSystemBootstrap : MonoBehaviour    //  インプットシステムの初期化用クラス
    {
        InputSystem_Actions inputAction_System;

        [Tooltip("プレイヤーの入力受付スクリプト")]
        [SerializeField] InputHandleBase playerInputHandle;

        [Tooltip("天体の能力処理に必要なScriptableObject")]
        [SerializeField] PlanetAbilityStatsBase abilityStats;
        [SerializeField] Transform planetTF;
        [SerializeField] Transform playerTF;

        PlanetAbilityBase planetAbilityBase;

        [SerializeField] AudioSource audioSource;
        
        //  --  Unity life Cycle

        void Awake()
        {
            inputAction_System = new InputSystem_Actions();
            planetAbilityBase = new PlanetAbility(abilityStats, planetTF, playerTF, audioSource) as PlanetAbilityBase;
        }
        void Start()
        {
            playerInputHandle.Initialize(inputAction_System, planetAbilityBase);
        }
    }
}