using UnityEngine;

namespace TheClimb.UniversalGravity
{
    public static class AttractVectleCaluculate    //  引き寄せベクトル計算
    {
        public static Vector3 CalculateAttractVectle
            (Vector3 planetPos, Vector3 targetPos, float PlanetMass, float TargetMass, float Dist)    //  引き寄せ方向計算
        {
            Vector3 dir = (planetPos - targetPos).normalized;
            float forceMag = (PlanetMass * PlanetMass) / (Dist * Dist);
            Vector3 acceleration = forceMag * dir / TargetMass;

            return acceleration;
        }
    }
}
