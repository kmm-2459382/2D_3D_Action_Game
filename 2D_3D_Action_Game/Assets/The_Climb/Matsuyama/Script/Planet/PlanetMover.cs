using UnityEngine;

namespace TheClimb.Astral
{
    public class PlanetMover    //  “V‘Ì‚ğˆÚ“®‚³‚¹‚éŠÖ”‚ğŠ
    {
        void MovePlanet()    //  “V‘Ì‚ğˆÚ“®‚³‚¹‚é
        {

        }
        public void RotationPlanet(Transform planet, float RotationPerSecond)    //  “V‘Ì‚ğ‰ñ“]‚³‚¹‚é
        {
            planet.Rotate(Vector3.up * -RotationPerSecond * Time.deltaTime);
        }
    }
}