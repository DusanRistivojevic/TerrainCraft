using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace TerrainCraft
{
    public class Calc
    {

        public static Vector3 AbsoluteVector(Vector3 v)
        {
            return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        }

        public static List<int> NormalizedList(List<int> list)
        {
            List<int> newlist = new List<int>();
            for (int i = 0; i < list.Count; i++)
                if (list[i] > 0)
                    newlist.Add(1);
                else
                    newlist.Add(0);
            return newlist;
        }

        public static int MagnitudeInList(List<int> list, int startValue)
        {
            int maxValue = -10;
            for (int i = startValue; i < list.Count; i++)
                if (maxValue < list[i])
                    maxValue = list[i];
            return maxValue;
        }
        public static int MagnitudeInList(int[] list, int startValue)
        {
            int maxValue = -10;
            for (int i = startValue; i < list.Length; i++)
                if (maxValue < list[i])
                    maxValue = list[i];
            return maxValue;
        }
        public static float MagnitudeInList(List<float> list)
        {
            float maxValue = -10;
            for (int i = 0; i < list.Count; i++)
                if (maxValue < list[i])
                    maxValue = list[i];
            return maxValue;
        }
        public static int ColorMagnitudeInList(List<int> list, int startValue)
        {
            int maxValue = 0;
            for (int i = startValue; i < startValue + 4; i++)
                if (maxValue < list[i])
                    maxValue = list[i];
            return maxValue;
        }
        public static int ColorMagnitudeInList(int[] list, int startValue)
        {
            int maxValue = 0;
            for (int i = startValue; i < startValue + 4; i++)
                if (maxValue < list[i])
                    maxValue = list[i];
            return maxValue;
        }
        public static List<int> AllValuesList(int[] values)
        {
            List<int> list = new List<int>();

            for (int i = 0; i < values.Length; i++)
                list.Add(MinimumInList(values, 0));

            for (int a = MinimumInList(values, 0) + 1; a < 8; a = MinimumInList(values, a) + 1)
            {
                if (MinimumInList(values, a) < 8)
                {
                    for (int b = 0; b < values.Length; b++)
                    {
                        if (values[b] == MinimumInList(values, a))
                            list.Add(MinimumInList(values, a));
                        else
                            list.Add(0);
                    }
                }
            }

            return list;

        }
        public static List<int> AllValuesList(List<int> values)
        {
            List<int> list = new List<int>();

            for (int i = 0; i < values.Count; i++)
                list.Add(MinimumInList(values, 0));

            for (int a = MinimumInList(values, 0) + 1; a < 8; a = MinimumInList(values, a) + 1)
            {
                if (MinimumInList(values, a) < 8)
                {
                    for (int b = 0; b < values.Count; b++)
                    {
                        if (values[b] == MinimumInList(values, a))
                            list.Add(MinimumInList(values, a));
                        else
                            list.Add(0);
                    }
                }
            }

            return list;

        }
        public static int MinimumInList(int[] values, int min)
        {
            int minValue = 8;
            for (int i = 0; i < values.Length; i++)
                if (min <= values[i] && values[i] < minValue)
                    minValue = values[i];
            return minValue;
        }
        public static float MinimumInList(List<float> values)
        {
            float min = 0;
            for (int i = 0; i < values.Count; i++)
                if (min > values[i])
                    min = values[i];
            return min;
        }
        public static int MinimumInList(List<int> values, int min)
        {
            int minValue = 8;
            for (int i = 0; i < values.Count; i++)
                if (min <= values[i] && values[i] < minValue)
                    minValue = values[i];
            return minValue;
        }
        private Vector3 m = Vector3.zero;

        public int Vector3Compare(Vector3 o1, Vector3 o2)
        {
            float angle1 = Mathf.Atan2(o1.z - m.z, o1.x - m.x);
            float angle2 = Mathf.Atan2(o2.z - m.z, o2.x - m.x);

            //For counter-clockwise, just reverse the signs of the return values
            if (angle1 < angle2)
                return 1;
            else if (angle2 < angle1)
                return -1;
            return 0;
        }
        public static float RandomVectorMagnitude(float percent)
        {
            return Random.Range(-percent / 2, percent / 2);
        }
        public static float RandomVectorMagnitudeFromZero(float percent)
        {
            return Random.Range(0, percent);
        }
        public static bool IsPointInEnd(Vector3 point)
        {

            return (point.x == 0 || point.x == 1) || (point.z == 0 || point.z == 1);
        }
        public static bool IsDefaultPoint(Vector3 point)
        {

            return (point.x == 0 || point.x == 1) && (point.z == 0 || point.z == 1);
        }
        public static int ListToNumber(int[] list)
        {
            int number = 0;
            for (int i = 0; i < list.Length; i++)
                number += list[i] * (int)Mathf.Pow(2, i);
            return number;
        }

        public static Vector3 RotatePointAroundHalfPoint(Vector3 point, Vector3 angles)
        {
            Vector3 pivot = new Vector3(0.5f, 0, 0.5f);
            return RotatePointAroundPivot(point, angles, pivot); // return it
        }
        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 angles, Vector3 pivot)
        {
            Vector3 dir = point - pivot; // get point direction relative to pivot
            dir = Quaternion.Euler(angles) * dir; // rotate it
            point = dir + pivot; // calculate rotated point

            point = new Vector3(Mathf.Round(point.x * 1000f) / 1000f, Mathf.Round(point.y * 1000f) / 1000f, Mathf.Round(point.z * 1000f) / 1000f);

            return point; // return it
        }
        public static List<Vector3> RotatePoints(List<Vector3> myPoints, float angle)
        {
            for (int i = 0; i < myPoints.Count; i++)
                myPoints[i] = RotatePointAroundHalfPoint(myPoints[i], angle * Vector3.up);

            return myPoints;
        }

        public static Vector3 GetHeightValue(Vector3 vector, float[] heghtValues)
        {
            float a = Mathf.Lerp(heghtValues[0], heghtValues[1], vector.z);
            float b = Mathf.Lerp(heghtValues[3], heghtValues[2], vector.z);
            float value = Mathf.Lerp(a, b, vector.x);
            return value * Vector3.up;
        }
        public static List<Vector3> RhombPointsInverse(Vector3 v1, Vector3 v2, int lengthSegments)
        {
            List<Vector3> points = new List<Vector3>();
            Vector3 dir = (v1 - v2) / lengthSegments;
            for (int i = lengthSegments - 1; i > 0; i--)
            {
                points.Add(v2 + dir * i);
            }
            return points;
        }
        public static List<Vector3> RhombPoints(Vector3 v1, Vector3 v2, int lengthSegments)
        {
            List<Vector3> points = new List<Vector3>();
            Vector3 dir = (v1 - v2) / lengthSegments;
            for (int i = 1; i < lengthSegments; i++)
            {
                points.Add(v2 + dir * i);
            }
            return points;
        }
        public static List<Vector3> QuadPointsInverse(int value, Vector3 p1, Vector3 p2, int lengthSegments)
        {
            List<Vector3> points = new List<Vector3>();
            Vector3 firstDirection = Vector3.forward;
            Vector3 secondDirection = Vector3.left;
            switch (value)
            {
                case 1:
                    firstDirection = Vector3.forward;
                    secondDirection = Vector3.right;
                    break;
                case 2:
                    firstDirection = Vector3.right;
                    secondDirection = Vector3.back;
                    break;
                case 4:
                    firstDirection = Vector3.back;
                    secondDirection = Vector3.left;
                    break;
                case 8:
                    firstDirection = Vector3.left;
                    secondDirection = Vector3.forward;
                    break;

            }
            if (value != 1 && value != 2 && value != 4 && value != 8)
                return RhombPointsInverse(p1, p2, lengthSegments);

            float l = 0.5f / ((float)lengthSegments / 2f);
            for (int i = 1; i <= lengthSegments / 2; i++)
            {
                points.Add(p1 + secondDirection * l * i);
            }
            for (int i = lengthSegments / 2; i > 0; i--)
            {
                points.Add(p2 + firstDirection * l * i);
            }
            return points;
        }
        public static List<Vector3> RoundPoints(Vector3 center, int lengthSegments)
        {
            float r = 0.5f;
            List<Vector3> points = new List<Vector3>();
            for (int i = lengthSegments - 1; i > 0; i--)
            {
                float angle = i * 0.5f * Mathf.PI / lengthSegments;
                float x = center.x + r * Mathf.Cos(angle);
                float y = center.z + r * Mathf.Sin(angle);

                points.Add(new Vector3(x, center.y, y));
            }
            return points;
        }
        public static List<Vector3> RoundPointsInverse(Vector3 center, int lengthSegments)
        {
            float r = -0.5f;
            List<Vector3> points = new List<Vector3>();
            for (int i = 1; i < lengthSegments; i++)
            {
                float angle = i * 0.5f * Mathf.PI / lengthSegments;
                float x = center.x + r * Mathf.Cos(angle);
                float y = center.z + r * Mathf.Sin(angle);

                points.Add(new Vector3(x, center.y, y));
            }
            return points;
        }

        public static Vector3 InversePoint(Vector3 point)
        {
            if (point.x > 0.99f || point.x < 0.01f)
                return AbsoluteVector(Vector3.right - point);
            if (point.z > 0.99f || point.z < 0.01f)
                return AbsoluteVector(Vector3.forward - point);
            return point;
        }
    }
}
