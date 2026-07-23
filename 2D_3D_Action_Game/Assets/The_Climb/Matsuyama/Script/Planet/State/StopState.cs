namespace TheClimb.Astral
{
    public class StopState : IPlanetState
    {
        PlanetCommandProvider commandProvider;

        public StopState(PlanetCommandProvider cmdProvider)
        {
            commandProvider = cmdProvider;
        }

        //  --  Public method

        public void Enter()
        { }
        public void Update()
        { }

        public void Exit()
        { }
    }
}