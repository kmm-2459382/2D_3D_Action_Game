namespace TheClimb.Astral
{
    public class PlanetStateMachine
    {

        //  --  Public API

        public IPlanetState CurrentPlanetState { get; private set; }
        
        public void ChangeState(IPlanetState newState)
        {
            CurrentPlanetState?.Exit();
            CurrentPlanetState = newState;
            CurrentPlanetState?.Enter();
        }

        public void Update()
        {
            CurrentPlanetState?.Update();
        }
    }
}