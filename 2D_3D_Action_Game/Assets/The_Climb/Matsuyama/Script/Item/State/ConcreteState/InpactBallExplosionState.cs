namespace TheClimb.Item
{
    public class InpactBallExplosionState : ItemStateBase    //  爆発中のState
    {
        public override void OnEnter()    //  爆発開始時の処理
        {
            _context.NotityExplosionStart();
        }
    }
}