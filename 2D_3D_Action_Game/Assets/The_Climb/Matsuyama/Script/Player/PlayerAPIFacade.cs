using TheClimb.Player;
using UnityEngine;

namespace TheClimb.Core
{
    public class PlayerAPIFacade : PlayerAPIFacadeBase    //  プレイヤーAPIを使うためのFacadaクラス(ServiceLocatorに登録)
    {
        public PlayerAPIFacade(PlayerPhysics physics, PlayerPhysicsJudge judge) : base(physics, judge)
        { }

        public override void AddForce(Vector3 force, AddForceMode mode)
        {
            if (judge.Judge())
            {
                playerPhysics.AddForce(force, mode);
            }
        }
    }
}