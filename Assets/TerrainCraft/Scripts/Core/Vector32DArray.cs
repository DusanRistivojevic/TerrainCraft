using UnityEngine;
using System.Collections;
namespace TerrainCraft
{
    [System.Serializable]
    public class Vector32DArray
    {

        public Vector3[] array;

        public Vector3 this[int i]
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
        public Vector32DArray(Vector3[] vec)
        {
            array = vec;
        }
    }
}
