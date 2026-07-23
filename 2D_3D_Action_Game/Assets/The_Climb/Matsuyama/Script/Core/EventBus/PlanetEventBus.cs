using System;
using UnityEngine;

namespace TheClimb.Core
{
    public class PlanetEventBus    //  天体のイベントバス
    {
        public static event Action<Transform, float, int>  OnActivatePlanet;      //  天体が有効になった時
        public static event Action  OnDeactivatePlanet;                           //  天体が無効になった時
        
        public static void ActivePlanet(Transform target, float Radius, int Segments = 64)    //  キャッチkeyが押された時
        {
            OnActivatePlanet?.Invoke(target, Radius, Segments);
        }

        public static void Clear()
        {
            OnActivatePlanet = null;
            OnDeactivatePlanet = null;
        }
    }
}