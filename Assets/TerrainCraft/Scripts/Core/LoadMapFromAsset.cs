using System.IO;
using UnityEngine;
namespace TerrainCraft
{
    [RequireComponent(typeof(TerrainCraft))]
    public class LoadMapFromAsset : MonoBehaviour
    {
        [SerializeField]
        public TextAsset terraincraftJson;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            TerrainCraft terrain = GetComponent<TerrainCraft>();
            string json = terraincraftJson.text;
            JSONmodel model = JsonUtility.FromJson<JSONmodel>(json);
            terrain.Width = model.Width;
            terrain.Height = model.Height;
            terrain.CreateNew();
            for (int i = 0; i < model.Points.Length; i++)
            {
                terrain.Points.array[i].x = model.Points[i].x;
                terrain.Points.array[i].y = model.Points[i].y;

                terrain.Points.array[i].cliffValue = model.Points[i].cliffValue;
                terrain.Points.array[i].colorValue = model.Points[i].colorValue;
                terrain.Points.array[i].heightValue = model.Points[i].heightValue;
                terrain.Points.array[i].rampValue = model.Points[i].rampValue;
                terrain.Points.array[i].waterValue = model.Points[i].waterValue;

                terrain.Points.array[i].prefabIndex = model.Points[i].prefabOnPoint;
                if (terrain.Points.array[i].prefabIndex != -1)
                {
                    terrain.Points.array[i].prefabOnPoint = Instantiate(terrain.Prefabs[terrain.Points.array[i].prefabIndex], terrain.PrefabParent);
                    terrain.Points.array[i].prefabOnPoint.transform.position = model.Points[i].prefabPosition;
                    terrain.Points.array[i].prefabOnPoint.transform.eulerAngles = model.Points[i].prefabDirection;
                    terrain.Points.array[i].prefabOnPoint.transform.localScale = model.Points[i].prefabScale;
                }

            }
            terrain.RepaintAll();
        }
    }
}
