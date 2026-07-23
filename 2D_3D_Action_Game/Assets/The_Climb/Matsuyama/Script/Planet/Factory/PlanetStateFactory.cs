namespace TheClimb.Astral
{
    public class PlanetStateFactory    //  天体のStateを生成するファクトリー
    {
        readonly PlanetCommandProvider commandProvider;

        public PlanetStateFactory(PlanetController controller, PlanetStateMachine sm, PlanetCommandProvider cmdProvider)    //  Controllerから呼ばれる
        {
            commandProvider = cmdProvider;
        }

        public IPlanetState CreateIdleState()    //  IdleState生成
        {
            return new IdleState(commandProvider);
        }
        public IPlanetState CreateStopState()    //  IdleState生成
        {
            return new StopState(commandProvider);
        }

        //  ジャンプ状態生成    現在は未使用
        //public IEnemyState CreateJumpState()
        //{
        //    return new JumpState(_kickerMoveCommander, _enemyStateMachine, this);
        //}
    }
}