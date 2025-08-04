using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TerrainCraft
{
    [ExecuteInEditMode]
    public class TerrainCraft : MonoBehaviour
    {
        [HideInInspector]
        public string path;

        public int Width = 32;
        public int Height = 32;
        public float TerrainScale = 1;

        public float CliffHeight = 1;
        public Mesh CliffMesh;
        public int CliffLengthSegments = 4;
        public int CliffWidthSegments = 4;
        public Texture2D clifftexture;
        [Range(0, -0.45f)]
        public float RandomizeCliffMeshMinimum = -0.2f;
        [Range(0, 0.45f)]
        public float RandomizeCliffMeshMaximum = 0.2f;
        public CliffTypes CliffType;

        public Material ShallowWaterMaterial;
        public Material DeepWaterMaterial;


        public Texture2D[] Textures = new Texture2D[7];

        public GameObject[] Prefabs;
        public bool PrefabRandomRotation = true;
        [Range(0, 0.35f)]
        public float PrefabRandomMovement = 0.1f;
        [Range(0, 0.5f)]
        public float prefabRandomSize = 0.25f;

        public bool HideChildren = false;


        public int FieldGroupSize = 16;

        [HideInInspector]
        public Material TerrainMaterial;
        [HideInInspector]
        public GameObject TerrainLightObject;
        [HideInInspector]
        public Field2DArray Fields;
        [HideInInspector]
        public Point2DArray Points;
        [HideInInspector]
        public List<FieldGroup> Groups;
        [HideInInspector]
        public GameObject cursorObject;
        [HideInInspector]
        public Texture2D[] Icons = new Texture2D[8];
        [HideInInspector]
        public Texture2D TextureAtlas;
        [HideInInspector]
        public Transform PrefabParent;

        public enum CliffTypes { round, flat };
        private LayerMask layerMask;


        void Init()
        {
            layerMask = ~(1 << gameObject.layer);
            if (cursorObject == null)
                cursorObject = Resources.Load("cursor") as GameObject;

            if (TerrainMaterial == null)
                TerrainMaterial = Resources.Load("TerrainMaterial") as Material;
            if (ShallowWaterMaterial == null)
                ShallowWaterMaterial = Resources.Load("ShallowWaterMaterial") as Material;
            if (DeepWaterMaterial == null)
                DeepWaterMaterial = Resources.Load("DeepWaterMaterial") as Material;

            if (PrefabParent == null)
            {
                GameObject tp = new GameObject("Prefab Parent");
                if (HideChildren)
                    tp.hideFlags = HideFlags.HideInHierarchy;
                tp.transform.parent = transform;
                tp.transform.localScale = Vector3.one;
                tp.transform.localPosition = Vector3.zero;
                tp.transform.localEulerAngles = Vector3.zero;
                PrefabParent = tp.transform;
            }

            if (CliffMesh != null)
            {
                int i = 0, ii = 0;
                while (ii < CliffMesh.vertices.Length)
                {
                    if (CliffMesh.vertices[i].y == 0)
                        i++;
                    ii++;
                }
                CliffLengthSegments = i - 1;
            }
        }
        public void CreateNew()
        {
            PackTextures();
            Init();
            if (Width <= 0 || Height <= 0)
            {
                Width = 1; Height = 1;
                return;
            }
            if (CliffLengthSegments < 1)
            {
                if (CliffMesh != null)
                    Debug.LogError("Cliff Mesh is not correct.");
                CliffLengthSegments = 1;
                return;
            }
            if (CliffWidthSegments < 1)
                CliffWidthSegments = 1;

            Width -= Width % FieldGroupSize;
            Height -= Height % FieldGroupSize;

            if (Height > 320)
                Height = 320;
            if (Width > 320)
                Width = 320;


            Create();

            cursorObject = Instantiate(cursorObject,transform) as GameObject;
            if (HideChildren)
                cursorObject.hideFlags = HideFlags.HideInHierarchy;
        }
        public void PackTextures()
        {
            TerrainMaterial = new Material(Shader.Find("Shader Graphs/URPTerrainCraftShader"));
            
            TextureAtlas = new Texture2D(2048, 2048, TextureFormat.ARGB32, false);
            Icons = new Texture2D[8];
            for (int i = 0; i < Textures.Length; i++)
            {
                if (Textures[i] != null)
                {
                    Icons[i] = new Texture2D(128, 128);
                    Icons[i].SetPixels(Textures[i].GetPixels(0, 3 * 128, 128, 128));
                    Icons[i].Apply();
                    Color[] c = Textures[i].GetPixels();
                    TextureAtlas.SetPixels((i / 4) * 1024, (i % 4) * 512, 1024, 512, c);
                }
            }
            if (clifftexture != null)
            {
                Icons[7] = new Texture2D(128, 128);
                Icons[7].SetPixels(clifftexture.GetPixels(0, 3 * 128, 128, 128));
                Icons[7].Apply();
            }

            TextureAtlas.SetPixels(1024, 3 * 512, 1024, 512, clifftexture.GetPixels());
            TextureAtlas.Apply(false);
            TextureAtlas.filterMode = FilterMode.Point;
            TextureAtlas.wrapMode = TextureWrapMode.Clamp;
            TerrainMaterial.SetTexture("_MainTex", TextureAtlas);
            TerrainMaterial.SetTexture("_MainTex2",TextureAtlas);
            TerrainMaterial.SetTexture("_MainTex3",TextureAtlas);
            TerrainMaterial.SetTexture("_MainTex4",TextureAtlas);
        }
        void Update()
        {
            SetDefaultTransform();
        }
        public void SetDefaultTransform()
        {
            if (transform.lossyScale != Vector3.one * TerrainScale)
            {
                transform.localScale = LossyToLocalScale();
            }
            if (transform.eulerAngles != Vector3.zero)
                transform.eulerAngles = Vector3.zero;
        }
        Vector3 LossyToLocalScale()
        {
            Vector3 sf = Vector3.one;

            Transform parentTransform = null;
            if (transform != transform.root)
                parentTransform = transform.parent;

            while (parentTransform)
            {
                sf.x *= 1.0f / parentTransform.localScale.x;
                sf.y *= 1.0f / parentTransform.localScale.y;
                sf.z *= 1.0f / parentTransform.localScale.z;
                if (parentTransform != transform.root)
                {
                    parentTransform = parentTransform.parent;
                }
                else
                {
                    parentTransform = null;
                }
            }

            return sf * TerrainScale;
        }

        void Create()
        {
            Fields = ScriptableObject.CreateInstance<Field2DArray>();
            Fields.Init(Width, Height);
            Points = ScriptableObject.CreateInstance<Point2DArray>();
            Points.Init(Width + 1, Height + 1);
            Groups = new List<FieldGroup>();

            for (int y = 0; y < Height + 1; y++)
                for (int x = 0; x < Width + 1; x++)
                {
                    Points[x, y] = new Point();
                    Points[x, y].SetIndexes(x, y);
                    Points[x, y].terrain = this;
                }


            CreateFields();

            for (int y = 0; y < Height; y += FieldGroupSize)
                for (int x = 0; x < Width; x += FieldGroupSize)
                {
                    CreateGroup(x, y, FieldGroupSize);
                }

            RepaintAllGroups();
        }
        public void CreateFields()
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    Field f = new Field();
                    f.terrain = this;
                    Fields[x, y] = f;
                    f.x = x;
                    f.y = y;
                    if (f.IsDefault())
                        f.SetDefault();
                    else
                        f.Repaint();
                }
        }
        public void CreateGroup(int x, int y, int size)
        {
            GameObject groupObject = new GameObject("FieldGroup");
            if (HideChildren)
                groupObject.hideFlags = HideFlags.HideInHierarchy;
            groupObject.AddComponent<MeshFilter>();
            groupObject.AddComponent<MeshRenderer>().material = TerrainMaterial;
            groupObject.AddComponent<MeshCollider>();
            FieldGroup f = groupObject.AddComponent<FieldGroup>();
            f.gameObject.layer = gameObject.layer;
            Groups.Add(f);
            f.transform.parent = transform;
            f.transform.localPosition = new Vector3(x * size, 0, y * size);
            f.transform.localScale = Vector3.one;
            f.x = x;
            f.y = y;
            f.terrain = this;
            f.waterGroup = CreateWater(x, y, size);
            if (HideChildren)
                groupObject.hideFlags = HideFlags.HideInHierarchy;
        }
        public WaterGroup CreateWater(int x, int y, int size)
        {
            GameObject groupObject = new GameObject("WaterGroup");
            if (HideChildren)
                groupObject.hideFlags = HideFlags.HideInHierarchy;
            groupObject.AddComponent<MeshFilter>();
            groupObject.AddComponent<MeshRenderer>();
            WaterGroup wg = groupObject.AddComponent<WaterGroup>();
            wg.gameObject.layer = gameObject.layer;
            wg.transform.parent = transform;
            wg.transform.localPosition = new Vector3(x * size, 0, y * size);
            wg.transform.localScale = Vector3.one;
            wg.Init(x, y, new Material[] { ShallowWaterMaterial, DeepWaterMaterial });
            wg.terrain = this;
            return wg;
        }
        void RepaintAllGroups_v1()
        {
            if (Groups == null || Groups[0] == null)
                return;
            foreach (FieldGroup fg in Groups)
                fg.Repaint();

        }
        
        public void RepaintAllGroups_v2()
        {
            if (Groups == null || Groups[0] == null)
                return;
    
            foreach (FieldGroup fg in Groups)
            {
                Mesh mesh = fg.GetComponent<MeshFilter>().sharedMesh;
                if (mesh != null)
                {
                    mesh.RecalculateNormals();
                }
            }
    
            for (int i = 0; i < Groups.Count; i++)
            {
                Groups[i].Repaint();
        
                if (i > 0) ProcessAdjacentGroups(Groups[i-1], Groups[i]);
                if (i < Groups.Count - 1) ProcessAdjacentGroups(Groups[i], Groups[i+1]);
            }
        }

        public void RepaintAllGroups()
        {
            if (Groups == null || Groups[0] == null)
                return;
            
            Dictionary<Vector3, List<Tuple<FieldGroup, int>>> globalBoundaryMap = 
                new Dictionary<Vector3, List<Tuple<FieldGroup, int>>>();
            
            foreach (FieldGroup group in Groups)
            {
                group.Repaint();
                
                foreach (var kvp in group.boundaryVertices)
                {
                    Vector3 globalPos = group.transform.TransformPoint(kvp.Key);
                    
                    if (!globalBoundaryMap.ContainsKey(globalPos))
                    {
                        globalBoundaryMap[globalPos] = new List<Tuple<FieldGroup, int>>();
                    }
                    
                    foreach (int vertexIndex in kvp.Value)
                    {
                        globalBoundaryMap[globalPos].Add(new Tuple<FieldGroup, int>(group, vertexIndex));
                    }
                }
            }
            
            foreach (var kvp in globalBoundaryMap)
            {
                if (kvp.Value.Count > 1) 
                {
                    Vector3 avgNormal = Vector3.zero;
                    foreach (var tuple in kvp.Value)
                    {
                        FieldGroup group = tuple.Item1;
                        int vertexIndex = tuple.Item2;
                        Mesh mesh = group.GetComponent<MeshFilter>().sharedMesh;
                        avgNormal += mesh.normals[vertexIndex];
                    }
                    avgNormal /= kvp.Value.Count;
                    avgNormal = avgNormal.normalized;
                    
                    foreach (var tuple in kvp.Value)
                    {
                        FieldGroup group = tuple.Item1;
                        int vertexIndex = tuple.Item2;
                        Mesh mesh = group.GetComponent<MeshFilter>().sharedMesh;
                        
                        Vector3[] normals = mesh.normals;
                        normals[vertexIndex] = avgNormal;
                        mesh.normals = normals;
                        
                        group.GetComponent<MeshFilter>().sharedMesh = mesh;
                    }
                }
            }
        }
        
        private void ProcessAdjacentGroups(FieldGroup groupA, FieldGroup groupB)
        {
            Mesh meshA = groupA.GetComponent<MeshFilter>().sharedMesh;
            Mesh meshB = groupB.GetComponent<MeshFilter>().sharedMesh;
    
            if (meshA == null || meshB == null) return;
    
            Vector3[] vertsA = meshA.vertices;
            Vector3[] vertsB = meshB.vertices;
            Vector3[] normalsA = meshA.normals;
            Vector3[] normalsB = meshB.normals;
    
            float threshold = 0.01f;
    
            for (int i = 0; i < vertsA.Length; i++)
            {
                for (int j = 0; j < vertsB.Length; j++)
                {
                    if (Vector3.Distance(vertsA[i], vertsB[j]) < threshold)
                    {
                        Vector3 avgNormal = (normalsA[i] + normalsB[j]).normalized;
                        normalsA[i] = avgNormal;
                        normalsB[j] = avgNormal;
                    }
                }
            }
    
            meshA.normals = normalsA;
            meshB.normals = normalsB;
        }
        
        void RepaintGroups(List<FieldGroup> fgroup)
        {
            if (Groups == null || Groups[0] == null)
                return;
            foreach (FieldGroup fg in fgroup)
                fg.Repaint();

        }

        public void RepaintAll()
        {
            foreach (Point p in Points.array)
                p.terrain = this;
            foreach (Field f in Fields.array)
            {
                f.terrain = this;
            }
            Repaint(Fields.array);
            RepaintAllGroups();
        }
        //editor tools
        public void DrawTexture(List<Point> selectedPoints, int selectedColorIndex)
        {

            List<Field> fields = new List<Field>();
            foreach (Point point in selectedPoints)
            {
                point.DrawTexture(selectedColorIndex);
                foreach (Field f in point.fields)
                    if (!fields.Contains(f))
                        fields.Add(f);
            }
            Repaint(fields);
        }

        public void MoveHeight(List<Point> selectedPoints, Point selectedPoint, float maxValue)
        {
            List<Field> fields = new List<Field>();
            foreach (Point point in selectedPoints)
            {
                float val = 1 / Vector3.Distance(Point.PointToPosition(selectedPoint, this), Point.PointToPosition(point, this));
                if (val > 1)
                    val = 1;
                point.MoveHeight(maxValue * val);
                foreach (Field f in point.fields)
                    if (!fields.Contains(f))
                        fields.Add(f);
            }
            Repaint(fields);
        }

        public void MoveHeightFlat(List<Point> selectedPoints, Point selectedPoint)
        {
            List<Field> fields = new List<Field>();
            foreach (Point point in selectedPoints)
            {
                point.SetHeight(selectedPoint.heightValue);
                foreach (Field f in point.fields)
                    if (!fields.Contains(f))
                        fields.Add(f);
            }
            Repaint(fields);
        }


        public void CreateCliffs(List<Point> selectedPoints, int cliffValue)
        {
            if (cliffValue > 9 || cliffValue < -9)
                return;

            List<Field> fields = new List<Field>();
            foreach (Point point in selectedPoints)
            {
                if (point.cliffValue != cliffValue && point.CliffCondition(cliffValue))
                {
                    point.SetCliff(cliffValue);
                    foreach (Field f in point.fields)
                        if (!fields.Contains(f))
                            fields.Add(f);
                }

            }
            Repaint(fields);
        }

        public void CreateRamps(List<Point> selectedPoints)
        {
            List<Field> fields = new List<Field>();
            foreach (Point point in selectedPoints)
            {
                if (point.rampValue == 0 && point.RampCondition())
                {
                    point.SetRamp();
                    for (int x = -1; x <= 1; x++)
                        for (int y = -1; y <= 1; y++)
                        {
                            int xx = point.x + x;
                            int yy = point.y + y;
                            if (xx >= 0 && xx < Width + 1 && yy >= 0 && yy < Height + 1)
                                fields.AddRange(Points[xx, yy].fields);
                        }
                }
            }
            Repaint(fields);
        }

        public void RemoveRamps(List<Point> selectedPoints)
        {
            List<Field> fields = new List<Field>();
            foreach (Point point in selectedPoints)
            {
                if (point.rampValue == 1)
                {
                    point.rampValue = 0;
                    for (int x = -1; x <= 1; x++)
                        for (int y = -1; y <= 1; y++)
                        {
                            int xx = point.x + x;
                            int yy = point.y + y;
                            if (xx >= 0 && xx < Width + 1 && yy >= 0 && yy < Height + 1)
                                fields.AddRange(Points[xx, yy].fields);
                        }
                }
            }
            Repaint(fields);
        }
        public void CreateWater(List<Point> selectedPoints, int cliffValue)
        {
            List<Field> fields = new List<Field>();
            foreach (Point point in selectedPoints)
            {
                if (cliffValue > -9 && point.CliffCondition( cliffValue))
                {
                    if (point.waterValue != 0)
                    {
                        point.SetCliff(cliffValue);
                        foreach (Field f in point.fields)
                            if (!fields.Contains(f))
                                fields.Add(f);
                    }
                    else if (point.WaterCondition(cliffValue))
                    {

                        point.waterValue = point.cliffValue - 3 * CliffHeight / 4;
                        point.SetCliff(cliffValue);
                        foreach (Field f in point.fields)
                            if (!fields.Contains(f))
                                fields.Add(f);
                    }
                }
            }
            Repaint(fields);
        }

        public void CreatePrefabs(List<Point> selectedPoints, int selectedPrefab)
        {
            List<Field> fields = new List<Field>();
            foreach (Point p in selectedPoints)
            {
                if (p.prefabOnPoint == null)
                {

                    if (p.x != 0 && p.x != p.terrain.Width && p.y != 0 && p.y != p.terrain.Height)
                    {
                        GameObject prefab = Instantiate(Prefabs[selectedPrefab], PrefabParent);
                        prefab.transform.localPosition = p.GetPosition()+Vector3.right*Random.Range(-PrefabRandomMovement, PrefabRandomMovement) + Vector3.forward * Random.Range(-PrefabRandomMovement, PrefabRandomMovement);
                        if (PrefabRandomRotation)
                            prefab.transform.localEulerAngles = Vector3.up * Random.Range(0, 360);
                        p.SetPrefab(prefab);
                    }
                }
            }
            //Repaint(fields);
        }

        public void DeletePrefabs(List<Point> selectedPoints)
        {
            List<Field> fields = new List<Field>();
            foreach (Point point in selectedPoints)
            {
                if (point.prefabOnPoint != null)
                {
                    DestroyImmediate(point.prefabOnPoint);
                }
            }
            //Repaint(fields);
        }

        public void Repaint(List<Field> fields)
        {
            List<FieldGroup> fg = new List<FieldGroup>();
            foreach (Field f in fields.Distinct())
            {
                f.Repaint();
                if (!fg.Contains(f.owner))
                    fg.Add(f.owner);
            }
            RepaintGroups(fg);
        }

        public void Repaint(Field[] fields)
        {
            List<FieldGroup> fg = new List<FieldGroup>();
            foreach (Field f in fields.Distinct())
            {
                f.Repaint();
                if (!fg.Contains(f.owner))
                    fg.Add(f.owner);
            }
            RepaintGroups(fg);
        }


        public List<Point> GetCubicSelectedPoints(Point selectedPoint, float penSize, int symmetry)
        {
            List<Point> selectedPoints = new List<Point>();


            int r = Mathf.RoundToInt(penSize);
            if (penSize <= 1)
            {
                selectedPoints.Add(selectedPoint);
                switch (symmetry)
                {
                    case 1:
                        selectedPoints.Add(Points[Width - selectedPoint.x, Height - selectedPoint.y]);
                        break;
                    case 2:
                        selectedPoints.Add(Points[selectedPoint.x, Height - selectedPoint.y]);
                        break;
                    case 3:
                        selectedPoints.Add(Points[Width - selectedPoint.x, selectedPoint.y]);
                        break;
                }
                return selectedPoints;
            }
            selectedPoints.Add(selectedPoint);
            for (int x = -r; x <= r; x++)
                for (int y = -r; y <= r; y++)
                {
                    int xx = selectedPoint.x + x;
                    int yy = selectedPoint.y + y;
                    if (xx >= 0 && xx < Width + 1 && yy >= 0 && yy < Height + 1)
                    {
                        selectedPoints.Add(Points[xx, yy]);
                        //symmetry
                        switch (symmetry)
                        {
                            case 1:
                                selectedPoints.Add(Points[Width - xx, Height - yy]);
                                break;
                            case 2:
                                selectedPoints.Add(Points[xx, Height - yy]);
                                break;
                            case 3:
                                selectedPoints.Add(Points[Width - xx, yy]);
                                break;
                        }
                    }
                }

            return selectedPoints.Distinct().ToList();
        }

        public List<Point> GetCircleSelectedPoints(Point selectedPoint, float penSize, int symmetry)
        {
            List<Point> selectedPoints = new List<Point>();

            int r = Mathf.RoundToInt(penSize);
            if (penSize <= 1)
            {
                selectedPoints.Add(selectedPoint);
                switch (symmetry)
                {
                    case 1:
                        selectedPoints.Add(Points[Width - selectedPoint.x, Height - selectedPoint.y]);
                        break;
                    case 2:
                        selectedPoints.Add(Points[selectedPoint.x, Height - selectedPoint.y]);
                        break;
                    case 3:
                        selectedPoints.Add(Points[Width - selectedPoint.x, selectedPoint.y]);
                        break;
                }
                return selectedPoints;
            }
            selectedPoints.Add(selectedPoint);
            for (int x = -r; x <= r; x++)
                for (int y = -r; y <= r; y++)
                {
                    int xx = selectedPoint.x + x;
                    int yy = selectedPoint.y + y;
                    if (xx >= 0 && xx < Width + 1 && yy >= 0 && yy < Height + 1 && Vector3.Distance(Point.PointToPosition(selectedPoint, this), Point.PointToPosition(Points[xx, yy], this)) <= r)
                    {
                        selectedPoints.Add(Points[xx, yy]);
                        //symmetry
                        switch (symmetry)
                        {
                            case 1:
                                selectedPoints.Add(Points[Width - xx, Height - yy]);
                                break;
                            case 2:
                                selectedPoints.Add(Points[xx, Height - yy]);
                                break;
                            case 3:
                                selectedPoints.Add(Points[Width - xx, yy]);
                                break;
                        }
                    }
                }
            return selectedPoints.Distinct().ToList();
        }

        public bool IsPointOnTerrain(Vector3 pos)
        {
            float xMin = transform.position.x;
            float xMax = transform.position.x + Width * TerrainScale;
            float zMin = transform.position.z;
            float zMax = transform.position.z + Height * TerrainScale;

            return pos.x < xMax && pos.x > xMin && pos.z < zMax && pos.z > zMin;
        } 
    }
}
