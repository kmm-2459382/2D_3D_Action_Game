using System.Collections.Generic;
using UnityEngine;
using TheClimb.Core;

namespace TheClimb.Astral
{
    [RequireComponent(typeof(LineRenderer))]
    public class CircleVisualizer : MonoBehaviour
    {
        LineRenderer lineRenderer;
        List<Vector3> localCircle;

        void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.loop = true;
            lineRenderer.useWorldSpace = true;
            lineRenderer.widthMultiplier = 0.05f;
        }

        void OnEnable()
        {
            PlanetEventBus.OnActivatePlanet += ShowCircle;
            PlanetEventBus.OnDeactivatePlanet += HideCircle;
        }

        void OnDisable()
        {
            PlanetEventBus.OnActivatePlanet -= ShowCircle;
            PlanetEventBus.OnDeactivatePlanet -= HideCircle;
        }

        public void ShowCircle(Transform target, float radius, int segments = 64)    //  円を描画する
        {
            if (target == null) return;
            if (localCircle == null || localCircle.Count != segments)
            {
                localCircle = CircleMath.SampleCircleXY(Vector3.zero, radius, segments);
                lineRenderer.positionCount = segments;
            }

            Matrix4x4 m = target.localToWorldMatrix;
            for (int i = 0; i < segments; i++)
            {
                lineRenderer.SetPosition(i, m.MultiplyPoint3x4(localCircle[i]));
            }

            lineRenderer.enabled = true;
        }

        public void HideCircle()    //  円の描画を止める
        {
            lineRenderer.enabled = false;
        }
    }
}