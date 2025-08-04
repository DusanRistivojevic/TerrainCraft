using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace TerrainCraft
{
    public class MeshVertsRound : MeshVerts
    {

        public new static void SetMiddlePoints(List<Vector3> points, int value)
        {

            MeshVerts.SetMiddlePoints(points, value);

            if (GetCenter(value) != -Vector3.one)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    if (!Calc.IsDefaultPoint(points[i]))
                    {
                        float y = points[i].y;
                        Vector3 v = GetCenter(value) + GetDirection(value, points[i]) * 0.5f;
                        v.y = y;
                        points[i] = v;
                    }
                }
            }
        }

    }
}
