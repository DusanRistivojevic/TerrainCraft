using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace TerrainCraft
{
    [System.Serializable]
    public class Point
    {
        public TerrainCraft terrain;

        public int x;
        public int y;

        public int cliffValue = 0;
        public int colorValue = 0;
        public float heightValue = 0;
        public int rampValue = 0;
        public float waterValue = 0;
        public int prefabIndex = -1;
        public GameObject prefabOnPoint;
        public List<Field> fields
        {
            get
            {
                List<Field> f = new List<Field>();
                for (int a = -1; a < 1; a++)
                    for (int b = -1; b < 1; b++)
                        if (x + a < terrain.Width && x + a >= 0 && y + b < terrain.Height && y + b >= 0)
                            f.Add(terrain.Fields[x + a, y + b]);

                return f;
            }
        }

        public Point2DArray points
        {
            get
            {
                return terrain.Points;
            }
        }

        public void SetIndexes(int X, int Y)
        {
            x = X;
            y = Y;
        }

        public Vector3 GetPosition()
        {
            float ramp = 0;
            if (rampValue == 1)
                ramp = 0.5f;
            Vector3 pos = new Vector3(x, cliffValue + heightValue + ramp, y);
            return pos;
        }

        public void SetPrefab(GameObject prefab)
        {
            prefabOnPoint = prefab;
        }

        void OnChanged()
        {
            if (prefabOnPoint != null)
                prefabOnPoint.transform.localPosition = GetPosition();
        }

        public void DrawTexture(int selectedColorIndex)
        {
            colorValue = selectedColorIndex;
            OnChanged();

        }

        public void MoveHeight(float v)
        {
            List<float> heightValues = new List<float>();
            foreach (Field f in fields)
                foreach (Point p in f.points)
                    heightValues.Add(p.heightValue);

            float minValue = Calc.MinimumInList(heightValues) - 0.5f;
            float maxValue = Calc.MagnitudeInList(heightValues) + 0.5f;

            float value = heightValue;
            value += v;

            if (value > maxValue)
                value = maxValue;
            if (value < minValue)
                value = minValue;
            heightValue = value;
            OnChanged();
        }

        public void SetHeight(float value)
        {
            heightValue = value;
            OnChanged();
        }

        public void SetCliff(int value)
        {

            cliffValue = value;

            if (waterValue == 0)
                foreach (Field f in fields)
                    if (f.HasWater() && cliffValue < f.GetWaterHeight())
                        waterValue = f.GetWaterHeight();

            if (cliffValue > waterValue)
                waterValue = 0;

            while (cliffValue + 2 < waterValue)
                cliffValue++;

            OnChanged();
        }
        public void SetRamp()
        {

            int xx = 0;
            int yy = 0;

            if (points[x - 1, y].cliffValue > points[x, y].cliffValue)
                xx += 1;
            if (points[x + 1, y].cliffValue > points[x, y].cliffValue)
                xx -= 1;
            if (points[x, y + 1].cliffValue > points[x, y].cliffValue)
                yy -= 1;
            if (points[x, y - 1].cliffValue > points[x, y].cliffValue)
                yy += 1;

            if (xx != 0 && yy != 0)
            {
                if (!points[x + xx, y].RampCondition() || !points[x, y + yy].RampCondition())
                    return;
                points[x + xx, y].rampValue = 1;
                points[x, y + yy].rampValue = 1;
            }

            rampValue = 1;
            OnChanged();
        }
        public int[] GetRampDirection()
        {
            Vector3 pos = PointToPosition(this, terrain);
            if ((pos.x < 3 || pos.x > terrain.Width - 2) && (pos.z < 3 && pos.z > terrain.Width - 2))
                return new int[] { 0, 0 };



            int xx = 0;
            int yy = 0;

            if (points[x - 1, y].cliffValue > points[x, y].cliffValue)
                xx += 1;
            if (points[x + 1, y].cliffValue > points[x, y].cliffValue)
                xx -= 1;
            if (points[x, y + 1].cliffValue > points[x, y].cliffValue)
                yy -= 1;
            if (points[x, y - 1].cliffValue > points[x, y].cliffValue)
                yy += 1;


            if (xx == 0 && yy == 0)
            {
                if (points[x - 1, y + 1].cliffValue > points[x, y].cliffValue)
                {
                    xx = 1; yy = -1;
                }
                if (points[x - 1, y - 1].cliffValue > points[x, y].cliffValue)
                {
                    xx = 1; yy = 1;
                }
                if (points[x + 1, y + 1].cliffValue > points[x, y].cliffValue)
                {
                    xx = -1; yy = -1;
                }
                if (points[x + 1, y - 1].cliffValue > points[x, y].cliffValue)
                {
                    xx = -1; yy = 1;
                }
            }


            return new int[] { xx, yy };
        }
        public bool RampCondition()
        {


            Vector3 pos = PointToPosition(this, terrain);
            if ((pos.x < 3 || pos.x > terrain.Width - 2) && (pos.z < 3 && pos.z > terrain.Width - 2))
                return false;

            List<int> cliffList = new List<int>();
            foreach (Field f in fields)
                cliffList.AddRange(f.cliffValues);



            if (Calc.MinimumInList(cliffList, -10) + 1 < Calc.MagnitudeInList(cliffList, 0))
                return false;

            int xx = 0;
            int yy = 0;

            if (x < 1 || x > terrain.Width || y < 1 || y > terrain.Height)
                return false;

            if (points[x - 1, y].cliffValue > points[x, y].cliffValue)
                xx += 1;
            if (points[x + 1, y].cliffValue > points[x, y].cliffValue)
            {
                if (xx > 0)
                    return false;
                xx -= 1;
            }
            if (points[x, y - 1].cliffValue > points[x, y].cliffValue)
                yy += 1;
            if (points[x, y + 1].cliffValue > points[x, y].cliffValue)
            {
                if (yy > 0)
                    return false;
                yy -= 1;
            }

            if (xx == 0 && yy == 0)
            {
                if (points[x - 1, y + 1].cliffValue > points[x, y].cliffValue)
                {
                    xx = 1; yy = -1;
                }
                if (points[x - 1, y - 1].cliffValue > points[x, y].cliffValue)
                {
                    xx = 1; yy = 1;
                }
                if (points[x + 1, y + 1].cliffValue > points[x, y].cliffValue)
                {
                    xx = -1; yy = -1;
                }
                if (points[x + 1, y - 1].cliffValue > points[x, y].cliffValue)
                {
                    xx = -1; yy = 1;
                }
            }
            if (points[x + xx, y + yy].cliffValue > points[x, y].cliffValue)
                return false;

            return (xx != 0 || yy != 0);
        }
        public int[] GetRampPoints(int index)
        {
            List<int> values = new List<int>();
            values.Add(index);

            int[] dir = GetRampDirection();
            switch (dir[0])
            {
                case 1:
                    if (index == 1 && dir[1] == -1)
                        values.Add(3);
                    if (index == 0 && dir[1] == 1)
                        values.Add(2);
                    switch (index)
                    {
                        case 0:
                            values.Add(3);
                            break;
                        case 1:
                            values.Add(2);
                            break;
                    }
                    break;
                case -1:
                    if (index == 2 && dir[1] == -1)
                        values.Add(0);
                    if (index == 3 && dir[1] == 1)
                        values.Add(1);
                    switch (index)
                    {
                        case 3:
                            values.Add(0);
                            break;
                        case 2:
                            values.Add(1);
                            break;
                    }
                    break;
            }
            switch (dir[1])
            {
                case 1:
                    switch (index)
                    {
                        case 0:
                            values.Add(1);
                            break;
                        case 3:
                            values.Add(2);
                            break;
                    }
                    break;
                case -1:
                    switch (index)
                    {
                        case 2:
                            values.Add(3);
                            break;
                        case 1:
                            values.Add(0);
                            break;
                    }
                    break;
            }
            return values.ToArray();
        }
        public bool CliffCondition(int value)
        {

            foreach (Field f in fields)
            {
                if (f.HasRamp())
                    return false;
            }
            if (value < cliffValue && waterValue == 0)
            {
                float wh = 0;
                foreach (Field f in fields)
                    if (f.HasWater() && wh != f.GetWaterHeight())
                    {
                        if (wh == 0)
                            wh = f.GetWaterHeight();
                        else
                            return false;
                    }
                if (wh == 0)
                    return true;

                foreach (Field f in fields)
                    if (!f.HasWater() && wh > Calc.MinimumInList(f.cliffValues, -10))
                        return false;

            }
            return true;
        }
        public bool WaterCondition(int cliff)
        {
            List<int> cliffValues = new List<int>();
            for (int i = 0; i < fields.Count; i++)
                cliffValues.AddRange(fields[i].GetRampedCliffValue());
            if (cliff > Calc.MinimumInList(cliffValues, -10))
                return false;
            return true;
        }
        public static Point PositionToPoint(Vector3 position, TerrainCraft terrain)
        {
            float x = position.x - terrain.transform.position.x;
            float y = position.z - terrain.transform.position.z;
            return terrain.Points[Mathf.RoundToInt(x / terrain.TerrainScale), Mathf.RoundToInt(y / terrain.TerrainScale)];
        }
        public static Vector3 PointToPosition(Point p, TerrainCraft terrain)
        {
            int xx = p.x;
            int zz = p.y;
            Vector3 v = new Vector3((float)xx + terrain.transform.position.x, terrain.transform.position.y, (float)zz + terrain.transform.position.z);
            v *= terrain.TerrainScale;
            return v;
        }

    }
}
