using UnityEngine;
using System.Collections;
namespace TerrainCraft
{
    public class Field2DArray : ScriptableObject
    {
        public int width;
        [SerializeField]
        public Field[] array;
        public Field this[int x, int y]
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
            array = new Field[w * h];
        }
    }
}
