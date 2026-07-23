using System.Collections.Generic;
using UnityEngine;

namespace TheClimb.Core
{
    public static class CircleMath
    {
        public static List<Vector3> SampleCircleXY(Vector3 center, float radius, int samples)    //  XY面に円を生成する
        {
            var points = new List<Vector3>(samples);
            for (int i = 0; i < samples; i++)
            {
                float theta = (2f * Mathf.PI * i) / samples;
                float x = center.x + radius * Mathf.Cos(theta);
                float y = center.y + radius * Mathf.Sin(theta);
                points.Add(new Vector3(x, y, center.z));
            }
            return points;
        }
    }
}