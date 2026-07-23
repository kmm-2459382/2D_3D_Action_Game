namespace TheClimb.Astral
{
    public class IdleState : IPlanetState
    {
        PlanetCommandProvider commandProvider;

        public IdleState(PlanetCommandProvider cmdProvider)
        {
            commandProvider = cmdProvider;
        }

        //  --  Public method

        public void Enter()
        {
            commandProvider.followOrbital.Execute();    //  マウスに位置に対応した円軌道上のポジションに動くループ実行
        }
        public void Update()
        {
            commandProvider.rotationPlanet.Execute();    //  天体を自転させるループ実行
        }

        public void Exit()
        {
            commandProvider.followOrbital.Stop();
        }
    }
}