using UnityEngine;
using System.Collections.Generic;
namespace TerrainCraft
{
    public class Triangulator
    {
        public static List<Vector3> m_points;
        public static void GetPoints(Vector3[] verts)
        {
            m_points = new List<Vector3>();
            m_points.AddRange(verts);
            for (int i = 0; i < verts.Length; i++)
            {
                Vector3 temp = Vector3.zero;
                float offsetZ = verts[i].z * 10;
                temp.y = verts[i].y + offsetZ; ;
                temp.x = verts[i].x;
                m_points[i] = temp;
            }
        }
        public static int[] Triangulate(List<Vector3> verts, bool isVertical, int lengthSegments, int widthSegments)
        {
            return Triangulate(verts.ToArray(), isVertical, lengthSegments, widthSegments);
        }
        public static int[] Triangulate(Vector3[] verts, bool isVertical, int lengthSegments, int widthSegments)
        {
            int[] triangles;

            if (isVertical)
            {
                int hCount2 = lengthSegments + 1;
                int numTriangles = widthSegments * lengthSegments * 6;
                triangles = new int[numTriangles];

                int index = 0;
                for (int y = 0; y < widthSegments; y++)
                    for (int x = 0; x < lengthSegments; x++)
                    {
                        if (index < triangles.Length)
                        {
                            triangles[index] = (y * hCount2) + x;
                            triangles[index + 1] = ((y + 1) * hCount2) + x;
                            triangles[index + 2] = (y * hCount2) + x + 1;

                            triangles[index + 3] = ((y + 1) * hCount2) + x;
                            triangles[index + 4] = ((y + 1) * hCount2) + x + 1;
                            triangles[index + 5] = (y * hCount2) + x + 1;
                            index += 6;
                        }
                    }
            }
            else
            {
                List<int> indices = new List<int>();
                GetPoints(verts);
                int n = m_points.Count;
                if (n < 3)
                    return indices.ToArray();

                int[] V = new int[n];
                if (Area() > 0)
                {
                    for (int v = 0; v < n; v++)
                        V[v] = v;
                }
                else
                {
                    for (int v = 0; v < n; v++)
                        V[v] = (n - 1) - v;
                }

                int nv = n;
                int count = 2 * nv;
                for (int m = 0, v = nv - 1; nv > 2;)
                {
                    if ((count--) <= 0)
                        return indices.ToArray();

                    int u = v;
                    if (nv <= u)
                        u = 0;
                    v = u + 1;
                    if (nv <= v)
                        v = 0;
                    int w = v + 1;
                    if (nv <= w)
                        w = 0;

                    if (Snip(u, v, w, nv, V))
                    {
                        int a, b, c, s, t;
                        a = V[u];
                        b = V[v];
                        c = V[w];
                        indices.Add(a);
                        indices.Add(b);
                        indices.Add(c);
                        m++;
                        for (s = v, t = v + 1; t < nv; s++, t++)
                            V[s] = V[t];
                        nv--;
                        count = 2 * nv;
                    }
                }
                indices.Reverse();
                triangles = indices.ToArray();
            }

            return triangles;
        }

        static float Area()
        {
            int n = m_points.Count;
            float A = 0.0f;
            for (int p = n - 1, q = 0; q < n; p = q++)
            {
                Vector3 pval = m_points[p];
                Vector3 qval = m_points[q];
                A += pval.x * qval.y - qval.x * pval.y;
            }
            return (A * 0.5f);
        }

        static bool Snip(int u, int v, int w, int n, int[] V)
        {
            int p;
            Vector3 A = m_points[V[u]];
            Vector3 B = m_points[V[v]];
            Vector3 C = m_points[V[w]];
            if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
                return false;
            for (p = 0; p < n; p++)
            {
                if ((p == u) || (p == v) || (p == w))
                    continue;
                Vector3 P = m_points[V[p]];
                if (InsideTriangle(A, B, C, P))
                    return false;
            }
            return true;
        }

        static bool InsideTriangle(Vector3 A, Vector3 B, Vector3 C, Vector3 P)
        {
            float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
            float cCROSSap, bCROSScp, aCROSSbp;

            ax = C.x - B.x; ay = C.y - B.y;
            bx = A.x - C.x; by = A.y - C.y;
            cx = B.x - A.x; cy = B.y - A.y;
            apx = P.x - A.x; apy = P.y - A.y;
            bpx = P.x - B.x; bpy = P.y - B.y;
            cpx = P.x - C.x; cpy = P.y - C.y;

            aCROSSbp = ax * bpy - ay * bpx;
            cCROSSap = cx * apy - cy * apx;
            bCROSScp = bx * cpy - by * cpx;

            return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
        }
    }
}