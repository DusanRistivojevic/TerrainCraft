using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace TerrainCraft
{
    public class WaterGroup : MonoBehaviour
    {

        public int x;
        public int y;
        public TerrainCraft terrain;
        public List<Field> fields
        {
            get
            {
                List<Field> f = new List<Field>();
                for (int a = x; a < x + terrain.FieldGroupSize; a++)
                    for (int b = y; b < y + terrain.FieldGroupSize; b++)
                        f.Add(terrain.Fields[a, b]);
                return f;
            }
        }
        public MeshFilter[] filterers;

        private Field field;
        private List<Vector3> vert;
        private List<int> triangles;
        private List<int> triangles2;
        private List<Vector2> uv1;
        private List<Color> colors;
        public GameObject foam;

        private void Start()
        {
            foreach (Field f in fields)
                CreateParticle(f);
        }

        public void Init(int X, int Y, Material[] m)
        {
            foam = Resources.Load("FoamParticle") as GameObject;
            x = X;
            y = Y;
            gameObject.GetComponent<MeshRenderer>().materials = m;
            m[0].renderQueue = 3400;
            m[1].renderQueue = 3000;
        }
        public void Repaint()
        {
            Mesh mesh = new Mesh();
            mesh.subMeshCount = 2;
            colors = new List<Color>();
            vert = new List<Vector3>();
            triangles = new List<int>();
            triangles2 = new List<int>();
            uv1 = new List<Vector2>();


            foreach (Field f in fields)
                if (f.verticesWater.Count > 0)
                {
                    int[] wt = Triangulator.Triangulate(f.verticesWater[0].array, false, 0, 0);
                    foreach (int t in wt)
                        triangles.Add(t + vert.Count);
                    foreach (Vector3 v in f.verticesWater[0].array)
                        vert.Add(v + new Vector3(f.x - transform.localPosition.x, 0, f.y - transform.localPosition.z));

                    wt = Triangulator.Triangulate(f.verticesWater[1].array, false, 0, 0);
                    foreach (int t in wt)
                        triangles2.Add(t + vert.Count);
                    foreach (Vector3 v in f.verticesWater[1].array)
                        vert.Add(v + new Vector3(f.x - transform.localPosition.x, 0, f.y - transform.localPosition.z));

                    List<Vector2> uv = new List<Vector2>();
                    for (int i = 0; i < f.verticesWater[0].Length; i++)
                        uv.Add(new Vector2(f.verticesWater[0][i].x, f.verticesWater[0][i].z));
                    for (int i = 0; i < f.verticesWater[1].Length; i++)
                        uv.Add(new Vector2(f.verticesWater[1][i].x, f.verticesWater[1][i].z));

                    uv1.AddRange(uv);
                    colors.AddRange(f.verticesWaterColors[0].array);
                    colors.AddRange(f.verticesWaterColors[1].array);
                }



            mesh.vertices = vert.ToArray();
            mesh.SetTriangles(triangles.ToArray(), 0);
            mesh.SetTriangles(triangles2.ToArray(), 1);
            mesh.colors = colors.ToArray();
            mesh.uv = uv1.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            transform.GetComponent<MeshFilter>().sharedMesh = mesh;
        }
        public GameObject CreateParticle(Field field)
        {
            if (field.foamEffect != null)
                DestroyImmediate(field.foamEffect.gameObject);
            int value = field.waterValue;
            Vector3 position = new Vector3(field.x, 0, field.y);
            float waterHeight = field.GetWaterHeight();
            if (value == 0 || value == 15)
                return null;

            GameObject f = Instantiate(foam) as GameObject;
            f.transform.parent = transform;

            switch (value)
            {
                case 1://0001



                    f.transform.position = position + new Vector3(0, waterHeight + 0.01f, 0);
                    f.transform.localEulerAngles = Vector3.up * 45;
                    break;

                case 2://0010


                    f.transform.position = position + new Vector3(0, waterHeight + 0.01f, 1);
                    f.transform.localEulerAngles = Vector3.up * 135;
                    break;

                case 3://0011


                    f.transform.position = position + new Vector3(0, waterHeight + 0.01f, 0.5f);
                    f.transform.localEulerAngles = Vector3.up * 90;
                    break;

                case 4://0100


                    f.transform.position = position + new Vector3(1, waterHeight + 0.01f, 1);
                    f.transform.localEulerAngles = Vector3.up * -135;
                    break;

                case 5://0101


                    f.transform.position = position + new Vector3(0, waterHeight + 0.01f, 0);
                    f.transform.localEulerAngles = Vector3.up * 45;



                    f.transform.position = position + new Vector3(1, waterHeight + 0.01f, 1);
                    f.transform.localEulerAngles = Vector3.up * -135;
                    break;

                case 6://0110


                    f.transform.position = position + new Vector3(0.5f, waterHeight + 0.01f, 1);
                    f.transform.localEulerAngles = Vector3.up * 180;
                    break;

                case 7://0111


                    f.transform.position = position + new Vector3(0.4f, waterHeight + 0.01f, 0.6f);
                    f.transform.localEulerAngles = Vector3.up * 135;
                    break;

                case 8://1000


                    f.transform.position = position + new Vector3(1, waterHeight + 0.01f, 0);
                    f.transform.localEulerAngles = Vector3.up * -45;
                    break;

                case 9://1001


                    f.transform.position = position + new Vector3(0.5f, waterHeight + 0.01f, 0);
                    f.transform.localEulerAngles = Vector3.up;
                    break;

                case 10://1010


                    f.transform.position = position + new Vector3(0, waterHeight + 0.01f, 1);
                    f.transform.localEulerAngles = Vector3.up * 135;



                    f.transform.position = position + new Vector3(1, waterHeight + 0.01f, 0);
                    f.transform.localEulerAngles = Vector3.up * -45;
                    break;

                case 11://1011


                    f.transform.position = position + new Vector3(0.4f, waterHeight + 0.01f, 0.4f);
                    f.transform.localEulerAngles = Vector3.up * 45;
                    break;

                case 12://1100


                    f.transform.position = position + new Vector3(1, waterHeight + 0.01f, 0.5f);
                    f.transform.localEulerAngles = -Vector3.up * 90;
                    break;

                case 13://1101


                    f.transform.position = position + new Vector3(0.6f, waterHeight + 0.01f, 0.4f);
                    f.transform.localEulerAngles = Vector3.up * -45;
                    break;

                case 14://1110


                    f.transform.position = position + new Vector3(0.6f, waterHeight + 0.01f, 0.6f);
                    f.transform.localEulerAngles = Vector3.up * -135;
                    break;
            }
            return f;
        }
    }
}