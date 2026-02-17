using UnityEngine;

namespace StormPig.Inventory {
    public class InventoriesManager : MonoBehaviour {
        public static InventoriesManager Instance;
        [SerializeField] private Inventory[] InventoriesInLevel;

        private void Awake() {
            Singleton();
        }

        private void Singleton() {
            if(Instance == null) {
                Instance = this;
                DontDestroyOnLoad(this);
                return;
            }
            Destroy(this);
        }
    }
}