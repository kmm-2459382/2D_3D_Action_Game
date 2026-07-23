using System.Collections;
using TheClimb.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TheClimb.Astral
{
    public abstract class PlanetAbilityBase   //  差し替えするためと、コンテキスト共通処理吸い上げ用
    {
        protected PlanetAbilityStatsBase abilityStats;    //  天体の能力処理に必要な値が入ったSO

        protected Coroutine chargeCoroutine;
        protected bool isChargeComplete;
        protected Transform planetTF;
        protected Transform playerTF;

        protected Vector3 vectorToPlanet;

        protected PlanetAbilityBase(PlanetAbilityStatsBase Stats, Transform planetTF, Transform playerTF)
        {
            abilityStats = Stats;
            this.planetTF = planetTF;
            this.playerTF = playerTF;
        }

        public abstract void ChargeAbility(InputAction.CallbackContext context);
        public abstract void BurstChargeForce(InputAction.CallbackContext context);
    }
}