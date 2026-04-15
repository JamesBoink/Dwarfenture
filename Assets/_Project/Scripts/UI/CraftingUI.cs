using UnityEngine;

namespace StormPig {
    public class CraftingUI : MonoBehaviour {
        [SerializeField] private GameObject craftingWindow;

        private void Awake() {
      //      still.InteractWithStation += HandleCraftingWindow;
        }

        private void HandleCraftingWindow() {
            if(craftingWindow.activeInHierarchy && craftingWindow.activeSelf) {
                craftingWindow.SetActive(false);
            } else {
                craftingWindow.SetActive(true);
            }
        }

        private void DisplayRecipes() {

        }
    }
}