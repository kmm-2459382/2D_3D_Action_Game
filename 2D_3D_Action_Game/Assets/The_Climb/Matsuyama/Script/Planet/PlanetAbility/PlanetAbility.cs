using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TheClimb.Core;

namespace TheClimb.Astral
{
    public class PlanetAbility : PlanetAbilityBase   //  天体の能力コマンド
    {
        AudioSource audioSource;
        AudioClip clip;

        public PlanetAbility(PlanetAbilityStatsBase stats, Transform planetTF, Transform playerTF, AudioSource audioSource) : base(stats, planetTF, playerTF)
        {
            isChargeComplete = false;
            chargeCoroutine = null;
            this.audioSource = audioSource;
        }

        public override void ChargeAbility(InputAction.CallbackContext context)    //  能力チャージコルーチンを作動させる受け子関数
        {
            chargeCoroutine = ServiceLocator.Resolve<ICoroutineRunnerFacade>().StartCoroutine(ChargePower());
        }

        IEnumerator ChargePower()    //  能力チャージの挙動処理
        {
            float holdTime = 0f;

            while ((holdTime += Time.deltaTime) < abilityStats.PrimaryEffectSpawnTime)    //  チャージ開始エフェクト生成まで待機
            { yield return null; }
            EffectAPIWindow.Play(new EffectKey(GameMode.Astral, EffectKind.AwakePower), planetTF);

            while ((holdTime += Time.deltaTime) < abilityStats.SecondaryEffectSpawnTime)    //  二段階目のエフェクト生成まで待機
            { yield return null; }
            
            EffectAPIWindow.Play(new EffectKey(GameMode.Astral, EffectKind.ChargePower), planetTF);
            EffectAPIWindow.StopSudden(new EffectKey(GameMode.Astral, EffectKind.AwakePower));

            //ServiceLocator.Resolve<ICoroutineRunnerFacade>().StartCoroutine(PlayTrimmed(0f, 0.7f));
            while ((holdTime += Time.deltaTime) < abilityStats.ChargeCompleteTime)    //  二段階目のエフェクト生成まで待機
            { yield return null; }
            
            EffectAPIWindow.Stop(new EffectKey(GameMode.Astral, EffectKind.ChargePower));

            while ((holdTime += Time.deltaTime) < abilityStats.ChargeCompleteTime + abilityStats.ChargeEffectDelay)    //  二段階目のエフェクト生成まで待機
            { yield return null; }

            isChargeComplete = true;

            EffectAPIWindow.Play(new EffectKey(GameMode.Astral, EffectKind.HoldPower), planetTF);

        }

        public override void BurstChargeForce(InputAction.CallbackContext context)
        {
            if (!isChargeComplete)
            {
                ServiceLocator.Resolve<ICoroutineRunnerFacade>().StopCoroutine(chargeCoroutine);
                EffectAPIWindow.Stop(new EffectKey(GameMode.Astral, EffectKind.AwakePower));
            }
            else
            {
                //Vector3 vectorToPlanet = playerTF.transform.position - planetTF.transform.position;
                Vector3 vectorToPlanet = planetTF.transform.position - playerTF.transform.position;
                Vector3 direction = vectorToPlanet.normalized;
                Vector3 blowForce = direction * abilityStats.RepulsiveFouce;
                ServiceLocator.Resolve<PlayerAPIFacadeBase>().AddForce(blowForce, AddForceMode.VelocityChagne);
                EffectAPIWindow.Play(new EffectKey(GameMode.Astral, EffectKind.BurstPower), planetTF);
            }

            EffectAPIWindow.Stop(new EffectKey(GameMode.Astral, EffectKind.ChargePower));
            EffectAPIWindow.Stop(new EffectKey(GameMode.Astral, EffectKind.HoldPower));

            isChargeComplete = false;
        }

        //IEnumerator PlayTrimmed(float startTime, float endTime)
        //{
        //    audioSource.Play();
            
        //    while ((startTime += Time.deltaTime) < endTime)
        //    {
        //        source.volume = Mathf.Lerp(sta, 0f, time / duration);
        //        yield return null;
        //    }

        //    audioSource.Stop();
        //}
    }
}