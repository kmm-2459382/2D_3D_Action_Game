using UnityEngine;

namespace TheClimb.Astral
{
    public abstract class PlanetCommandBaseCtx     //  天体のコマンドパターンのBaseCtx
    {
        public Transform planetTransform { get; protected set; }
        public OrbitalStatusBlock orbitalStatusBlock { get; protected set; }    //  円軌道のステータスブロック

        protected PlanetCommandBaseCtx(Transform planetTF, OrbitalStatusBlock orbitalStatus)
        {
            planetTransform = planetTF;
            orbitalStatusBlock = orbitalStatus;
        }
    }
}