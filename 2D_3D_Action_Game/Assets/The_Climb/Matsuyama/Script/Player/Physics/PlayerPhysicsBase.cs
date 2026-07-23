using TheClimb.Core;
using UnityEngine;

namespace TheClimb.Player
{
    public abstract class PlayerPhysicsBase    //  イベント時に差し替えるためのBaseClass
    {
        protected Rigidbody rigidbody;

        protected PlayerPhysicsBase(Rigidbody rb)
        {
            rigidbody = rb;
        }

        public abstract void AddForce(Vector3 force, AddForceMode mode);
    }
}