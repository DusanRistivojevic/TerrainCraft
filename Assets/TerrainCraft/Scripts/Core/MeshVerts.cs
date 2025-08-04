using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace TerrainCraft
{
    public class MeshVerts
    {
        public static TerrainCraft terrain;
        public static List<Vector3> GetFieldPoints(TerrainCraft t, Field field, int fieldValue, int height, int minimum, bool isV)
        {
            terrain = t;
            List<Vector3> points = new List<Vector3>();
            float cliffHeight = terrain.CliffHeight;

            float heightValue = height * cliffHeight;
            float[] heights = new float[] { heightValue, heightValue, heightValue, heightValue };
            for (int i = 0; i < heights.Length; i++)
            {
                if (field.cliffValues[i] < height)
                {
                    heights[i] = field.cliffValues[i] * cliffHeight;
                }
            }
            if (field.HasRamp())
            {
                for (int i = 0; i < heights.Length; i++)
                {
                    if (field.HasRamp(i))
                    {
                        heights[i] = (Calc.MinimumInList(field.cliffValues, -10) + 0.5f) * cliffHeight;
                    }
                }

            }


            if (isV)
            {
                if (terrain.CliffMesh == null)
                    points.AddRange(GetFieldPointsVerticalDefault(fieldValue, heights, minimum));
                else
                    points.AddRange(GetFieldPointsVerticalFromMesh(fieldValue, heights, minimum));
            }
            else
                points.AddRange(GetFieldPointsHorizontalDefault(fieldValue, heights));


            return points;
        }
        public static List<Vector3> GetFieldPointsHorizontalDefault(int value, float[] heights)
        {
            List<Vector3> points = new List<Vector3>();
            switch (value)
            {
                case 1://0001
                    points.Add(new Vector3(0f, heights[0], 0f));
                    points.Add(new Vector3(0f, heights[0], 0.5f));
                    points.Add(new Vector3(0.5f, heights[0], 0f));
                    break;

                case 2://0010
                    points.Add(new Vector3(0f, heights[1], 1f));
                    points.Add(new Vector3(0.5f, heights[1], 1f));
                    points.Add(new Vector3(0f, heights[1], 0.5f));
                    break;

                case 3://0011
                    points.Add(new Vector3(0f, heights[0], 0f));
                    points.Add(new Vector3(0f, heights[1], 1f));
                    points.Add(new Vector3(0.5f, heights[1], 1f));
                    points.Add(new Vector3(0.5f, heights[0], 0f));
                    break;

                case 4://0100
                    points.Add(new Vector3(1f, heights[2], 1f));
                    points.Add(new Vector3(1f, heights[2], 0.5f));
                    points.Add(new Vector3(0.5f, heights[2], 1f));
                    break;

                case 5://0101
                    points.Add(new Vector3(1f, heights[2], 1f));
                    points.Add(new Vector3(1f, heights[2], 0.5f));
                    points.Add(new Vector3(0.5f, heights[0], 0f));
                    points.Add(new Vector3(0f, heights[0], 0f));
                    points.Add(new Vector3(0f, heights[0], 0.5f));
                    points.Add(new Vector3(0.5f, heights[2], 1f));
                    break;

                case 6://0110
                    points.Add(new Vector3(0f, heights[1], 1f));
                    points.Add(new Vector3(1f, heights[2], 1f));
                    points.Add(new Vector3(1f, heights[2], 0.5f));
                    points.Add(new Vector3(0f, heights[1], 0.5f));
                    break;

                case 7://0111
                    points.Add(new Vector3(0f, heights[1], 1f));
                    points.Add(new Vector3(1f, heights[2], 1f));
                    points.Add(new Vector3(1f, heights[2], 0.5f));
                    points.Add(new Vector3(0.5f, heights[0], 0f));
                    points.Add(new Vector3(0f, heights[0], 0f));
                    break;

                case 8://1000
                    points.Add(new Vector3(1f, heights[3], 0f));
                    points.Add(new Vector3(0.5f, heights[3], 0f));
                    points.Add(new Vector3(1f, heights[3], 0.5f));
                    break;

                case 9://1001
                    points.Add(new Vector3(1f, heights[3], 0f));
                    points.Add(new Vector3(0f, heights[0], 0f));
                    points.Add(new Vector3(0f, heights[0], 0.5f));
                    points.Add(new Vector3(1f, heights[3], 0.5f));
                    break;

                case 10://1010
                    points.Add(new Vector3(0f, heights[1], 1f));
                    points.Add(new Vector3(0.5f, heights[1], 1f));
                    points.Add(new Vector3(1f, heights[3], 0.5f));
                    points.Add(new Vector3(1f, heights[3], 0f));
                    points.Add(new Vector3(0.5f, heights[3], 0f));
                    points.Add(new Vector3(0f, heights[1], 0.5f));
                    break;

                case 11://1011
                    points.Add(new Vector3(0f, heights[0], 0f));
                    points.Add(new Vector3(0f, heights[1], 1f));
                    points.Add(new Vector3(0.5f, heights[1], 1f));
                    points.Add(new Vector3(1f, heights[3], 0.5f));
                    points.Add(new Vector3(1f, heights[3], 0f));
                    break;

                case 12://1100
                    points.Add(new Vector3(1f, heights[2], 1f));
                    points.Add(new Vector3(1f, heights[3], 0f));
                    points.Add(new Vector3(0.5f, heights[3], 0f));
                    points.Add(new Vector3(0.5f, heights[2], 1f));
                    break;

                case 13://1101
                    points.Add(new Vector3(1f, heights[3], 0f));
                    points.Add(new Vector3(0f, heights[0], 0f));
                    points.Add(new Vector3(0f, heights[0], 0.5f));
                    points.Add(new Vector3(0.5f, heights[2], 1f));
                    points.Add(new Vector3(1f, heights[2], 1f));
                    break;

                case 14://1110
                    points.Add(new Vector3(1f, heights[2], 1f));
                    points.Add(new Vector3(1f, heights[3], 0f));
                    points.Add(new Vector3(0.5f, heights[3], 0f));
                    points.Add(new Vector3(0f, heights[1], 0.5f));
                    points.Add(new Vector3(0f, heights[1], 1f));
                    break;

                case 15://1111
                    points.Add(new Vector3(0f, heights[0], 0f));
                    points.Add(new Vector3(0f, heights[1], 1f));
                    points.Add(new Vector3(1f, heights[2], 1f));
                    points.Add(new Vector3(1f, heights[3], 0f));
                    break;
            }
            switch (terrain.CliffType)
            {
                case TerrainCraft.CliffTypes.flat:
                    SetMiddlePoints(points, value);
                    break;
                case TerrainCraft.CliffTypes.round:
                    MeshVertsRound.SetMiddlePoints(points, value);
                    break;
            }
            return points;
        }

        public static List<Vector3> GetFieldPointsVerticalDefault(int value, float[] heights, float minimum)
        {
            List<Vector3> points = new List<Vector3>();
            List<Vector3> drawingpoints = new List<Vector3>(GetUndefaultHorizontalPoints(value, heights));
            float a = 0;

            if (drawingpoints.Count > 0)
            {
                if (value % 5 == 0)
                {
                    for (float h = 0; h <= terrain.CliffWidthSegments; h++)
                    {
                        for (int i = 0; i < drawingpoints.Count / 2; i++)
                        {
                            a = drawingpoints[i].y - (drawingpoints[i].y - minimum) * h / terrain.CliffWidthSegments;
                            points.Add(new Vector3(drawingpoints[i].x, a, drawingpoints[i].z));
                        }
                    }
                    for (float h = 0; h <= terrain.CliffWidthSegments; h++)
                    {
                        for (int i = drawingpoints.Count / 2; i < drawingpoints.Count; i++)
                        {
                            a = drawingpoints[i].y - (drawingpoints[i].y - minimum) * h / terrain.CliffWidthSegments;
                            points.Add(new Vector3(drawingpoints[i].x, a, drawingpoints[i].z));
                        }
                    }
                }
                else
                {
                    for (float h = 0; h <= terrain.CliffWidthSegments; h++)
                    {
                        for (int i = 0; i < drawingpoints.Count; i++)
                        {
                            a = drawingpoints[i].y - (drawingpoints[i].y - minimum) * h / terrain.CliffWidthSegments;
                            points.Add(new Vector3(drawingpoints[i].x, a, drawingpoints[i].z));
                        }
                    }
                }
            }
            return points;
        }

        public static List<Vector3> GetFieldPointsVerticalFromMesh(int value, float[] heights, float minimum)
        {
            List<Vector3> points = new List<Vector3>();
            List<Vector3> drawingpoints = new List<Vector3>(GetUndefaultHorizontalPoints(value, heights));
            if (drawingpoints.Count > 0)
            {
                if (value % 5 == 0)
                {
                    List<Vector3> points1 = new List<Vector3>();
                    points1.AddRange(terrain.CliffMesh.vertices);
                    MatchPointsFromMesh(value, points1, drawingpoints.GetRange(0, drawingpoints.Count / 2), minimum);

                    List<Vector3> points2 = new List<Vector3>();
                    points2.AddRange(terrain.CliffMesh.vertices);
                    MatchPointsFromMesh(value, points2, drawingpoints.GetRange(drawingpoints.Count / 2, drawingpoints.Count / 2), minimum);

                    points.AddRange(points1);
                    points.AddRange(points2);
                }
                else
                {
                    points.AddRange(terrain.CliffMesh.vertices);
                    MatchPointsFromMesh(value, points, drawingpoints, minimum);
                }
            }
            return points;
        }

        public static void MatchPointsFromMesh(int value, List<Vector3> points, List<Vector3> drawingpoints, float minimum)
        {

            for (int i = 0; i < points.Count; i++)
            {
                int index = 0;
                float x = 0;
                while (Mathf.Abs(points[i].x - x) > 0.01f)
                {
                    index++;
                    x += 1f / (float)(terrain.CliffLengthSegments);
                    if (index >= drawingpoints.Count)
                    {
                        index = -1;
                        return;
                    }
                }
                if (index == -1)
                {
                    Debug.Log("Not Found:" + i);
                    return;
                }

                Vector3 p = drawingpoints[index];

                Vector3 v = GetDirection(value, p);
                if (value == 7 || value == 13 || value == 11 || value == 14)
                    v *= -1;
                p -= points[i].z * v;

                float h = drawingpoints[index].y - minimum;
                p.y = minimum + points[i].y * h;

                points[i] = p;

            }
        }

        public static List<Vector3> GetUndefaultHorizontalPoints(int value, float[] heights)
        {
            List<Vector3> points = new List<Vector3>(GetFieldPointsHorizontalDefault(value, heights));
            for (int i = 0; i < points.Count; i++)
                if (Calc.IsDefaultPoint(points[i]))
                {
                    points.RemoveAt(i);
                    i--;
                }
            return points;
        }

        public static void SetMiddlePoints(List<Vector3> points, int value)
        {

            for (int i = 0; i < points.Count; i++)
            {

                if (!Calc.IsDefaultPoint(points[i]))
                {
                    points.InsertRange(i + 1, Calc.RhombPointsInverse(points[i], points[i + 1], terrain.CliffLengthSegments));
                    i += terrain.CliffLengthSegments;
                }
            }

        }

        public static Vector3 GetDirection(int fieldValue, Vector3 point)
        {
            Vector3 center = GetCenter(fieldValue);
            Vector3 direction = Vector3.zero;
            Vector3 center1 = Vector3.zero;
            Vector3 center2 = Vector3.zero;
            Vector3 p = Vector3.zero;
            float l = 0;
            if (center != -Vector3.one)
                direction = point - center;
            else
            {
                switch (fieldValue)
                {
                    case 3:
                        direction = new Vector3(1, 0, 0);
                        break;
                    case 9:
                        direction = new Vector3(0, 0, 1);
                        break;
                    case 6:
                        direction = new Vector3(0, 0, -1);
                        break;
                    case 12:
                        direction = new Vector3(-1, 0, 0);
                        break;
                    case 5:
                        //diagonal
                        center1 = new Vector3(0, 0, 0);
                        center2 = new Vector3(1, 0, 1);
                        p = new Vector3(point.x, 0, point.z);
                        l = (Vector3.Distance(center1, p) - 0.5f) / (0.5f / Mathf.Sqrt(2));
                        center = Vector3.Lerp(center1, center2, l);
                        direction = point - center;
                        break;
                    case 10:
                        //diagonal rotated
                        center1 = new Vector3(1, 0, 0);
                        center2 = new Vector3(0, 0, 1);
                        p = new Vector3(point.x, 0, point.z);
                        l = (Vector3.Distance(center1, p) - 0.5f) / (0.5f / Mathf.Sqrt(2));
                        center = Vector3.Lerp(center1, center2, l);
                        direction = point - center;
                        break;
                }
            }
            direction.y = 0;
            direction = direction.normalized;

            return direction;
        }

        public static Vector3 GetCenter(int fieldValue)
        {
            Vector3 center = -Vector3.one;
            switch (fieldValue)
            {
                case 1:
                    center = new Vector3(0, 0, 0);
                    break;
                case 2:
                    center = new Vector3(0, 0, 1);
                    break;
                case 4:
                    center = new Vector3(1, 0, 1);
                    break;
                case 8:
                    center = new Vector3(1, 0, 0);
                    break;
                case 7:
                    center = new Vector3(1, 0, 0);
                    break;
                case 13:
                    center = new Vector3(0, 0, 1);
                    break;
                case 11:
                    center = new Vector3(1, 0, 1);
                    break;
                case 14:
                    center = new Vector3(0, 0, 0);
                    break;
            }
            return center;
        }
    }
}
