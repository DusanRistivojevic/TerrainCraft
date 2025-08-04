using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using UnityEngine.Rendering;
namespace TerrainCraft
{
    public class FieldGroup : MonoBehaviour
    {
        public WaterGroup waterGroup;
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

        private List<Vector3> vert;
        private List<int> triangles;
        private List<Vector2> uv1;
        private List<Vector2> uv2;
        private List<Vector2> uv3;
        private List<Vector2> uv4;

        public void Repaint()
        {
            Mesh mesh = new Mesh();

            vert = new List<Vector3>();
            triangles = new List<int>();
            uv1 = new List<Vector2>();
            uv2 = new List<Vector2>();
            uv3 = new List<Vector2>();
            uv4 = new List<Vector2>();

            foreach (Field f in fields)
            {
                foreach (int t in f.triangles)
                    triangles.Add(t + vert.Count);

                foreach (Vector3 v in f.GetVertsWithHeight())
                    vert.Add(v + new Vector3(f.x - transform.localPosition.x, 0, f.y - transform.localPosition.z));

                foreach (Vector22DArray v in f.uv1)
                    uv1.AddRange(v.array);
                foreach (Vector22DArray v in f.uv2)
                    uv2.AddRange(v.array);
                foreach (Vector22DArray v in f.uv3)
                    uv3.AddRange(v.array);
                foreach (Vector22DArray v in f.uv4)
                    uv4.AddRange(v.array);

            }
            mesh.vertices = vert.ToArray();
            mesh.SetTriangles(triangles.ToArray(), 0);
            mesh.uv = uv1.ToArray();
            mesh.uv2 = uv2.ToArray();
            mesh.uv3 = uv3.ToArray();
            mesh.uv4 = uv4.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            transform.GetComponent<MeshFilter>().sharedMesh = mesh;
            transform.GetComponent<MeshCollider>().sharedMesh = mesh;
            waterGroup.Repaint();
        }

    }
}