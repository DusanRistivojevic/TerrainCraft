using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace TerrainCraft
{
    public class WaterVerts
    {
        public static Vector32DArray GetWaterPoints(Field field, float height)
        {
            int value = Calc.ListToNumber(field.GetWaterFieldValues(height));
            List<Vector3> points = new List<Vector3>();
            switch (value)
            {
                case 1://0001
                    points.Add(new Vector3(0f, height, 0f));
                    points.Add(new Vector3(0f, height, 0.5f));
                    points.Add(new Vector3(0.5f, height, 0f));
                    break;

                case 2://0010
                    points.Add(new Vector3(0f, height, 1f));
                    points.Add(new Vector3(0.5f, height, 1f));
                    points.Add(new Vector3(0f, height, 0.5f));
                    break;

                case 3://0011
                    points.Add(new Vector3(0f, height, 0f));
                    points.Add(new Vector3(0f, height, 1f));
                    points.Add(new Vector3(0.5f, height, 1f));
                    points.Add(new Vector3(0.5f, height, 0f));
                    break;

                case 4://0100
                    points.Add(new Vector3(1f, height, 1f));
                    points.Add(new Vector3(1f, height, 0.5f));
                    points.Add(new Vector3(0.5f, height, 1f));
                    break;

                case 5://0101
                    points.Add(new Vector3(1f, height, 1f));
                    points.Add(new Vector3(1f, height, 0.5f));
                    points.Add(new Vector3(0.5f, height, 0f));
                    points.Add(new Vector3(0f, height, 0f));
                    points.Add(new Vector3(0f, height, 0.5f));
                    points.Add(new Vector3(0.5f, height, 1f));
                    break;

                case 6://0110
                    points.Add(new Vector3(0f, height, 1f));
                    points.Add(new Vector3(1f, height, 1f));
                    points.Add(new Vector3(1f, height, 0.5f));
                    points.Add(new Vector3(0f, height, 0.5f));
                    break;

                case 7://0111
                    points.Add(new Vector3(0f, height, 1f));
                    points.Add(new Vector3(1f, height, 1f));
                    points.Add(new Vector3(1f, height, 0.5f));
                    points.Add(new Vector3(0.5f, height, 0f));
                    points.Add(new Vector3(0f, height, 0f));
                    break;

                case 8://1000
                    points.Add(new Vector3(1f, height, 0f));
                    points.Add(new Vector3(0.5f, height, 0f));
                    points.Add(new Vector3(1f, height, 0.5f));
                    break;

                case 9://1001
                    points.Add(new Vector3(1f, height, 0f));
                    points.Add(new Vector3(0f, height, 0f));
                    points.Add(new Vector3(0f, height, 0.5f));
                    points.Add(new Vector3(1f, height, 0.5f));
                    break;

                case 10://1010
                    points.Add(new Vector3(0f, height, 1f));
                    points.Add(new Vector3(0.5f, height, 1f));
                    points.Add(new Vector3(1f, height, 0.5f));
                    points.Add(new Vector3(1f, height, 0f));
                    points.Add(new Vector3(0.5f, height, 0f));
                    points.Add(new Vector3(0f, height, 0.5f));
                    break;

                case 11://1011
                    points.Add(new Vector3(0f, height, 0f));
                    points.Add(new Vector3(0f, height, 1f));
                    points.Add(new Vector3(0.5f, height, 1f));
                    points.Add(new Vector3(1f, height, 0.5f));
                    points.Add(new Vector3(1f, height, 0f));
                    break;

                case 12://1100
                    points.Add(new Vector3(1f, height, 1f));
                    points.Add(new Vector3(1f, height, 0f));
                    points.Add(new Vector3(0.5f, height, 0f));
                    points.Add(new Vector3(0.5f, height, 1f));
                    break;

                case 13://1101
                    points.Add(new Vector3(1f, height, 0f));
                    points.Add(new Vector3(0f, height, 0f));
                    points.Add(new Vector3(0f, height, 0.5f));
                    points.Add(new Vector3(0.5f, height, 1f));
                    points.Add(new Vector3(1f, height, 1f));
                    break;

                case 14://1110
                    points.Add(new Vector3(1f, height, 1f));
                    points.Add(new Vector3(1f, height, 0f));
                    points.Add(new Vector3(0.5f, height, 0f));
                    points.Add(new Vector3(0f, height, 0.5f));
                    points.Add(new Vector3(0f, height, 1f));
                    break;

                case 15://1111
                    points.Add(new Vector3(0f, height, 0f));
                    points.Add(new Vector3(0f, height, 1f));
                    points.Add(new Vector3(1f, height, 1f));
                    points.Add(new Vector3(1f, height, 0f));
                    break;
            }
            return new Vector32DArray(points.ToArray());
        }
        public static Color2DArray GetWaterColors(Field field, float height, Color edgeColor, Color middleColor)
        {
            int[] cliffs = field.cliffValues;
            int value = Calc.ListToNumber(field.GetWaterFieldValues(height));
            List<Color> colors = new List<Color>();

            switch (value)
            {
                case 1://0001
                    colors.Add(middleColor);
                    colors.Add(edgeColor);
                    colors.Add(edgeColor);

                    break;

                case 2://0010
                    colors.Add(middleColor);
                    colors.Add(edgeColor);
                    colors.Add(edgeColor);
                    break;

                case 3://0011
                    colors.Add(middleColor);
                    colors.Add(middleColor);
                    colors.Add(edgeColor);
                    colors.Add(edgeColor);
                    break;

                case 4://0100
                    colors.Add(middleColor);
                    colors.Add(edgeColor);
                    colors.Add(edgeColor);
                    break;

                case 5://0101
                    colors.Add(middleColor);
                    colors.Add(edgeColor);
                    colors.Add(edgeColor);
                    colors.Add(middleColor);
                    colors.Add(edgeColor);
                    colors.Add(edgeColor);
                    break;

                case 6://0110
                    colors.Add(middleColor);
                    colors.Add(middleColor);
                    colors.Add(edgeColor);
                    colors.Add(edgeColor);
                    break;

                case 7://0111
                    colors.Add(middleColor);
                    colors.Add(middleColor);
                    colors.Add(edgeColor);
                    colors.Add(edgeColor);
                    colors.Add(middleColor);
                    break;

                case 8://1000
                    colors.Add(middleColor);
                    colors.Add(edgeColor);
                    colors.Add(edgeColor);
                    break;

                case 9://1001
                    colors.Add(middleColor);
                    colors.Add(middleColor);
                    colors.Add(edgeColor);
                    colors.Add(edgeColor);
                    break;

                case 10://1010
                    colors.Add(middleColor);
                    colors.Add(edgeColor);
                    colors.Add(edgeColor);
                    colors.Add(middleColor);
                    colors.Add(edgeColor);
                    colors.Add(edgeColor);
                    break;

                case 11://1011
                    colors.Add(middleColor);
                    colors.Add(middleColor);
                    colors.Add(edgeColor);
                    colors.Add(edgeColor);
                    colors.Add(middleColor);
                    break;

                case 12://1100
                    colors.Add(middleColor);
                    colors.Add(middleColor);
                    colors.Add(edgeColor);
                    colors.Add(edgeColor);
                    break;

                case 13://1101
                    colors.Add(middleColor);
                    colors.Add(middleColor);
                    colors.Add(edgeColor);
                    colors.Add(edgeColor);
                    colors.Add(middleColor);
                    break;

                case 14://1110
                    colors.Add(middleColor);
                    colors.Add(middleColor);
                    colors.Add(edgeColor);
                    colors.Add(edgeColor);
                    colors.Add(middleColor);
                    break;

                case 15://1111
                    if (cliffs[0] < height - field.heightValues[0])
                        colors.Add(middleColor);
                    else
                        colors.Add(edgeColor);
                    if (cliffs[1] < height - field.heightValues[1])
                        colors.Add(middleColor);
                    else
                        colors.Add(edgeColor);
                    if (cliffs[2] < height - field.heightValues[2])
                        colors.Add(middleColor);
                    else
                        colors.Add(edgeColor);
                    if (cliffs[3] < height - field.heightValues[3])
                        colors.Add(middleColor);
                    else
                        colors.Add(edgeColor);
                    break;
            }
            return new Color2DArray(colors.ToArray());
        }
    }
}