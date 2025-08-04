using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System.Linq;
using System;

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
        
        public Dictionary<Vector3, List<int>> boundaryVertices = new Dictionary<Vector3, List<int>>();

        public void Repaint()
        {
            Mesh mesh = new Mesh();
            boundaryVertices.Clear();

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
                {
                    Vector3 position = v + new Vector3(f.x - transform.localPosition.x, 0, f.y - transform.localPosition.z);
                    
                    position = AlignBoundaryVertex(position);
                    
                    vert.Add(position);
                }

                foreach (Vector22DArray v in f.uv1)
                    uv1.AddRange(v.array);
                foreach (Vector22DArray v in f.uv2)
                    uv2.AddRange(v.array);
                foreach (Vector22DArray v in f.uv3)
                    uv3.AddRange(v.array);
                foreach (Vector22DArray v in f.uv4)
                    uv4.AddRange(v.array);
            }
            
            IdentifyBoundaryVertices(vert);
            
            mesh.vertices = vert.ToArray();
            mesh.SetTriangles(triangles.ToArray(), 0);
            mesh.uv = uv1.ToArray();
            mesh.uv2 = uv2.ToArray();
            mesh.uv3 = uv3.ToArray();
            mesh.uv4 = uv4.ToArray();
            
            mesh.RecalculateNormals();
            SmoothNormals(mesh, true);
            
            RecalculateTangents(mesh);
            
            mesh.RecalculateBounds();
            transform.GetComponent<MeshFilter>().sharedMesh = mesh;
            transform.GetComponent<MeshCollider>().sharedMesh = mesh;
            waterGroup.Repaint();
        }

        private Vector3 AlignBoundaryVertex(Vector3 position)
        {
            const float gridSize = 1.0f;
            const float threshold = 0.001f;
            
            float xMod = position.x % gridSize;
            if (Mathf.Abs(xMod) < threshold || Mathf.Abs(xMod - gridSize) < threshold)
            {
                position.x = Mathf.Round(position.x / gridSize) * gridSize;
            }
            
            float zMod = position.z % gridSize;
            if (Mathf.Abs(zMod) < threshold || Mathf.Abs(zMod - gridSize) < threshold)
            {
                position.z = Mathf.Round(position.z / gridSize) * gridSize;
            }
            
            return position;
        }
        
        private void IdentifyBoundaryVertices(List<Vector3> vertices)
        {
            const float threshold = 0.001f;
            
            for (int i = 0; i < vertices.Count; i++)
            {
                Vector3 pos = vertices[i];
                
                bool isBoundary = false;
                if (Mathf.Abs(pos.x - x) < threshold || 
                    Mathf.Abs(pos.x - (x + terrain.FieldGroupSize)) < threshold)
                {
                    isBoundary = true;
                }
                
                if (Mathf.Abs(pos.z - y) < threshold || 
                    Mathf.Abs(pos.z - (y + terrain.FieldGroupSize)) < threshold)
                {
                    isBoundary = true;
                }
                
                if (isBoundary)
                {
                    Vector3 key = new Vector3(
                        Mathf.Round(pos.x * 1000f) / 1000f,
                        Mathf.Round(pos.y * 1000f) / 1000f,
                        Mathf.Round(pos.z * 1000f) / 1000f
                    );
                    
                    if (!boundaryVertices.ContainsKey(key))
                    {
                        boundaryVertices[key] = new List<int>();
                    }
                    boundaryVertices[key].Add(i);
                }
            }
        }

        private void SmoothNormals(Mesh mesh, bool optimizeBoundaries = false)
        {
            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;
            int[] triangles = mesh.triangles;
          
            Dictionary<Vector3, List<int>> vertexToTriangles = new Dictionary<Vector3, List<int>>();
            for (int i = 0; i < triangles.Length; i++)
            {
                int vertexIndex = triangles[i];
                Vector3 vertex = vertices[vertexIndex];
                
                Vector3 simplifiedVertex = new Vector3(
                    Mathf.Round(vertex.x * 1000) / 1000f,
                    Mathf.Round(vertex.y * 1000) / 1000f,
                    Mathf.Round(vertex.z * 1000) / 1000f
                );
                
                if (!vertexToTriangles.ContainsKey(simplifiedVertex))
                {
                    vertexToTriangles[simplifiedVertex] = new List<int>();
                }
                vertexToTriangles[simplifiedVertex].Add(i / 3); // 添加三角形索引
            }
            
            Vector3[] smoothNormals = new Vector3[normals.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 simplifiedVertex = new Vector3(
                    Mathf.Round(vertices[i].x * 1000) / 1000f,
                    Mathf.Round(vertices[i].y * 1000) / 1000f,
                    Mathf.Round(vertices[i].z * 1000) / 1000f
                );
                
                if (vertexToTriangles.ContainsKey(simplifiedVertex))
                {
                    Vector3 normalSum = Vector3.zero;
                    int count = 0;
                    
                    foreach (int triangleIndex in vertexToTriangles[simplifiedVertex])
                    {
                        int baseIndex = triangleIndex * 3;
                        Vector3 faceNormal = Vector3.Cross(
                            vertices[triangles[baseIndex + 1]] - vertices[triangles[baseIndex]],
                            vertices[triangles[baseIndex + 2]] - vertices[triangles[baseIndex]]
                        ).normalized;
                        

                        faceNormal.y = Mathf.Clamp(faceNormal.y, -0.1f, 0.1f);
                        faceNormal = faceNormal.normalized;
                        
                        normalSum += faceNormal;
                        count++;
                    }
                    
                    smoothNormals[i] = (count > 0) ? (normalSum / count).normalized : normals[i];
                    
                    smoothNormals[i] = Vector3.Lerp(normals[i], smoothNormals[i], 0.7f);
                }
                else
                {
                    smoothNormals[i] = normals[i];
                }
            }
            
            if (optimizeBoundaries)
            {
                OptimizeBoundaryNormals(vertices, smoothNormals);
            }
            
            mesh.normals = smoothNormals;
        }
        
        private void OptimizeBoundaryNormals(Vector3[] vertices, Vector3[] normals)
        {
            foreach (var kvp in boundaryVertices)
            {
                if (kvp.Value.Count > 1)
                {
                    Vector3 avgNormal = Vector3.zero;
                    foreach (int index in kvp.Value)
                    {
                        avgNormal += normals[index];
                    }
                    avgNormal /= kvp.Value.Count;
                    avgNormal = avgNormal.normalized;
                    
                    foreach (int index in kvp.Value)
                    {
                        normals[index] = avgNormal;
                    }
                }
            }
        }

        private void RecalculateTangents(Mesh mesh)
        {
            Vector3[] vertices = mesh.vertices;
            Vector2[] uv = mesh.uv;
            Vector3[] normals = mesh.normals;
            int[] triangles = mesh.triangles;
            
            Vector4[] tangents = new Vector4[vertices.Length];
            Vector3[] tan1 = new Vector3[vertices.Length];
            Vector3[] tan2 = new Vector3[vertices.Length];
            
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int i1 = triangles[i];
                int i2 = triangles[i + 1];
                int i3 = triangles[i + 2];
                
                Vector3 v1 = vertices[i1];
                Vector3 v2 = vertices[i2];
                Vector3 v3 = vertices[i3];
                
                Vector2 w1 = uv[i1];
                Vector2 w2 = uv[i2];
                Vector2 w3 = uv[i3];
                
                float x1 = v2.x - v1.x;
                float x2 = v3.x - v1.x;
                float y1 = v2.y - v1.y;
                float y2 = v3.y - v1.y;
                float z1 = v2.z - v1.z;
                float z2 = v3.z - v1.z;
                
                float s1 = w2.x - w1.x;
                float s2 = w3.x - w1.x;
                float t1 = w2.y - w1.y;
                float t2 = w3.y - w1.y;
                
                float r = 1.0f / (s1 * t2 - s2 * t1);
                Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
                Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);
                
                tan1[i1] += sdir;
                tan1[i2] += sdir;
                tan1[i3] += sdir;
                
                tan2[i1] += tdir;
                tan2[i2] += tdir;
                tan2[i3] += tdir;
            }
            
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 n = normals[i];
                Vector3 t = tan1[i];
                
                Vector3.OrthoNormalize(ref n, ref t);
                tangents[i] = new Vector4(t.x, t.y, t.z, 
                    (Vector3.Dot(Vector3.Cross(n, t), tan2[i]) < 0.0f) ? -1.0f : 1.0f);
            }
            
            mesh.tangents = tangents;
        }
    }
}