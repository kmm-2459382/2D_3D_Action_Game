using UnityEngine;

namespace TheClimb.Item
{
    public class InpactBallAttractingState : ItemStateBase    //  引き寄せられてる時のState
    {
        public override void OnEnter()    //  引き付け状態遷移時の処理
        {
            _context.NotityCountStart();
        }
    }
}