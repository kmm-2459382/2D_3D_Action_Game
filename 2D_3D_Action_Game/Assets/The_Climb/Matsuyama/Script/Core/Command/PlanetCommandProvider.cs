using UnityEngine;

namespace TheClimb.Astral
{
    public class PlanetCommandProvider    //  天体のコマンドプロバイダー
    {
        public FollowOrbital followOrbital { get;}
        public RotationPlanet rotationPlanet{ get;}

        public PlanetCommandProvider(Transform PlanetTF, Transform PlayerTF, GravitationStatusBlock gravitationStat, OrbitalStatusBlock orbitalStat, OrbitalContext orbitalCtx)
        {
            followOrbital = new FollowOrbital(orbitalCtx);
            rotationPlanet = new RotationPlanet(PlanetTF, gravitationStat);
        }
    }
}