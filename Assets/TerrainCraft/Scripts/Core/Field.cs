using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace TerrainCraft
{
    [System.Serializable]
    public class Field
    {
        public FieldGroup owner
        {
            get
            {
                return terrain.Groups[(x / terrain.FieldGroupSize) + (y / terrain.FieldGroupSize) * terrain.Width / terrain.FieldGroupSize];
            }
        }

        public TerrainCraft terrain;
        public int x;
        public int y;
        public List<Color2DArray> verticesWaterColors;
        public List<Vector32DArray> vertices;
        public List<Vector32DArray> verticesWater;
        public int waterValue;
        public List<int> triangles;
        public List<Vector22DArray> uv1;
        public List<Vector22DArray> uv2;
        public List<Vector22DArray> uv3;
        public List<Vector22DArray> uv4;
        public GameObject foamEffect;

        private int cliffColorValue = 7;

        public void SetIndexes(int X, int Y)
        {
            x = X;
            y = Y;
        }

        public List<Point> points
        {
            get
            {
                return new List<Point>() {
                terrain.Points [x, y],
                terrain.Points [x, y + 1],
                terrain.Points [x + 1, y + 1],
                terrain.Points [x + 1, y]
            };
            }
        }

        public int[] rampValues
        {
            get
            {
                return new int[] {
                points [0].rampValue,
                points [1].rampValue,
                points [2].rampValue,
                points [3].rampValue
            };
            }
        }

        public bool IsDefault()
        {
            foreach (Point p in points)
                if (p.cliffValue != 0 || p.rampValue != 0 || p.colorValue != 0)
                    return false;
            return true;
        }

        public int[] cliffValues
        {
            get
            {
                return new int[] {
                points [0].cliffValue,
                points [1].cliffValue,
                points [2].cliffValue,
                points [3].cliffValue
            };
            }
        }

        public int[] colorValues
        {
            get
            {
                return new int[] {
                points [0].colorValue,
                points [1].colorValue,
                points [2].colorValue,
                points [3].colorValue
            };
            }
        }

        public float[] heightValues
        {
            get
            {
                return new float[] {
                points [0].heightValue,
                points [1].heightValue,
                points [2].heightValue,
                points [3].heightValue
            };
            }
        }

        void SetTriangles(int[] t)
        {
            int vv = 0;
            foreach (Vector32DArray v in vertices)
                vv += v.Length;
            if (vv > 0)
                for (int i = 0; i < t.Length; i++)
                    t[i] += vv;
            triangles.AddRange(t);
        }

        void SetFieldUVs(int uvIndex, List<Vector2> uvPoints, int repaintTime)
        {
            if (uv1.Count < repaintTime)
            {

                uv1.Add(new Vector22DArray(uvPoints.ToArray()));
                uv2.Add(new Vector22DArray(uvPoints.ToArray()));
                uv3.Add(new Vector22DArray(uvPoints.ToArray()));
                uv4.Add(new Vector22DArray(uvPoints.ToArray()));

            }
            switch (uvIndex)
            {
                case 1:
                    uv1[repaintTime - 1] = new Vector22DArray(uvPoints.ToArray());
                    uv2[repaintTime - 1] = new Vector22DArray(uvPoints.ToArray());
                    uv3[repaintTime - 1] = new Vector22DArray(uvPoints.ToArray());
                    uv4[repaintTime - 1] = new Vector22DArray(uvPoints.ToArray());
                    break;
                case 2:
                    uv2[repaintTime - 1] = new Vector22DArray(uvPoints.ToArray());
                    uv3[repaintTime - 1] = new Vector22DArray(uvPoints.ToArray());
                    uv4[repaintTime - 1] = new Vector22DArray(uvPoints.ToArray());
                    break;
                case 3:
                    uv3[repaintTime - 1] = new Vector22DArray(uvPoints.ToArray());
                    uv4[repaintTime - 1] = new Vector22DArray(uvPoints.ToArray());
                    break;
                case 4:
                    uv4[repaintTime - 1] = new Vector22DArray(uvPoints.ToArray());
                    break;
            }
        }

        void SetFieldUVs(int uvIndex, List<Vector2> uvPoints)
        {
            SetFieldUVs(uvIndex, uvPoints, vertices.Count);
        }

        List<Vector22DArray> GetAllUV(List<Vector22DArray> uv)
        {
            List<Vector22DArray> r = new List<Vector22DArray>();
            foreach (Vector22DArray v in uv)
                r.Add(v);
            return r;
        }

        public void Repaint()
        {
            verticesWaterColors = new List<Color2DArray>();
            verticesWater = new List<Vector32DArray>();
            vertices = new List<Vector32DArray>();
            triangles = new List<int>();
            uv1 = new List<Vector22DArray>();
            uv2 = new List<Vector22DArray>();
            uv3 = new List<Vector22DArray>();
            uv4 = new List<Vector22DArray>();
            GetMeshValues();
            GetWaterMeshValues();
        }

        public void SetDefault()
        {
            verticesWaterColors = new List<Color2DArray>();
            verticesWater = new List<Vector32DArray>();
            vertices = new List<Vector32DArray>();
            triangles = new List<int>();
            uv1 = new List<Vector22DArray>();
            uv2 = new List<Vector22DArray>();
            uv3 = new List<Vector22DArray>();
            uv4 = new List<Vector22DArray>();
            List<Vector3> vecs = new List<Vector3>();
            vecs.AddRange(new Vector3[] {
            new Vector3 (0f, 0, 0f),
            new Vector3 (0f, 0, 1f),
            new Vector3 (1f, 0, 1f),
            new Vector3 (1f, 0, 0f)
            });
            vertices.Add(new Vector32DArray(vecs.ToArray()));
            triangles.AddRange(new int[] { 0, 1, 2, 2, 3, 0 });

            SetUVSRandomDefault(vecs, false, 4, 0, 1);
        }
        void GetWaterMeshValues()
        {
            float waterHeight = GetWaterHeight();
            waterValue = Calc.ListToNumber(GetWaterFieldValues(waterHeight));

            
            if (!HasWater())
                return;

            verticesWater.Add(WaterVerts.GetWaterPoints(this, waterHeight));
            Color c1 = Color.white;
            Color c2 = Color.white;
            c1.a = 0.15f;
            c2.a = 1;
            verticesWaterColors.Add(WaterVerts.GetWaterColors(this, waterHeight-0.1f, c1, c2));
            c1.a = 0.35f;
            verticesWater.Add(WaterVerts.GetWaterPoints(this, waterHeight - 0.26f));
            verticesWaterColors.Add(WaterVerts.GetWaterColors(this, waterHeight - 0.26f, c1, c2));

        }
        void GetMeshValues()
        {

            realPoints = new List<Vector3>();
            rndPoints = new List<Vector3>();

            List<int> cliffs = new List<int>();
            cliffs.AddRange(GetRampedCliffValue());
            int minimum = Calc.MinimumInList(cliffs, -10);
            int lastminimum = minimum;
            for (int i = minimum; i < Calc.MagnitudeInList(cliffs, 0) + 1; i++)
                if (cliffs.Contains(i))
                {
                    CreateMeshes(i, 0, false);
                    if (i != minimum)
                    {
                        while (lastminimum != i)
                        {
                            CreateMeshes(lastminimum + 1, lastminimum, true);
                            lastminimum++;
                        }
                    }

                }


        }

        void CreateMeshes(int height, int minimum, bool isV)
        {
            int value = Calc.ListToNumber(GetFieldValues(height));
            int[] colors = new int[] { cliffColorValue, cliffColorValue, cliffColorValue, cliffColorValue };
            if (!isV)
                colors = GetFieldColorValues(height);




            List<Vector3> myPoints = new List<Vector3>();
            myPoints.AddRange(MeshVerts.GetFieldPoints(terrain, this, value, height, minimum, isV));

            if (isV && (value == 5 || value == 10))
            {
                List<Vector3> points = new List<Vector3>();
                for (int a = 0; a < 2; a++)
                {
                    points = new List<Vector3>();
                    for (int b = myPoints.Count / 2 * a; b < myPoints.Count / 2 * (a + 1); b++)
                        points.Add(myPoints[b]);
                    SetMesh(points, colors, value, height, isV);
                }
            }
            else
                SetMesh(myPoints, colors, value, height, isV);
        }

        void SetMesh(List<Vector3> myPoints, int[] colors, int value, int height, bool isV)
        {
            if (myPoints.Count <= 2)
                return;
            if (terrain.RandomizeCliffMeshMaximum != 0 && terrain.RandomizeCliffMeshMinimum != 0)
                RandomizePointsOnMeshes(myPoints, value);

            if (!isV || terrain.CliffMesh == null)
                SetTriangles(Triangulator.Triangulate(myPoints, isV, terrain.CliffLengthSegments, terrain.CliffWidthSegments));
            else
            {
                List<int> tris = new List<int>();
                tris.AddRange(terrain.CliffMesh.triangles);
                tris.Reverse();
                SetTriangles(tris.ToArray());
            }

            vertices.Add(new Vector32DArray(myPoints.ToArray()));

            RepaintUV(myPoints, colors, isV, vertices.Count);

        }

        public void RepaintUV(List<Vector3> myPoints, int[] colorValues, bool isVertical, int repaintTime)
        {

            List<int> list = new List<int>();
            list.AddRange(Calc.AllValuesList(colorValues));

            List<int> normalizedlist = new List<int>();
            normalizedlist.AddRange(Calc.NormalizedList(list));



            int count = 0;
            int xx = (Calc.ColorMagnitudeInList(list, 0) / 4) * 8;
            int yy = (Calc.ColorMagnitudeInList(list, 0) % 4) * 4;
            SetUVSRandomDefault(myPoints, isVertical, 4 + xx, yy, repaintTime);
            for (int i = 4; i < list.Count; i += 4)
            {

                int x = (int)((Calc.ColorMagnitudeInList(list, i)) / 4) * 8;
                int y = (int)(Calc.ColorMagnitudeInList(list, i) % 4) * 4 + 3;

                x += normalizedlist[i] * 2;
                y -= normalizedlist[i + 1] * 2;
                y -= normalizedlist[i + 2];
                x += normalizedlist[i + 3];
                count++;
                SetUVS(myPoints, isVertical, count, x, y, repaintTime);
            }


        }

        public void SetUVSRandomDefault(List<Vector3> myPoints, bool isVertical, int x, int y, int repaintTime)
        {
            int a = x;
            if (Random.Range(0, 100) < 30)
                a = Random.Range(x, x + 4);
            int b = Random.Range(y, y + 4);
            SetUVS(myPoints, isVertical, 0, a, b, repaintTime);
        }
        public void SetUVSRandomCliff(List<Vector3> myPoints, int repaintTime)
        {
            int a = Random.Range(0, 4), b = Random.Range(0, 4);
            SetUVS(myPoints, true, 3, a, b, repaintTime);

        }
        public void SetUVS(List<Vector3> myPoints, bool isVertical, int uvIndex, int x, int y, int repaintTime)
        {

            List<Vector3> ps = new List<Vector3>();
            ps.AddRange(myPoints);
            ps.Sort((aa, bb) => aa.magnitude.CompareTo(bb.magnitude));

            float a = 0.0625f;//1f / 16f;
            float b = 0.0625f;//1f / 16f;

            Vector2 offset = new Vector2(b * x, a * y);

            List<Vector2> uvPoints = new List<Vector2>();
            if (isVertical)
            {
                if (terrain.CliffMesh != null)
                    uvPoints.AddRange(GetVerticalMeshUVs(offset));
                else
                    uvPoints.AddRange(GetVerticalUVs(myPoints, offset));
            }
            else
                uvPoints.AddRange(GetHorizontalUVs(myPoints, offset));
            SetFieldUVs(uvIndex + 1, uvPoints, repaintTime);
        }

        List<Vector2> GetVerticalUVs(List<Vector3> myPoints, Vector2 offset)
        {
            List<Vector2> uvPoints = new List<Vector2>();
            float min = 0.0005f, max = 0.0620f;

            for (float b = 0; b <= terrain.CliffWidthSegments; b++)
                for (float a = 0; a <= terrain.CliffLengthSegments; a++)
                {
                    float vx = Mathf.Lerp(min, max, a / terrain.CliffLengthSegments);
                    float vy = Mathf.Lerp(min, max, b / terrain.CliffWidthSegments);
                    uvPoints.Add(new Vector2(vx, vy) + offset);
                }

            return uvPoints;
        }

        List<Vector2> GetHorizontalUVs(List<Vector3> myPoints, Vector2 offset)
        {
            List<Vector2> uvPoints = new List<Vector2>();
            float min = 0.0005f, max = 0.0620f;

            for (int i = 0; i < myPoints.Count; i++)
            {

                float vx = Mathf.Lerp(min, max, myPoints[i].x);
                float vy = Mathf.Lerp(min, max, myPoints[i].z);

                uvPoints.Add(new Vector2(vx, vy) + offset);
            }

            return uvPoints;
        }
        List<Vector2> GetVerticalMeshUVs(Vector2 offset)
        {
            List<Vector2> myPoints = new List<Vector2>();
            myPoints.AddRange(terrain.CliffMesh.uv);
            List<Vector2> uvPoints = new List<Vector2>();
            float min = 0.0005f, max = 0.0620f;

            for (int i = 0; i < myPoints.Count; i++)
            {

                float vx = Mathf.Lerp(min, max, myPoints[i].x);
                float vy = Mathf.Lerp(min, max, myPoints[i].y);

                uvPoints.Add(new Vector2(vx, vy) + offset);
            }

            return uvPoints;
        }
        void RandomizePointsOnMeshes(List<Vector3> points, int fieldValue)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (!Calc.IsPointInEnd(points[i]))
                {
                    Vector3 direction = -MeshVerts.GetDirection(fieldValue, points[i]) * 0.5f;
                    points[i] = GetRandomPoint(points[i], direction);

                }
            }

        }

        private List<Vector3> realPoints = new List<Vector3>();
        private List<Vector3> rndPoints = new List<Vector3>();

        public Vector3 GetRandomPoint(Vector3 point, Vector3 direction)
        {
            point = new Vector3(Mathf.Round(point.x * 100) / 100f, Mathf.Round(point.y * 100) / 100f, Mathf.Round(point.z * 100) / 100f);
            int i = realPoints.IndexOf(point);

            if (i < 0)
            {

                realPoints.Add(point);
                rndPoints.Add(point - direction * Random.Range(terrain.RandomizeCliffMeshMinimum, terrain.RandomizeCliffMeshMaximum));
                return rndPoints[rndPoints.Count - 1];
            }
            return rndPoints[i];
        }

        public bool HasRamp()
        {

            for (int i = 0; i < 4; i++)
                if (HasRamp(i))
                    return true;

            return false;
        }

        public bool HasRamp(int i)
        {
            if (rampValues[i] > 0)
                return true;
            return false;
        }

        public List<Vector3> GetVertsWithHeight()
        {

            float[] heights = heightValues;

            List<Vector3> myPoints = new List<Vector3>();
            foreach (Vector32DArray v in vertices)
                myPoints.AddRange(v.array);

            for (int i = 0; i < myPoints.Count; i++)
                myPoints[i] += Calc.GetHeightValue(myPoints[i], heights);

            return myPoints;
        }
        public int[] GetWaterFieldValues(float height)
        {
            int[] cliffs = GetWaterRampedCliffValue();

            int[] fieldValues = new int[] { 0, 0, 0, 0 };

            for (int i = 0; i < cliffs.Length; i++)
                if (cliffs[i] < height)
                    fieldValues[i] = 1;

            return fieldValues;
        }
        public bool HasWater()
        {
            for (int i = 0; i < points.Count; i++)
                if (points[i].waterValue != 0)
                    return true;
            return false;
        }
        public float GetWaterHeight()
        {
            float w = 10;
            for (int i = 0; i < points.Count; i++)
                if (points[i].waterValue != 0 && w > points[i].waterValue)
                    w = points[i].waterValue;

            for (int i = 0; i < points.Count; i++)
                if (w < points[i].waterValue)
                    points[i].waterValue = 0;
            return w;
        }
        public int[] GetFieldValues(int height)
        {
            int[] cliffs = GetRampedCliffValue();

            int[] fieldValues = new int[] { 0, 0, 0, 0 };

            for (int i = 0; i < cliffs.Length; i++)
                if (cliffs[i] == height)
                    fieldValues[i] = 1;
            //fix for double cliffs
            if (height != Calc.MinimumInList(cliffs, -10) && height != Calc.MagnitudeInList(cliffs, 0))
                for (int i = 0; i < cliffs.Length; i++)
                    if (cliffs[i] > height)
                        fieldValues[i] = 1;

            return fieldValues;
        }

        public int[] GetRampedCliffValue()
        {
            int[] cliffs = cliffValues;
            if (HasRamp())
            {
                for (int i = 0; i < cliffs.Length; i++)
                {
                    if (HasRamp(i))
                    {
                        int[] ramps = points[i].GetRampPoints(i);
                        foreach (int r in ramps)
                            cliffs[r] = Calc.MinimumInList(cliffValues, -10) + 1;
                    }
                }

            }
            return cliffs;
        }
        public int[] GetWaterRampedCliffValue()
        {
            int[] cliffs = cliffValues;
            if (HasRamp())
            {
                for (int i = 0; i < cliffs.Length; i++)
                {
                    if (HasRamp(i))
                    {
                        cliffs[i] = Calc.MinimumInList(cliffValues, -10) + 1;
                        //Debug.Log (cliffs[i]);
                    }
                }

            }
            return cliffs;
        }

        public int[] GetFieldColorValues(int height)
        {
            int[] fieldValues = GetFieldValues(height);
            int[] colors = colorValues;
            for (int i = 0; i < 4; i++)
            {
                if (fieldValues[i] == 0)
                    colors[i] = cliffColorValue;
                if (fieldValues[i] == 1)
                    colors[i] = colorValues[i];
            }
            return colors;
        }




        Vector2 GetPoint(int i)
        {
            switch (i)
            {
                case 0:
                    return Vector2.zero;
                case 1:
                    return Vector2.up;
                case 2:
                    return Vector2.one;
                case 3:
                    return Vector2.right;
            }
            return Vector2.zero;
        }
    }
}
