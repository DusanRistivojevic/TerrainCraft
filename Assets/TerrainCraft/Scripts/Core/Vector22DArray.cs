using UnityEngine;
using System.Collections;
namespace TerrainCraft
{
    [System.Serializable]
    public class Vector22DArray
    {

        public Vector2[] array;

        public Vector2 this[int i]
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
        public Vector22DArray(Vector2[] vec)
        {
            array = vec;
        }
    }
}
