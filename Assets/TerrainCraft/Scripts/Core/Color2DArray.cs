using UnityEngine;
using System.Collections;
namespace TerrainCraft
{
    [System.Serializable]
    public class Color2DArray
    {

        public Color[] array;

        public Color this[int i]
        {
            get
            {
                return array[i];
            }
            set
            {
                array[i] = value;
            }
        }
        public int Length
        {
            get
            {
                return array.Length;
            }
        }
        public Color2DArray(Color[] vec)
        {
            array = vec;
        }
    }
}
