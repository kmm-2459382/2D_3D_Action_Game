using TheClimb.Logging;

namespace TheClimb.Item
{
    public class InpactBallIdleState : ItemStateBase     //  インパクトボールのIdleState
    {
        public override void OnEnter()
        {
            LogUtility.Log(LogPrefix.idleState, "iiuueeooaa", LogLevel.Warning);
        }
    }
}