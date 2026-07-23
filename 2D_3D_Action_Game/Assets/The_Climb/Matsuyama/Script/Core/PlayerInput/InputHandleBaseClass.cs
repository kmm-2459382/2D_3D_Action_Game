using TheClimb.Astral;
using UnityEngine;

namespace TheClimb.Core
{
    public abstract class InputHandleBase : MonoBehaviour    //  入力を受け取るクラスの差し替えや共通処理吸い上げ用クラス
    {
       public abstract void Initialize(InputSystem_Actions inputSystem, PlanetAbilityBase planetAbility);
    }
}