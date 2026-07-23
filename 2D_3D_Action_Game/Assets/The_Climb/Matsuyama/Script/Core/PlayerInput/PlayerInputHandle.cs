using TheClimb.Astral;

namespace TheClimb.Core
{
    public class PlayerInputHandle : InputHandleBase    //  プレイヤーの入力を受けつけるクラス
    {
        InputSystem_Actions inputSystem_Action;
        PlanetAbilityBase planetAbility;    //  天体の能力関数が詰まってるクラス

        public override void Initialize(InputSystem_Actions inputSystem, PlanetAbilityBase planetAbility)
        {
            inputSystem_Action = inputSystem;
            this.planetAbility = planetAbility;

            SetReaction();
        }

        private void Awake()
        {
        }

        void OnDestroy()
        {
            SetOffReaction();
        }

        void SetReaction()
        {
            inputSystem_Action.Player.Enable();
            inputSystem_Action.Player.AstralAbility.started += planetAbility.ChargeAbility;
            inputSystem_Action.Player.AstralAbility.canceled += planetAbility.BurstChargeForce;
        }

        void SetOffReaction()
        {
            inputSystem_Action.Player.AstralAbility.started -= planetAbility.ChargeAbility;
            inputSystem_Action.Player.AstralAbility.canceled -= planetAbility.BurstChargeForce;
            inputSystem_Action.Player.Disable();
        }
    }
}