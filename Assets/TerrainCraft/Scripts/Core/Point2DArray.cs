using UnityEngine;
using System.Collections;
namespace TerrainCraft
{
    [System.Serializable]
    public class Point2DArray : ScriptableObject
    {
        public int width;
        [SerializeField]
        public Point[] array;
        public Point this[int x, int y]
        {
            get
            {
                return array[x + y * width];
            }
            set
            {
                array[x + y * width] = value;
            }
        }
        public void Init(int w, int h)
        {
            width = w;
            array = new Point[w * h];
        }
    }
}
