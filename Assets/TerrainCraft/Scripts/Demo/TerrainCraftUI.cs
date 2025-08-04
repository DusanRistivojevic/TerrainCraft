using System;
using UnityEngine;
using UnityEngine.UI;
namespace TerrainCraft
{
    public class TerrainCraftUI : MonoBehaviour
    {
        [SerializeField]
        private TerrainCraftRuntimeEditor terrainRE;
        [SerializeField]
        private TerrainCraft terrain;

        [SerializeField]
        private Color selectColor;



        [SerializeField]
        private RawImage[] textureIconsRI;
        [SerializeField]
        private Slider pensizeSlider;
        [SerializeField]
        private Toggle snapToGrid;
        [SerializeField]
        private Button[] shapeButtons;
        [SerializeField]
        private Button[] symmetryButtons;
        [SerializeField]
        private Button[] textureButtons;
        [SerializeField]
        private Button[] toolButtons;
        [SerializeField]
        private Button[] prefabButtons;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            for (int i = 0; i < textureIconsRI.Length; i++)
            {
                textureIconsRI[i].texture = terrain.Icons[i];
            }
            ChangeBrushShape(0);
            ChangeSymmetry(0);
            SelectTexture(0);
        }

        public void OnPenSizeValueChange()
        {
            terrainRE.penSize = pensizeSlider.value;
        }

        public void OnSnapValueChanged()
        {
            terrainRE.snapToGrid = snapToGrid.isOn;
        }

        public void ChangeBrushShape(int index)
        {
            for (int i = 0; i < shapeButtons.Length; i++)
            {
                shapeButtons[i].GetComponent<Image>().color = Color.white;
            }
            shapeButtons[index].GetComponent<Image>().color = selectColor;
            terrainRE.selectedBrush = index;
        }

        public void ChangeSymmetry(int index)
        {
            for(int i =0;i< symmetryButtons.Length;i++) 
            {
                symmetryButtons[i].GetComponent<Image>().color = Color.white;
            }
            symmetryButtons[index].GetComponent<Image>().color = selectColor;
            terrainRE.symmetry = index;
        }

        public void SelectTexture(int index)
        {
            for (int i = 0; i < textureButtons.Length; i++)
            {
                textureButtons[i].GetComponent<Image>().color = Color.white;
            }
            textureButtons[index].GetComponent<Image>().color = selectColor;
            terrainRE.selectedColorIndex = index;
            SelectTool(0);
        }
        public void SelectTool(int index)
        {
            for (int i = 0; i < prefabButtons.Length; i++)
            {
                prefabButtons[i].GetComponent<Image>().color = Color.white;
            }
            for (int i = 0; i < toolButtons.Length; i++)
            {
                toolButtons[i].GetComponent<Image>().color = Color.white;
            }
            toolButtons[index].GetComponent<Image>().color = selectColor;
            terrainRE.SelectTool(index);
            if (index == 10)
                SelectPrefab(0);
        }
        public void SelectPrefab(int index)
        {
            for (int i = 0; i < prefabButtons.Length; i++)
            {
                prefabButtons[i].GetComponent<Image>().color = Color.white;
            }
            prefabButtons[index].GetComponent<Image>().color = selectColor;
            terrainRE.selectedPrefab = index;

            for (int i = 0; i < toolButtons.Length; i++)
            {
                toolButtons[i].GetComponent<Image>().color = Color.white;
            }
            toolButtons[10].GetComponent<Image>().color = selectColor;
            terrainRE.SelectTool(10);
        }
    }
}
