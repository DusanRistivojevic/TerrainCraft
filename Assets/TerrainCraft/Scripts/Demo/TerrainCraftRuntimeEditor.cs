using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
namespace TerrainCraft
{
    [RequireComponent(typeof(TerrainCraft))]
    public class TerrainCraftRuntimeEditor : MonoBehaviour
    {
        [SerializeField]
        private TerrainCraftUI ui;


        [HideInInspector]
        public float penSize = 1;
        [HideInInspector]
        public bool snapToGrid = false;
        [HideInInspector]
        public int selectedBrush;
        [HideInInspector]
        public int symmetry;
        [HideInInspector]
        public int selectedColorIndex = 0;
        [HideInInspector]
        public int selectedPrefab;
        private TerrainCraft terrain;
        private Transform cursor;

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


        delegate void DrawDelegate();

        DrawDelegate drawDelegate;



        private void Awake()
        {
            terrain = GetComponent<TerrainCraft>();
            cursor = terrain.cursorObject.transform;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Camera.main.ScreenToViewportPoint(Input.mousePosition).x < 0.75f && Physics.Raycast(ray, out hit, 100))
            {
                if (!cursor.gameObject.activeSelf)
                    cursor.gameObject.SetActive(true);
                selectionPointPosition = hit.point;
                if (snapToGrid)
                {
                    Vector3 p = (selectionPointPosition - terrain.transform.position);
                    p = new Vector3(Mathf.RoundToInt(p.x), 0, Mathf.RoundToInt(p.z));
                    selectionPointPosition = terrain.transform.position + p;
                }
                if (cursor.transform.position != selectionPointPosition + 20 * Vector3.up)
                    cursor.transform.position = selectionPointPosition + 20 * Vector3.up;

                Point pi = Point.PositionToPoint(selectionPointPosition, terrain);
                if (Input.GetMouseButtonDown(0))
                {
                    
                    selectedPoint = pi;
                    selectedCliffValue = points[(int)pi.x, (int)pi.y].cliffValue;
                    Click();
                }

                if (pi != selectedPoint && Input.GetMouseButton(0))
                {
                    selectedPoint = pi;
                    Click();
                }

                if (penSize != cursor.transform.localScale.x*2)
                    cursor.transform.localScale = new Vector3(penSize*2, penSize*2, 100);
            }
            else
                if (cursor.gameObject.activeSelf)
                    cursor.gameObject.SetActive(false);
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
                        prefab.transform.localScale = Random.Range(1 - terrain.prefabRandomSize, 1) * Vector3.one;
                        p.SetPrefab(prefab);
                    }
                }
            }
            //Repaint(fields);
        }

        public void DeletePrefabs()
        {
            List<Field> fields = new List<Field>();
            foreach (Point point in GetSelectedPoints())
            {
                if (point.prefabOnPoint != null)
                {

                    DestroyImmediate(point.prefabOnPoint);
                }
            }
            //Repaint(fields);
        }

        private int selectedTool;

        public void SelectTool(int toolIndex)
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
                    drawDelegate = DeletePrefabs;
                    break;


            }
        }


        private void SelectPrefab(int toolIndex)
        {
            selectedPrefab = toolIndex;


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
