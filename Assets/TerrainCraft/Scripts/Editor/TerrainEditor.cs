using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Rendering.Universal;

namespace TerrainCraft
{
    [CustomEditor(typeof(TerrainCraft))]
    public class TerrainEditor : Editor
    {
        private List<Texture2D> uiTextures;
        private List<Texture2D> brushTextures;
        private List<Texture2D> symmetryTextures;
        private List<Texture2D> prefabIcons;
        private Texture2D texture;
        private float paintSize = 1;
        private int symmetry;
        private bool showInspector;
        private bool snapToGrid;
        private bool snap;
        private float penSize = 1;
        private int selectedBrush;
        private int selectPrefab = -1;
        private TerrainCraft terrain
        {
            get
            {
                return (TerrainCraft)target;
            }
        }

        private Transform cursor
        {
            get
            {
                return terrain.cursorObject.transform;
            }
        }

        private Point selectedPoint;
        private Vector3 selectionPointPosition;

        private Point2DArray points
        {
            get
            {
                return terrain.Points;
            }
        }

        private int selectedCliffValue = 0;
        private int selectedColorIndex = 0;


        delegate void DrawDelegate();

        DrawDelegate drawDelegate;

        public override void OnInspectorGUI()
        {
            if (terrain.Fields == null && terrain.Points == null)
            {
                DrawDefaultInspector();

                if (GUILayout.Button("Create New Terrain"))
                {
                    if (terrain.transform.childCount > 0)
                        return;

                    terrain.Fields = null;
                    terrain.Points = null;
                    EditorUtility.UnloadUnusedAssetsImmediate();
                    terrain.CreateNew();
                    if(PrefabUtility.IsPartOfRegularPrefab(terrain.gameObject))
                        PrefabUtility.UnpackPrefabInstance(terrain.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                }
                if (GUILayout.Button("Open Terrain from json"))
                {
                    string path = EditorUtility.OpenFilePanel("Open TerrainCraft Json", "", "json");
                    if (path.Length != 0)
                    {
                        string json = File.ReadAllText(path);
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
                                terrain.Points.array[i].prefabOnPoint.transform.localPosition = model.Points[i].prefabPosition;
                                terrain.Points.array[i].prefabOnPoint.transform.eulerAngles = model.Points[i].prefabDirection;
                                terrain.Points.array[i].prefabOnPoint.transform.localScale = model.Points[i].prefabScale;
                            }

                        }
                        terrain.RepaintAll();
                    }
                }
            }
            else
            {
                

                GUILayout.Label("Brush", EditorStyles.boldLabel);
                paintSize = GUILayout.HorizontalScrollbar(paintSize, 0.5f, 1.0f, 10.0f);
                if (paintSize != penSize)
                    SetSize(paintSize);

                snap = GUILayout.Toggle(snap,"Snap to grid");
                if (snapToGrid != snap)
                    snapToGrid = snap;

                brushTextures = new List<Texture2D>();
                for (int a = 1; a < 3; a++)
                    brushTextures.Add(Resources.Load<Texture2D>("ui icons/brush" + a));
                selectedBrush = GUILayout.SelectionGrid(selectedBrush, brushTextures.ToArray(), 2, "gridlist", GUILayout.Width(80), GUILayout.Height(40));

                
                GUILayout.Label("Symmetry", EditorStyles.boldLabel);
                symmetryTextures = new List<Texture2D>();
                for (int ti = 0; ti < 12; ti++)
                    symmetryTextures.Add(Resources.Load<Texture2D>("ui icons/symmetry" + ti));
                symmetry = GUILayout.SelectionGrid(symmetry, symmetryTextures.ToArray(), symmetryTextures.Count, "gridlist", GUILayout.Width(320), GUILayout.Height(40));
                

                GUILayout.Label("Textures", EditorStyles.boldLabel);
                int ii = GUILayout.SelectionGrid(selectedColorIndex, terrain.Icons, 8, "gridlist", GUILayout.Width(320), GUILayout.Height(40));
                if (ii != selectedColorIndex)
                {
                    selectedColorIndex = ii;
                    SelectTool(0);
                }

                GUILayout.Label("Tools", EditorStyles.boldLabel);
                uiTextures = new List<Texture2D>();
                for (int ti = 0; ti < 12; ti++)
                    uiTextures.Add(Resources.Load<Texture2D>("ui icons/ui0" + ti));
                int iii = GUILayout.SelectionGrid(selectedTool, uiTextures.ToArray(), 12, "gridlist", GUILayout.Width(320), GUILayout.Height(40));
                if (iii != selectedTool)
                {
                    selectedTool = 0;
                    SelectTool(iii);
                    SelectPrefab(0);
                    if (iii == 11)
                    {
                        SelectPrefab(terrain.Prefabs.Length);
                    }
                }
                if (selectedTool == 10)
                {
                    GUILayout.Label("Prefabs", EditorStyles.boldLabel);
                    prefabIcons = new List<Texture2D>();
                    for (int i = 0; i < terrain.Prefabs.Length; i++)
                    {
                        prefabIcons.Add(uiTextures[10]);
                    }
                    prefabIcons.Add(uiTextures[11]);

                    selectPrefab = GUILayout.SelectionGrid(selectedPrefab, prefabIcons.ToArray(), prefabIcons.Count, "gridlist", GUILayout.Width(prefabIcons.Count * 40), GUILayout.Height(40));
                    if (selectPrefab != selectedPrefab)
                    {
                        if (selectPrefab >= terrain.Prefabs.Length)
                        {
                            SelectTool(11);
                            SelectPrefab(terrain.Prefabs.Length);
                        }
                        else
                        {
                            SelectTool(10);
                            SelectPrefab(selectPrefab);
                        }
                    }

                }
                if (GUILayout.Button("Remove"))
                {
                    while (terrain.transform.childCount > 0)
                    {
                        DestroyImmediate(terrain.transform.GetChild(0).gameObject);
                    }
                    terrain.Fields = null;
                    terrain.Points = null;
                    System.GC.Collect();
                }
                if (GUILayout.Button("Save as Json"))
                {
                    JSONmodel model = new JSONmodel();
                    model.Width = terrain.Width;
                    model.Height = terrain.Height;
                    model.Points = new JSONPoint[(model.Width + 1) * (model.Height + 1)];
                    for (int i = 0; i < model.Points.Length; i++)
                    {
                        model.Points[i] = new JSONPoint();
                        model.Points[i].x = terrain.Points.array[i].x;
                        model.Points[i].y = terrain.Points.array[i].y;

                        model.Points[i].cliffValue = terrain.Points.array[i].cliffValue;
                        model.Points[i].colorValue = terrain.Points.array[i].colorValue;
                        model.Points[i].heightValue = terrain.Points.array[i].heightValue;
                        model.Points[i].rampValue = terrain.Points.array[i].rampValue;
                        model.Points[i].waterValue = terrain.Points.array[i].waterValue;

                        if (terrain.Points.array[i].prefabOnPoint != null)
                        {
                            model.Points[i].prefabOnPoint = terrain.Points.array[i].prefabIndex;
                            model.Points[i].prefabPosition = terrain.Points.array[i].prefabOnPoint.transform.localPosition;
                            model.Points[i].prefabDirection = terrain.Points.array[i].prefabOnPoint.transform.eulerAngles;
                            model.Points[i].prefabScale = terrain.Points.array[i].prefabOnPoint.transform.localScale;

                        }
                        else
                        {
                            model.Points[i].prefabOnPoint = -1;
                            model.Points[i].prefabPosition = Vector3.zero;
                            model.Points[i].prefabDirection = Vector3.zero;
                            model.Points[i].prefabScale = Vector3.zero;
                        }
                    }

                    var path = EditorUtility.SaveFilePanel("Save TerrainCraft as Json","", "TerrainCraft" + ".json","json");

                    if (path.Length != 0)
                    {
                        string json = JsonUtility.ToJson(model, true);
                        if (json != null)
                            File.WriteAllText(path, json);
                    }

                }
            }
        }
        void OnEnable()
        {
            Undo.undoRedoPerformed += UndoRedo;
        }
        void OnDisable()
        {
            Undo.undoRedoPerformed -= UndoRedo;
        }
        void UndoRedo()
        {
            terrain.RepaintAll();
        }
        void OnSceneGUI()
        {
            if (terrain.cursorObject == null)
            {
                if (GUIUtility.hotControl == GUIUtility.GetControlID(FocusType.Passive))
                    GUIUtility.hotControl = 0;

                return;
            }
            Event e = Event.current;
            Vector3 mousePosition = Event.current.mousePosition;

            float mult = EditorGUIUtility.pixelsPerPoint;
            mousePosition.y = Camera.current.pixelHeight - mousePosition.y * mult;
            mousePosition.x *= mult;

            Ray ray = Camera.current.ScreenPointToRay(mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                selectionPointPosition = hit.point;
            }
            else
            {
                if (GUIUtility.hotControl == GUIUtility.GetControlID(FocusType.Passive))
                    GUIUtility.hotControl = 0;
                if (cursor.gameObject.activeSelf)
                    cursor.gameObject.SetActive(false);
            }
            if (terrain.IsPointOnTerrain(selectionPointPosition))
            {
                if (!cursor.gameObject.activeSelf)
                    cursor.gameObject.SetActive(true);
                if (snapToGrid)
                {
                    Vector3 p = (selectionPointPosition - terrain.transform.position);
                    p = new Vector3(Mathf.RoundToInt(p.x), 0, Mathf.RoundToInt(p.z));
                    selectionPointPosition = terrain.transform.position + p;
                }
                if (cursor.transform.position != selectionPointPosition + 20 * Vector3.up)
                    cursor.transform.position = selectionPointPosition + 20 * Vector3.up;

                if ((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0)
                {

                    Point pi = Point.PositionToPoint(selectionPointPosition, terrain);
                    if (e.type == EventType.MouseDown)
                    {
                        GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                        e.Use();
                        selectedCliffValue = points[(int)pi.x, (int)pi.y].cliffValue;
                        selectedPoint = pi;

                        Undo.RegisterCompleteObjectUndo(terrain.Points, "TerrainCraft Changed");
                        EditorUtility.SetDirty(terrain);
                        Click();    
                    }

                    if (pi != selectedPoint || e.type == EventType.MouseDown)
                    {
                        selectedPoint = pi;
                        Click();
                    }
                }
                if (penSize != cursor.transform.localScale.x*2)
                    cursor.transform.localScale = new Vector3( penSize*2,penSize*2,100) ;
            }
            else
            {
                if (GUIUtility.hotControl == GUIUtility.GetControlID(FocusType.Passive))
                    GUIUtility.hotControl = 0;
                if(cursor.gameObject.activeSelf)
                    cursor.gameObject.SetActive(false);
            }
            if (e.button == 0 && e.type == EventType.MouseUp)
            {

                Undo.IncrementCurrentGroup();
                if(selectedPoint != null)
                    selectedCliffValue = points[(int)selectedPoint.x, (int)selectedPoint.y].cliffValue;
                if (GUIUtility.hotControl == GUIUtility.GetControlID(FocusType.Passive))
                    GUIUtility.hotControl = 0;
            }
        }

        void Click()
        {
            if (drawDelegate != null)
                drawDelegate();
        }

        void DrawTexture()
        {
            terrain.DrawTexture(GetSelectedPoints(), selectedColorIndex);
        }

        void MoveHeightUp()
        {
            terrain.MoveHeight(GetSelectedPoints(), selectedPoint, 0.05f);
        }

        void MoveHeightDown()
        {
            terrain.MoveHeight(GetSelectedPoints(), selectedPoint, -0.05f);
        }

        void CreateCliffsUp()
        {
            int cliffValue = selectedCliffValue + 1;
            CreateCliffs(cliffValue);
        }

        void CreateCliffsDown()
        {
            int cliffValue = selectedCliffValue - 1;
            CreateCliffs(cliffValue);
        }

        void CreateCliffs(int cliffValue)
        {
            terrain.CreateCliffs(GetSelectedPoints(), cliffValue);
        }

        void FlatCliffsAndHeight()
        {
            int cliffValue = selectedCliffValue;
            CreateCliffs(cliffValue);
            terrain.MoveHeightFlat(GetSelectedPoints(), selectedPoint);
        }

        void CreateRamps()
        {
            terrain.CreateRamps(GetSelectedPoints());
        }

        void RemoveRamps()
        {
            terrain.RemoveRamps(GetSelectedPoints());
        }

        void CreateShallowWater()
        {
            terrain.CreateWater(GetSelectedPoints(), selectedCliffValue - 1);
        }
        void CreateDeepWater()
        {
            terrain.CreateWater(GetSelectedPoints(), selectedCliffValue - 2);
        }
        public void CreatePrefabs()
        {
            if (terrain.PrefabParent == null)
            {
                GameObject tp = new GameObject("Prefab Parent");
                if (terrain.HideChildren)
                    tp.hideFlags = HideFlags.HideInHierarchy;
                tp.transform.parent = terrain.transform;
                tp.transform.localPosition = Vector3.zero;
                tp.transform.localEulerAngles = Vector3.zero;
                terrain.PrefabParent = tp.transform;
            }
            List<Field> fields = new List<Field>();
            int undoID = Undo.GetCurrentGroup();
            foreach (Point p in GetSelectedPoints())
            {
                if (p.prefabOnPoint == null && p.waterValue == 0)
                {

                    if (p.x != 0 && p.x != p.terrain.Width && p.y != 0 && p.y != p.terrain.Height)
                    {
                        GameObject prefab = Instantiate(terrain.Prefabs[selectedPrefab], terrain.PrefabParent);
                        prefab.transform.localPosition = p.GetPosition() + Vector3.right * Random.Range(-terrain.PrefabRandomMovement, terrain.PrefabRandomMovement) + Vector3.forward * Random.Range(-terrain.PrefabRandomMovement, terrain.PrefabRandomMovement);
                        if (terrain.PrefabRandomRotation)
                            prefab.transform.localEulerAngles = Vector3.up * Random.Range(0, 360);
                        prefab.transform.localScale = Random.Range(1-terrain.prefabRandomSize, 1) * Vector3.one;
                        p.prefabIndex = selectedPrefab;
                        p.SetPrefab(prefab);
                        Undo.RegisterCreatedObjectUndo(prefab, "TerainCraft Created Prefabs");
                        Undo.CollapseUndoOperations(undoID);
                    }
                }
            }
            //Repaint(fields);
        }

        public void DeletePrefabs()
        {
            List<Field> fields = new List<Field>();
            int undoID = Undo.GetCurrentGroup();
            foreach (Point point in GetSelectedPoints())
            {
                if (point.prefabOnPoint != null)
                {
                    point.prefabIndex = -1;
                    Undo.DestroyObjectImmediate(point.prefabOnPoint);
                    Undo.CollapseUndoOperations(undoID);
                }
            }
            //Repaint(fields);
        }

        void CreateTrees()
        {
            terrain.CreatePrefabs(GetSelectedPoints(), selectedPrefab);
        }
        void DeleteTrees()
        {
            terrain.DeletePrefabs(GetSelectedPoints());
        }

        private int selectedTool;

        private void SelectTool(int toolIndex)
        {
            selectedTool = toolIndex;

            switch (selectedTool)
            {
                case 0:
                    drawDelegate = DrawTexture;
                    break;
                case 1:
                    drawDelegate = FlatCliffsAndHeight;
                    break;
                case 2:
                    drawDelegate = CreateCliffsUp;
                    break;
                case 3:
                    drawDelegate = CreateCliffsDown;
                    break;
                case 4:
                    drawDelegate = MoveHeightUp;
                    break;
                case 5:
                    drawDelegate = MoveHeightDown;
                    break;
                case 6:
                    drawDelegate = CreateRamps;
                    break;
                case 7:
                    drawDelegate = RemoveRamps;
                    break;
                case 8:
                    drawDelegate = CreateShallowWater;
                    break;
                case 9:
                    drawDelegate = CreateDeepWater;
                    break;
                case 10:
                    drawDelegate = CreatePrefabs;
                    break;
                case 11:
                    drawDelegate = DeletePrefabs ;
                    break;


            }
        }

        private int selectedPrefab;

        private void SelectPrefab(int toolIndex)
        {
            selectedPrefab = toolIndex;


        }

        private void SetSize(float size)
        {
            penSize = size;
        }

        List<Point> GetSelectedPoints()
        {

            switch (selectedBrush)
            {
                case 0:
                    return terrain.GetCircleSelectedPoints(selectedPoint, penSize, symmetry);
                case 1:
                    return terrain.GetCubicSelectedPoints(selectedPoint, penSize, symmetry);

            }
            return null;
        }

        private void SelectBrush(int brushIndex)
        {
            selectedBrush = brushIndex;
        }
    }
}