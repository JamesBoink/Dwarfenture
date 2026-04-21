using UnityEngine;

namespace StormPig.Managers {
    public class CraftingStationsManager : MonoBehaviour {
        public static CraftingStationsManager Instance;

        private UI.CraftingUI _craftingUI = null;

        private void Awake() {
            Singleton();
        }

        private void Singleton() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(this);
                return;
            }
            Destroy(this);
        }

        public void OpenStation(Crafting.CraftingStationData c) {
            if (_craftingUI == null) {
                _craftingUI = FindAnyObjectByType<UI.CraftingUI>();
            }

            _craftingUI.OpenStation(c); 
        }
    }
}