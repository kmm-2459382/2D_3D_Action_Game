using UnityEngine;

namespace TheClimb.Astral
{
    public class AttractTargetEntry
    {
        public AttractTargetMarker target;    //  マーカーコンポーネント
        public Rigidbody rigidbody;

        public AttractTargetEntry(AttractTargetMarker target, Rigidbody rigidbody)
        {
            this.target = target;
            this.rigidbody = rigidbody;
        }
    }
}