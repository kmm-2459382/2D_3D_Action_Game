using TheClimb.Player;
using UnityEngine;

namespace TheClimb.Core
{
    public class PlayerPhysics : PlayerPhysicsBase
    {
        public PlayerPhysics(Rigidbody rb) :  base(rb)
        { }

        public override void AddForce(Vector3 force, AddForceMode mode)    //  プレイヤーを吹き飛ばす
        {
            if (mode == AddForceMode.Force)
            {
                rigidbody.AddForce(force, ForceMode.Force);
            }
            else if(mode == AddForceMode.Acceleration)
            {
                rigidbody.AddForce(force, ForceMode.Acceleration);
            }
            else if (mode == AddForceMode.VelocityChagne)
            {
                rigidbody.AddForce(force, ForceMode.VelocityChange);
            }
            else if (mode == AddForceMode.Impalse)
            {
                rigidbody.AddForce(force, ForceMode.Impulse);
            }
        }
    }
}