using UnityEngine;
using UnityEditor;
namespace TerrainCraft
{
    public class TerrainCraftMenuButton
    {
        // Add a new menu item under an existing menu

        [MenuItem("GameObject/Create TerrainCraft")]
        private static void NewMenuOption()
        {
            Object prefab = AssetDatabase.LoadAssetAtPath("Assets/TerrainCraft/Prefabs/TerrainCraft.prefab", typeof(Object));
            Undo.RegisterCreatedObjectUndo(PrefabUtility.InstantiatePrefab(prefab), "TerainCraft Created");
        }
    }
}