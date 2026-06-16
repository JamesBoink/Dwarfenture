using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


namespace StormPig.UI {
    public class CraftingUI : MonoBehaviour {
        [SerializeField] private RecipeUI _recipeUIPrefab;
        [SerializeField] private RecipePreview _recipePreviewPrefab;
        [SerializeField] private GameObject _craftingWindow;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Image _icon;

        private List<RecipeUI> _recipes = new List<RecipeUI>();

        private Crafting.CraftingStationData stationData;

        public void OpenStation(Crafting.CraftingStationData c) {
            stationData = c;
            _craftingWindow.SetActive(true);
            _nameText.text = c.Name;
            _descriptionText.text = c.Description;
            _icon.sprite = c.Icon;
            ShowRecipes();
        }

        private void ShowRecipes() {
            ClearRecipes();
            for(int i=0; i < stationData.AvailableRecipes.Length; i++) {
                _recipes.Add(Instantiate(_recipeUIPrefab));
                _recipes[_recipes.Count - 1].OnCreate(stationData.AvailableRecipes[i]);
            }
        }

        private void ClearRecipes() {
            for(int i = _recipes.Count-1; i > 0; i--) {
                Destroy(_recipes[i].gameObject);
            }
            _recipes.Clear();
        }

    }
}