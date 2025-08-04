using UnityEngine;
namespace TerrainCraft
{
    [System.Serializable]
    public class JSONmodel
    {
        public int Width;
        public int Height;
        [SerializeField]
        public JSONPoint[] Points;
    }
    [System.Serializable]
    public struct JSONPoint
    {

        [SerializeField]
        public int x;
        [SerializeField]
        public int y;

        public int cliffValue;
        public int colorValue;
        public float heightValue;
        public int rampValue;
        public float waterValue;

        public int prefabOnPoint;
        public Vector3 prefabPosition;
        public Vector3 prefabDirection;
        public Vector3 prefabScale;

    }
}
